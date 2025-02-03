using System.Collections.ObjectModel;

namespace kafi.ViewModels;

public class NavItem
{
    public string Icon
    {
        get; set;
    }
    public string Content
    {
        get; set;
    }
    public string Tag
    {
        get; set;
    }
}

public partial class ShellViewModel
{
    public ObservableCollection<NavItem> NavItems
    {
        get; set;
    }

    public ShellViewModel()
    {
        NavItems = new ObservableCollection<NavItem>
        {
            new() { Icon = "/Assets/IconDashboard.svg", Content = "Tổng quan", Tag = "MainPage" },
            new() { Icon = "/Assets/IconFoodMenu.svg", Content = "Menu", Tag = "MenuPage" },
            new() { Icon = "/Assets/IconTeaTime.svg", Content = "Bàn", Tag = "TablePage" },
            new() { Icon = "/Assets/IconNotepad.svg", Content = "Đơn hàng", Tag = "OrderPage" },
            new() { Icon = "/Assets/IconSetting.svg", Content = "Thông tin", Tag = "InfoPage" },
            new() { Icon = "/Assets/IconLogOut.svg", Content = "Đăng xuất", Tag = "LogOut" },
        };
    }
}
