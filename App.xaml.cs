using System;
using kafi.Contracts.Services;
using kafi.Data;
using kafi.Repositories;
using kafi.Services;
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
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        ConfigureServices();
    }

    private static void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Windows
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();

        // HttpClient
        services.AddTransient<AuthMessageHandler>();
        services.AddHttpClient("Common", client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddHttpMessageHandler<AuthMessageHandler>();

        // Services
        services.AddSingleton<ISecureTokenStorage, SecureTokenStorage>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IActivationService, ActivationService>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IAuthService, AuthService>();

        // Repositories and Daos
        services.AddSingleton<IEmployeeDao, RestEmployeeDao>();
        services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
        services.AddSingleton<ICategoryDao, RestCategoryDao>();
        services.AddSingleton<IProductDao, RestProductDao>();
        services.AddSingleton<IMenuDao, RestMenuDao>();
        services.AddSingleton<IMenuRepository, MenuRepository>();
        services.AddSingleton<IOrderDao, RestOrderDao>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IInventoryDao, RestInventoryDao>();
        services.AddSingleton<IInventoryRepository, InventoryRepository>();
        services.AddSingleton<IInfoDao, RestInfoDao>();
        services.AddSingleton<IInfoRepository, InfoRepository>();

        // Viewmodels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ShellViewModel>();
        services.AddTransient<InfoViewModel>();
        services.AddTransient<MenuViewModel>();
        services.AddTransient<EmployeeViewModel>();
        services.AddTransient<OrderViewModel>();
        services.AddTransient<InventoryViewModel>();

        Services = services.BuildServiceProvider();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var activationService = Services.GetRequiredService<IActivationService>();
        await activationService.ActivateAsync(args);
    }
}
