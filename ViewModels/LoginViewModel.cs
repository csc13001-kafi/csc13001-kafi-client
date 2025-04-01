using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Contracts.Services;
using kafi.Models.Authentication;
namespace kafi.ViewModels;

public partial class LoginViewModel(IAuthService authService, IWindowService windowService) : ObservableObject
{
    private readonly IAuthService _authService = authService;
    private readonly IWindowService _windowService = windowService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    [NotifyCanExecuteChangedFor(nameof(RequestForgotPasswordOtpCommand))]
    private string userName;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string password;
    [ObservableProperty]
    private string errorMessage;
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RequestForgotPasswordOtpCommand))]
    private bool isForgotPassword = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    private bool isVerifyPassword = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetPasswordCommand))]
    private bool isResetPassword = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    private string firstOtpDigit;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    private string secondOtpDigit;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    private string thirdOtpDigit;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    private string fourthOtpDigit;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    private string fifthOtpDigit;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Otp))]
    [NotifyCanExecuteChangedFor(nameof(VerifyOtpCommand))]
    private string sixthOtpDigit;

    public string Otp => $"{firstOtpDigit}{secondOtpDigit}{thirdOtpDigit}{fourthOtpDigit}{fifthOtpDigit}{sixthOtpDigit}";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetPasswordCommand))]
    private string newPassword;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetPasswordCommand))]
    private string confirmPassword;

    private bool CanLogin() => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !IsBusy;

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
        catch (Exception ex)
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
        IsForgotPassword = true;
        UserName = string.Empty;
        Password = string.Empty;
    }

    [RelayCommand]
    private void CancelForgotPassword()
    {
        IsForgotPassword = false;
        UserName = string.Empty;
    }

    private bool CanRequestForgotPasswordOtp() => !string.IsNullOrWhiteSpace(UserName) && !IsBusy && IsForgotPassword;
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
                IsVerifyPassword = true;
                IsForgotPassword = false;
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(response));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred. Please check your network and try again.";
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
                IsResetPassword = true;
                IsVerifyPassword = false;
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(response));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred. Please check your network and try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanResetPassword() => !string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmPassword) && !IsBusy && NewPassword == ConfirmPassword && IsResetPassword;
    [RelayCommand(CanExecute = nameof(CanResetPassword))]
    private async Task ResetPasswordAsync()
    {
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
                WeakReferenceMessenger.Default.Send(new ValueChangedMessage<string>(response));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred. Please check your network and try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
