using System.Threading.Tasks;
using kafi.Contracts;
using kafi.Views;
using Microsoft.UI.Xaml.Controls;

namespace kafi.Service
{
    public class ActivationService : IActivationService
    {
        private readonly INavigationService _navigationService;
        private readonly ISecureTokenStorage _tokenStorage;

        public ActivationService(INavigationService navigationService, ISecureTokenStorage tokenStorage)
        {
            _navigationService = navigationService;
            _tokenStorage = tokenStorage;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            Frame frame = App.MainWindow.Content as Frame;
            if (App.MainWindow.Content == null)
            {
                frame = new Frame();
                App.MainWindow.Content = frame;
            }
            _navigationService.Frame = frame;

            var tokens = _tokenStorage.GetTokens();

            if (string.IsNullOrEmpty(tokens.accessToken))
            {
                _navigationService.NavigateTo(typeof(LoginPage));
            }
            else
            {
                _navigationService.NavigateTo(typeof(ShellPage));
            }

            App.MainWindow.Activate();
            await Task.CompletedTask;
        }
    }
}
