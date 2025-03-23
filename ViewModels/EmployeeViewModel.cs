using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kafi.Models;
using kafi.Repositories;

namespace kafi.ViewModels
{
    public partial class EmployeeViewModel(IEmployeeRepository repository) : ObservableValidator
    {
        private readonly IEmployeeRepository _repository = repository;
        private const int DefaultPageSize = 10;

        [ObservableProperty]
        [Required]
        private string? userName;

        [ObservableProperty]
        [Required]
        [EmailAddress]
        private string? email;

        [ObservableProperty]
        [Required]
        [Phone]
        private string? phone;

        [ObservableProperty]
        [Required]
        private string? address;

        [ObservableProperty]
        [Required]
        private DateTimeOffset birthdate;

        [ObservableProperty]
        [Required]
        private int salary;

        [ObservableProperty]
        [Required]
        private TimeSpan startShift;

        [ObservableProperty]
        [Required]
        private TimeSpan endShift;

        [ObservableProperty]
        private string message = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GoToPreviousPageCommand))]
        [NotifyCanExecuteChangedFor(nameof(GoToNextPageCommand))]
        private int currentPage = 1;

        [ObservableProperty]
        private int totalPages = 1;

        [ObservableProperty]
        private int pageSize = DefaultPageSize;

        [ObservableProperty]
        private int totalEmployees = 0;

        [ObservableProperty]
        private bool isLoading;

        private List<User> _fullEmployeeList = [];
        public ObservableCollection<User> Employees { get; } = new ObservableCollection<User>();

        private void UpdatePagedView()
        {
            CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);

            TotalPages = (int)Math.Ceiling((double)_fullEmployeeList.Count / PageSize);
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

        private bool CanLoadEmployees => !Employees.Any();
        [RelayCommand(CanExecute = nameof(CanLoadEmployees))]
        
        private async Task LoadEmployeesAsync()
        {
            IsLoading = true;
            try {
                _fullEmployeeList = [.. await _repository.GetAll()];
                TotalEmployees = _fullEmployeeList.Count;
                UpdatePagedView();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            } finally {
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
                await _repository.Add(employee);
                await LoadEmployeesAsync();
                DeleteAllInput();
            }
            catch (Exception)
            {
                Message = "Failed to add employee";
            }
        }

        private bool CanUpdateEmployee(User user)
        {
            if (user == null)
            {
                return false;
            }

            ValidateProperty(user.Name, nameof(UserName));
            ValidateProperty(user.Email, nameof(Email));
            ValidateProperty(user.Phone, nameof(Phone));
            ValidateProperty(user.Address, nameof(Address));
            ValidateProperty(user.Salary, nameof(Salary));
            ValidateProperty(
                DateTimeOffset.Parse(user.Birthdate.ToString()), nameof(Birthdate));

            return !HasErrors;
        }
        [RelayCommand(CanExecute = nameof(CanUpdateEmployee))]
        private async Task UpdateEmployeeAsync(User user)
        {
            var updatedUser = Employees.FirstOrDefault(e => e.Id == user.Id)!;

            UserRequest updatedRequest = new()
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Salary = user.Salary,
                Birthdate = user.Birthdate,
                StartShift = updatedUser.StartShift,
                EndShift = updatedUser.EndShift,
            };

            try
            {
                await _repository.Update(user.Id!, updatedRequest);
            }
            catch (Exception)
            {
                await LoadEmployeesAsync();
            }
        }

        [RelayCommand]
        private async Task DeleteEmployeeAsync(string id)
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
    }
}
