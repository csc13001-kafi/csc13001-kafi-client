using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Contracts.Services;
using kafi.Models;
using kafi.Repositories;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace kafi.ViewModels;

public partial class InfoViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IInfoRepository _repository;
    private readonly IWindowService _windowService;
    private System.Timers.Timer? _successMessageTimer;
    private DispatcherQueue _dispatcherQueue;
    public bool IsManager => _authService.IsInRole(Role.Manager);

    private Window Window => _windowService.GetCurrentWindow();
    private StorageFile? _file;

    public InfoViewModel(IAuthService authService, IInfoRepository repository, IWindowService windowService)
    {
        _authService = authService;
        _repository = repository;
        _windowService = windowService;

        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _successMessageTimer = new System.Timers.Timer(3000); // 3 seconds
        _successMessageTimer.Elapsed += (s, e) =>
        {
            // Dispatch the UI update to the UI thread
            _dispatcherQueue.TryEnqueue(() =>
            {
                SuccessMessage = string.Empty;
            });
            _successMessageTimer.Stop();
        };
        _successMessageTimer.AutoReset = false;
    }

    private User User => _authService.CurrentUser!;

    [ObservableProperty]
    public partial bool IsPickerEnable { get; set; } = true;

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial ImageSource? Image { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateUserInfoCommand))]
    public partial string? Name { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateUserInfoCommand))]
    public partial string? Email { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateUserInfoCommand))]
    public partial string? Phone { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateUserInfoCommand))]
    public partial string? Address { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateUserInfoCommand))]
    public partial DateTime Birthdate { get; set; } = DateTime.Now;

    [ObservableProperty]
    public partial int? Salary { get; set; }

    [ObservableProperty]
    public partial TimeSpan StartShift { get; set; }

    [ObservableProperty]
    public partial TimeSpan EndShift { get; set; }

    [ObservableProperty]
    public partial string Message { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SuccessMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    private void SetStatusMessage(string message, bool isError)
    {
        if (isError)
        {
            ErrorMessage = message;
            SuccessMessage = string.Empty;
        }
        else
        {
            SuccessMessage = message;
            ErrorMessage = string.Empty;
        }
    }

    private bool CanLoadUserInfo() => User != null;
    [RelayCommand(CanExecute = nameof(CanLoadUserInfo))]
    public void LoadUserInfo()
    {
        IsLoading = true;
        SetStatusMessage(string.Empty, false); // Clear messages

        try
        {
            Image = string.IsNullOrEmpty(User.Image) ? null : new BitmapImage(new Uri(User.Image));
            Name = User.Name;
            Email = User.Email;
            Phone = User.Phone;
            Address = User.Address;
            Birthdate = User.Birthdate;
            Salary = User.Salary;
            StartShift = User.StartShift;
            EndShift = User.EndShift;
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading user info: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanUpdateUserInfo()
    {
        if (string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Address) ||
            string.IsNullOrWhiteSpace(Phone) ||
            !IsVietnamesePhoneNumber(Phone)
            )
        {
            return false;
        }
        return true;
    }
    [RelayCommand(CanExecute = nameof(CanUpdateUserInfo))]
    public async Task UpdateUserInfoAsync()
    {
        UserRequest userRequest = new()
        {
            Name = Name,
            Email = Email,
            Phone = Phone,
            Address = Address,
            Birthdate = Birthdate,
        };
        try
        {
            await _repository.UpdateInfo(userRequest);
            if (_file != null)
            {
                var request = new ImageRequest(await _file.OpenStreamForReadAsync(), _file.ContentType, _file.Name);
                await _repository.UpdateProfileImage(request);
            }
            await _authService.LoadCurrentUserFromToken();

            SetStatusMessage("Thông tin đã được cập nhật thành công.", isError: false);
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<User>(_authService.CurrentUser!));
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(Message));
        }
        catch (System.Exception ex)
        {
            SetStatusMessage($"Lỗi khi cập nhật thông tin: {ex.Message}", isError: true);
            System.Diagnostics.Debug.WriteLine($"Error updating user info: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task PickImageAsync()
    {
        IsPickerEnable = false;
        try
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(Window);

            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                _file = file;
                var stream = await file.OpenAsync(FileAccessMode.Read);
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream);
                Image = bitmapImage;
                UpdateUserInfoCommand.NotifyCanExecuteChanged();
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error picking image: {ex.Message}");
        }
        finally
        {
            IsPickerEnable = true;
        }
    }

    [RelayCommand]
    private void CancelUpdate()
    {
        _file = null;
        Image = string.IsNullOrEmpty(User.Image) ? null : new BitmapImage(new Uri(User.Image));
        Name = User.Name;
        Email = User.Email;
        Phone = User.Phone;
        Address = User.Address;
        Birthdate = User.Birthdate;

        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(string.Empty));
    }

    public async Task<string> ChangePasswordAsync(string oldPassword, string newPassword, string confirmPassword)
    {
        return await _authService.ChangePasswordAsync(oldPassword, newPassword, confirmPassword);
    }

    public static bool IsVietnamesePhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;
        phone = phone.Trim();
        string pattern = @"^(0|\+84)(3[2-9]|5[25689]|7[06-9]|8[1-5]|9[0-9])\d{7}$";
        return Regex.IsMatch(phone, pattern);
    }
    partial void OnSuccessMessageChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _successMessageTimer?.Start();
        }
    }
}