using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using kafi.Contracts.Services;
using kafi.Models;

namespace kafi.ViewModels;
public partial class ShellViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IWindowService _windowService;
    private readonly ISecureTokenStorage _secureTokenStorage;

    public bool IsManager => _authService.IsInRole(Role.Manager);
    public bool IsEmployee => _authService.IsInRole(Role.Employee);
    public ObservableCollection<NavItem> NavItems;
    public ObservableCollection<NavItem> FooterItems;

    public ShellViewModel(IAuthService authService, IWindowService windowService, ISecureTokenStorage secureTokenStorage)
    {
        _authService = authService;
        _windowService = windowService;
        _secureTokenStorage = secureTokenStorage;

        NavItems = [];
        FooterItems =
        [
            new() { Icon = "/Assets/NavLogoutIcon.svg", Content = "Đăng xuất", Tag = "Logout" },
        ];

        if (IsManager)
        {
            NavItems.Add(new() { Icon = "/Assets/NavMainIcon.svg", Content = "Tổng quan", Tag = "MainPage" });
            NavItems.Add(new() { Icon = "/Assets/NavMenuIcon.svg", Content = "Quản lý menu", Tag = "MenuPage" });
            NavItems.Add(new() { Icon = "/Assets/NavEmployeeIcon.svg", Content = "Quản lý nhân viên", Tag = "EmployeePage" });
            NavItems.Add(new() { Icon = "/Assets/NavOrderIcon.svg", Content = "Quản lý đơn hàng", Tag = "OrderPage" });
            NavItems.Add(new() { Icon = "/Assets/NavInventoryIcon.svg", Content = "Quản lý kho", Tag = "InventoryPage" });
            NavItems.Add(new() { Icon = "/Assets/NavInfoIcon.svg", Content = "Thông tin", Tag = "InfoPage" });
        }
        else if (IsEmployee)
        {
            NavItems.Add(new() { Icon = "/Assets/NavMainIcon.svg", Content = "Tổng quan", Tag = "MainPage" });
            NavItems.Add(new() { Icon = "/Assets/NavMenuIcon.svg", Content = "Menu", Tag = "MenuPage" });
            NavItems.Add(new() { Icon = "/Assets/NavTableIcon.svg", Content = "Bàn", Tag = "TablePage" });
            NavItems.Add(new() { Icon = "/Assets/NavOrderIcon.svg", Content = "Đơn hàng", Tag = "OrderPage" });
            NavItems.Add(new() { Icon = "/Assets/NavInventoryIcon.svg", Content = "Kho hàng", Tag = "InventoryPage" });
            FooterItems.Add(new() { Icon = "/Assets/NavInfoIcon.svg", Content = "Thông tin", Tag = "InfoPage" });
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var message = await _authService.LogoutAsync();
        _secureTokenStorage.ClearTokens();
        _windowService.ShowLoginWindow();
    }

}
