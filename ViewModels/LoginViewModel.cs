using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Contracts.Services;
using kafi.Models;

namespace kafi.ViewModels;

public partial class LoginViewModel(IAuthService authService, IWindowService windowService) : ObservableObject
{
    private readonly IAuthService _authService = authService;
    private readonly IWindowService _windowService = windowService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [NotifyCanExecuteChangedFor(nameof(RequestForgotPasswordOtpCommand))]
    public partial string UserName { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    public partial string Password { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; }

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsShowingUserName))]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    public partial bool IsLoggingIn { get; set; } = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsShowingUserName))]
    [NotifyCanExecuteChangedFor(nameof(RequestForgotPasswordOtpCommand))]
    public partial bool IsRequestForgotPassword { get; set; } = false;
    public bool IsShowingUserName => IsLoggingIn || IsRequestForgotPassword;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    public partial bool IsVerifyPassword { get; set; } = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetPasswordCommand))]
    public partial bool IsResetPassword { get; set; } = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    public partial string FirstOtpDigit { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    public partial string SecondOtpDigit { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    public partial string ThirdOtpDigit { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    public partial string FourthOtpDigit { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    public partial string FifthOtpDigit { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    public partial string SixthOtpDigit { get; set; }

    public string Otp => $"{FirstOtpDigit}{SecondOtpDigit}{ThirdOtpDigit}{FourthOtpDigit}{FifthOtpDigit}{SixthOtpDigit}";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetPasswordCommand))]
    public partial string NewPassword { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetPasswordCommand))]
    public partial string ConfirmPassword { get; set; }

    private bool CanLogin() => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !IsBusy && IsLoggingIn;

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;
        try
        {
            var loginRequest = new LoginRequest
            {
                UserName = UserName,
                Password = Password
            };

            var loginResponse = await _authService.LoginAsync(loginRequest);
            if (loginResponse is null)
            {
                ErrorMessage = "Invalid username or password. Please try again.";
            }
            else
            {
                _windowService.ShowMainWindow();
            }
        }
        catch
        {
            ErrorMessage = "An error occurred during login. Please check your network and try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ForgotPassword()
    {
        IsLoggingIn = false;
        ErrorMessage = string.Empty;
        IsRequestForgotPassword = true;
        UserName = string.Empty;
        Password = string.Empty;
    }

    [RelayCommand]
    private void CancelForgotPassword()
    {
        IsLoggingIn = true;
        ErrorMessage = string.Empty;
        IsRequestForgotPassword = false;
        IsVerifyPassword = false;
        IsResetPassword = false;
        UserName = string.Empty;
    }

    private bool CanRequestForgotPasswordOtp() => !string.IsNullOrWhiteSpace(UserName) && !IsBusy && IsRequestForgotPassword;
    [RelayCommand(CanExecute = nameof(CanRequestForgotPasswordOtp))]
    private async Task RequestForgotPasswordOtpAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;
        try
        {
            var response = await _authService.RequestForgotPasswordOtpAsync(UserName);
            if (response is null)
            {
                ErrorMessage = "An error occurred. Please try again.";
            }
            else
            {
                IsRequestForgotPassword = false;
                IsVerifyPassword = true;
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(response));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            ErrorMessage = "Email không hợp lệ hoặc người dùng không tồn tại";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanVerifyOtp() => !string.IsNullOrWhiteSpace(Otp) && !IsBusy && Otp.Length == 6 && IsVerifyPassword;
    [RelayCommand(CanExecute = nameof(CanVerifyOtp))]
    private async Task VerifyOtpAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;
        try
        {
            var response = await _authService.VerifyOtpAsync(UserName, Otp);
            if (response is null)
            {
                ErrorMessage = "An error occurred. Please try again.";
            }
            else
            {
                IsVerifyPassword = false;
                IsResetPassword = true;
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(response));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            ErrorMessage = "Mã OTP không chính xác";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanResetPassword() => !string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmPassword) && !IsBusy && IsResetPassword;
    [RelayCommand(CanExecute = nameof(CanResetPassword))]
    private async Task ResetPasswordAsync()
    {
        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Mật khẩu không khớp";
            return;
        }

        ErrorMessage = string.Empty;
        IsBusy = true;
        try
        {
            var response = await _authService.ResetPasswordAsync(UserName, Otp, NewPassword, ConfirmPassword);
            if (response is null)
            {
                ErrorMessage = "An error occurred. Please try again.";
            }
            else
            {
                IsResetPassword = false;
                IsLoggingIn = true;
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(response));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            ErrorMessage = "Mã OTP đã hết hạn";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
