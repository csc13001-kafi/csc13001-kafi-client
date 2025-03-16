using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Views;
using Microsoft.UI.Xaml.Controls;

namespace kafi.Service
{
    public class ActivationService : IActivationService
    {
        private readonly ISecureTokenStorage _tokenStorage;
        private readonly INavigationService _navigationService;

        public ActivationService(ISecureTokenStorage tokenStorage, INavigationService navigationService)
        {
            _tokenStorage = tokenStorage;
            _navigationService = navigationService;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            var tokens = _tokenStorage.GetTokens();

            if (string.IsNullOrEmpty(tokens.accessToken))
            {
                var loginWindow = new LoginWindow();
                loginWindow.Activate();
            }
            else
            {
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
            }

            await Task.CompletedTask;
        }
    }
}
