using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using kafi.Contracts;
using kafi.Messages;
using kafi.Models.Authentication;
using kafi.Views;
using Microsoft.UI.Xaml.Controls;
namespace kafi.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthRepository _authRepository;
    private readonly ISecureTokenStorage _tokenStorage;
    private readonly INavigationService _navigationService;

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

    public LoginViewModel(IAuthRepository authRepository, ISecureTokenStorage tokenStorage, INavigationService navigationService)
    {
        _authRepository = authRepository;
        _tokenStorage = tokenStorage;
        _navigationService = navigationService;
    }

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

                var mainWindow = new MainWindow();
                Frame frame = mainWindow.Content as Frame;
                if (frame == null)
                {
                    frame = new Frame();
                    mainWindow.Content = frame;
                }
                _navigationService.Frame = frame;
                _navigationService.NavigateTo(typeof(ShellPage));
                mainWindow.Activate();

                WeakReferenceMessenger.Default.Send(new CloseLoginWindowMessage());
            }
        }
        catch (System.Exception ex)
        {
            ErrorMessage = "An error occurred during login. Please check your network and try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
