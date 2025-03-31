using Microsoft.UI.Xaml;

namespace kafi.Contracts.Services
{
    public interface IWindowService
    {
        void ShowMainWindow();
        void ShowLoginWindow();
        Window GetCurrentWindow();
    }
}
