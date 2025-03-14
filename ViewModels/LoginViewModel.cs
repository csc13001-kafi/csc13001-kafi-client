using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kafi.Contracts;
using kafi.Models.Authentication;
using kafi.Views;
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

    public LoginViewModel(IAuthRepository authRepository, ISecureTokenStorage tokenStorage, INavigationService navigationService)
    {
        _authRepository = authRepository;
        _tokenStorage = tokenStorage;
        _navigationService = navigationService;
    }

    private bool CanLogin() => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        var loginRequest = new LoginRequest
        {
            username = UserName,
            password = Password
        };
        var loginResponse = await _authRepository.LoginAsync(loginRequest);
        if (loginResponse is null)
        {
            ErrorMessage = "Invalid username or password";
        }
        else
        {
            _tokenStorage.SaveTokens(loginResponse.accessToken, loginResponse.refreshToken);
            _navigationService.NavigateTo(typeof(ShellPage));
        }
    }
}
