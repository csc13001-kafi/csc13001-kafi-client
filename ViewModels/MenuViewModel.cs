using System.Collections.ObjectModel;

namespace kafi.ViewModels;

public class Category
{
    public string Name
    {
        get; set;
    }
    public string Icon
    {
        get; set;
    }
}

public class MenuItem
{
    public string Name
    {
        get; set;
    }
    public string Price
    {
        get; set;
    }
    public string Image
    {
        get; set;
    }
}
public partial class MenuViewModel
{
    public ObservableCollection<Category> Categories
    {
        get; set;
    }
    public ObservableCollection<MenuItem> MenuItems
    {
        get; set;
    }

    public MenuViewModel()
    {
        Categories = new ObservableCollection<Category>
        {
            new () { Name = "Đá Xay", Icon = "/Assets/Cup.png" },
            new () { Name = "Trà", Icon = "/Assets/Cup.png" },
            new () { Name = "Matcha", Icon = "/Assets/Cup.png" },
            new () { Name = "Cà Phê", Icon = "/Assets/Cup.png" },
            new () { Name = "Bánh Ngọt", Icon = "/Assets/Cup.png" }
        };

        MenuItems = new ObservableCollection<MenuItem>
        {
            new () { Name = "Trà sen nóng", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà vải", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen đá", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà dâu", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà đào cam sả", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen nóng", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà vải", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen đá", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà dâu", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà đào cam sả", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen nóng", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà vải", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen đá", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà dâu", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà đào cam sả", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen nóng", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà vải", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen đá", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà dâu", Price = "40.000", Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà đào cam sả", Price = "40.000", Image = "/Assets/TraSenNong.png" },
        };

    }
}
