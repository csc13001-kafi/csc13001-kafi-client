using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Models;
using kafi.Repositories;

namespace kafi.ViewModels;

public partial class EmployeeViewModel(IEmployeeRepository repository) : ObservableValidator
{
    private readonly IEmployeeRepository _repository = repository;
    private const int DefaultPageSize = 10;

    [ObservableProperty]
    [Required]
    public partial string? UserName { get; set; }

    [ObservableProperty]
    [Required]
    [EmailAddress]
    public partial string? Email { get; set; }

    [ObservableProperty]
    [Required]
    [Phone]
    public partial string? Phone { get; set; }

    [ObservableProperty]
    [Required]
    public partial string? Address { get; set; }

    [ObservableProperty]
    [Required]
    public partial DateTimeOffset Birthdate { get; set; }

    [ObservableProperty]
    [Required]
    public partial int Salary { get; set; }

    [ObservableProperty]
    [Required]
    public partial TimeSpan StartShift { get; set; }

    [ObservableProperty]
    [Required]
    public partial TimeSpan EndShift { get; set; }

    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))]
    public partial int CurrentPage { get; set; } = 1;

    [ObservableProperty]
    public partial int TotalPages { get; set; } = 1;

    [ObservableProperty]
    public partial int PageSize { get; set; } = DefaultPageSize;

    [ObservableProperty]
    public partial int TotalEmployees { get; set; } = 0;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateEmployeeCommand))]
    public partial User SelectedUser { get; set; }

    private List<User> _fullEmployeeList = [];
    public ObservableCollection<User> Employees { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateEmployeeCommand))]
    public partial int EditedSalary { get; set; }

    private bool CanLoadEmployees => !Employees.Any();
    [RelayCommand(CanExecute = nameof(CanLoadEmployees))]
    private async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        try
        {
            _fullEmployeeList = [.. await _repository.GetAll()];
            UpdatePagedView();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddEmployeeAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            Message = "Please fill all the fields";
            return;
        }

        UserRequest employee = new()
        {
            Name = UserName,
            Email = Email,
            Phone = Phone,
            Address = Address,
            Salary = Salary,
            Birthdate = Birthdate.DateTime,
            StartShift = StartShift,
            EndShift = EndShift,
        };

        try
        {
            UserResponse newUserInfo = (UserResponse)await _repository.Add(employee);
            _fullEmployeeList.Add(new User
            {
                Id = newUserInfo.Id,
                Name = UserName,
                Image = newUserInfo.Image,
                Email = Email,
                Phone = Phone,
                Address = Address,
                Salary = Salary,
                Birthdate = Birthdate.DateTime,
                StartShift = StartShift,
                EndShift = EndShift
            });
            UpdatePagedView();
            DeleteAllInput();
        }
        catch (Exception)
        {
            Message = "Failed to add employee";
        }
    }

    private bool CanUpdateEmployee() => SelectedUser != null && EditedSalary > 0;
    [RelayCommand(CanExecute = nameof(CanUpdateEmployee))]
    private async Task UpdateEmployeeAsync()
    {
        if (EditedSalary <= 0)
            return;
        UserRequest updatedRequest = new()
        {
            // Keep original personal info
            Name = SelectedUser.Name,
            Email = SelectedUser.Email,
            Phone = SelectedUser.Phone,
            Address = SelectedUser.Address,
            Birthdate = SelectedUser.Birthdate,

            // Update only salary and shifts
            Salary = EditedSalary,
            StartShift = SelectedUser.StartShift,
            EndShift = SelectedUser.EndShift,
        };
        SelectedUser.Salary = EditedSalary;

        try
        {
            await _repository.Update(SelectedUser.Id, updatedRequest);
            _fullEmployeeList[_fullEmployeeList.FindIndex(u => u.Id == SelectedUser.Id)] = SelectedUser;
            UpdatePagedView();
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(Message));
        }
        catch (Exception)
        {
            Message = "Failed to update employee";
        }
    }

    [RelayCommand]
    private async Task DeleteEmployeeAsync(Guid id)
    {
        try
        {
            await _repository.Delete(id);
            var removedCount = _fullEmployeeList.RemoveAll(u => u.Id == id);

            if (removedCount > 0 &&
                Employees.Count == 1 &&
                CurrentPage > 1)
            {
                CurrentPage--;
            }

            UpdatePagedView();
        }
        catch (Exception) { }
    }

    [RelayCommand]
    private void DeleteAllInput()
    {
        UserName = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Salary = 0;
        StartShift = TimeSpan.Zero;
        EndShift = TimeSpan.Zero;
        Address = string.Empty;
        Birthdate = DateTimeOffset.Now;
        Message = string.Empty;
    }

    [RelayCommand]
    private void ViewEmployee(Guid id)
    {
        User user = _fullEmployeeList.FirstOrDefault(u => u.Id == id)!;
        SelectedUser = new User
        {
            Id = id,
            Image = user.Image,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Address = user.Address,
            Salary = user.Salary,
            Birthdate = user.Birthdate,
            StartShift = user.StartShift,
            EndShift = user.EndShift
        };
        EditedSalary = user.Salary;
    }

    private void UpdatePagedView()
    {
        TotalEmployees = _fullEmployeeList.Count;
        CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);

        TotalPages = (int)Math.Ceiling((double)TotalEmployees / PageSize);
        TotalPages = TotalPages == 0 ? 1 : TotalPages;

        var pagedItems = _fullEmployeeList
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        Employees.Clear();
        foreach (var item in pagedItems)
        {
            Employees.Add(item);
        }

        GoToPreviousPageCommand.NotifyCanExecuteChanged();
        GoToNextPageCommand.NotifyCanExecuteChanged();
    }

    private bool CanGoToPreviousPage => CurrentPage > 1;
    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private void GoToPreviousPage()
    {
        CurrentPage--;
        UpdatePagedView();
    }

    private bool CanGoToNextPage => CurrentPage < TotalPages;
    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private void GoToNextPage()
    {
        CurrentPage++;
        UpdatePagedView();
    }

    partial void OnPageSizeChanged(int value)
    {
        UpdatePagedView();
    }
}
