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
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

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

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateEmployeeCommand))]
        private User selectedUser;

        [ObservableProperty]
        private ImageSource selectedUserImage;

        private List<User> _fullEmployeeList = [];
        public ObservableCollection<User> Employees { get; } = new ObservableCollection<User>();

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

        private bool CanUpdateEmployee()
        {
            if (SelectedUser == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(SelectedUser.Name) ||
                string.IsNullOrEmpty(SelectedUser.Email) ||
                string.IsNullOrEmpty(SelectedUser.Phone) ||
                string.IsNullOrEmpty(SelectedUser.Address) ||
                SelectedUser.Salary == 0)
            {
                return false;
            }

            return true;
        }
        [RelayCommand(CanExecute = nameof(CanUpdateEmployee))]
        private async Task UpdateEmployeeAsync()
        {
            UserRequest updatedRequest = new()
            {
                Name = SelectedUser.Name,
                Email = SelectedUser.Email,
                Phone = SelectedUser.Phone,
                Address = SelectedUser.Address,
                Salary = SelectedUser.Salary,
                Birthdate = SelectedUser.Birthdate,
                StartShift = SelectedUser.StartShift,
                EndShift = SelectedUser.EndShift,
            };

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
            SelectedUserImage = new BitmapImage(new Uri(user.Image));
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
}
