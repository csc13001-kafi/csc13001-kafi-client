using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using kafi.Contracts.Services;
using kafi.Models;
using Microsoft.UI.Xaml.Media.Imaging;

namespace kafi.ViewModels;

public partial class ShellViewModel : ObservableObject, IRecipient<ValueChangedMessage<string>>, IRecipient<ValueChangedMessage<User>>
{
    private readonly IAuthService _authService;
    private readonly IWindowService _windowService;

    public string Username => _authService.CurrentUser?.Name ?? "Unknown User";
    public BitmapImage? ProfileImage
    {
        get
        {
            var imageUrl = _authService.CurrentUser?.Image;
            return imageUrl != null ? new BitmapImage(new Uri(imageUrl)) : null;
        }
    }

    public bool IsManager => _authService.IsInRole(Role.Manager);
    public bool IsEmployee => _authService.IsInRole(Role.Employee);
    public ObservableCollection<NavItem> NavItems;
    public ObservableCollection<NavItem> FooterItems;

    [ObservableProperty]
    public partial bool IsOverlayVisible { get; set; } = false;

    public ShellViewModel(IAuthService authService, IWindowService windowService, ISecureTokenStorage secureTokenStorage)
    {
        _authService = authService;
        _windowService = windowService;

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
            NavItems.Add(new() { Icon = "/Assets/NavInfoIcon.svg", Content = "Thông tin", Tag = "InfoPage" });
        }


        WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>>(this);
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<User>>(this);
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var message = await _authService.LogoutAsync();
        _windowService.ShowLoginWindow();
    }

    public void Receive(ValueChangedMessage<string> message)
    {
        if (message.Value == "showoverlay")
        {
            IsOverlayVisible = true;
        }
        else if (message.Value == "hideoverlay")
        {
            IsOverlayVisible = false;
        }
    }

    public void Receive(ValueChangedMessage<User> message)
    {
        if (message.Value != null)
        {
            OnPropertyChanged(nameof(Username));
            OnPropertyChanged(nameof(ProfileImage));
        }
    }
}
