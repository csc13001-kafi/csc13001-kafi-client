using System;
using kafi.Contracts;
using kafi.Service;
using kafi.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    private static Window _mainWindow;
    public static Window MainWindow => _mainWindow;
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISecureTokenStorage, SecureTokenStorage>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IActivationService, ActivationService>();

        services.AddTransient<AuthMessageHandler>();
        services.AddHttpClient<IAuthRepository, AuthRepository>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<AuthMessageHandler>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<ShellViewModel>();

        Services = services.BuildServiceProvider();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _mainWindow = new MainWindow();
        Services.GetRequiredService<ISecureTokenStorage>().ClearTokens();
        var activationService = Services.GetRequiredService<IActivationService>();
        await activationService.ActivateAsync(args);
    }
}

