using System.Collections.Generic;
using System.Linq;
using kafi.Contracts.Services;
using kafi.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace kafi.Services
{
    public class WindowService(INavigationService navigationService) : IWindowService
    {
        private readonly INavigationService _navigationService = navigationService;
        private readonly List<Window> _activeWindows = new();
        private readonly object _windowsLock = new();
        public void ShowMainWindow()
        {
            MainWindow window = App.Services.GetRequiredService<MainWindow>();
            var frame = window.Content as Frame;
            if (frame == null)
            {
                frame = new Frame();
                window.Content = frame;
            }
            _navigationService.Initialize(frame);
            _navigationService.NavigateTo(typeof(ShellPage));
            ActivateWindow(window);
            CloseWindowOfType<LoginWindow>();
        }

        public void ShowLoginWindow()
        {
            LoginWindow window = App.Services.GetRequiredService<LoginWindow>();
            ActivateWindow(window);
            CloseWindowOfType<MainWindow>();
        }

        private void ActivateWindow(Window window)
        {
            lock (_windowsLock)
            {
                window.Activate();
                window.Closed += Window_Closed;
                _activeWindows.Add(window);
            }
        }

        private void CloseWindowOfType<TWindow>() where TWindow : Window
        {
            lock (_windowsLock)
            {
                foreach (var window in _activeWindows.OfType<TWindow>().ToList())
                {
                    window.Close();
                }
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            lock (_windowsLock)
            {
                if (sender is Window window)
                {
                    window.Closed -= Window_Closed;
                    _activeWindows.Remove(window);
                }
            }
        }

    }
}
