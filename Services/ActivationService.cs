using System.Diagnostics;
using System.Threading.Tasks;
using kafi.Contracts.Services;

namespace kafi.Service
{
    public class ActivationService(ISecureTokenStorage tokenStorage, IWindowService windowService, IAuthService authService) : IActivationService
    {
        private readonly ISecureTokenStorage _tokenStorage = tokenStorage;
        private readonly IWindowService _windowService = windowService;
        private readonly IAuthService _authService = authService;

        public async Task ActivateAsync(object activationArgs)
        {
            var tokens = _tokenStorage.GetTokens();

            if (string.IsNullOrEmpty(tokens.accessToken))
            {
                _windowService.ShowLoginWindow();
            }
            else
            {
                await _authService.LoadCurrentUserFromToken(tokens.accessToken);
                _windowService.ShowMainWindow();
            }

            await Task.CompletedTask;
        }
    }
}
