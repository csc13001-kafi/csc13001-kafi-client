using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kafi.Contracts.Services;
using kafi.Models.Authentication;
namespace kafi.ViewModels;

public partial class LoginViewModel(IAuthService authRepository, ISecureTokenStorage tokenStorage, IWindowService windowService, INavigationService navigationService) : ObservableObject
{
    private readonly IAuthService _authRepository = authRepository;
    private readonly ISecureTokenStorage _tokenStorage = tokenStorage;
    private readonly IWindowService _windowService = windowService;
    private readonly INavigationService _navigationService = navigationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string userName;
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string password;
    [ObservableProperty]
    private string errorMessage;
    [ObservableProperty]
    private bool isBusy;

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
                username = UserName,
                password = Password
            };

            var loginResponse = await _authRepository.LoginAsync(loginRequest);
            if (loginResponse is null)
            {
                ErrorMessage = "Invalid username or password. Please try again.";
            }
            else
            {
                _tokenStorage.SaveTokens(loginResponse.accessToken, loginResponse.refreshToken);
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
}
