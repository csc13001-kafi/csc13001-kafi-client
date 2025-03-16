using System.Collections.ObjectModel;
using kafi.Models;

namespace kafi.ViewModels;

public partial class MenuViewModel
{
    public ObservableCollection<Category> Categories
    {
        get; set;
    }
    public ObservableCollection<Product> MenuItems
    {
        get; set;
    }

    public MenuViewModel()
    {
        Categories = new ObservableCollection<Category>
        {
            new () { Name = "Đá Xay", Image = "/Assets/Cup.png" },
            new () { Name = "Trà", Image = "/Assets/Cup.png" },
            new () { Name = "Matcha", Image = "/Assets/Cup.png" },
            new () { Name = "Cà Phê", Image = "/Assets/Cup.png" },
            new () { Name = "Bánh Ngọt", Image = "/Assets/Cup.png" }
        };

        MenuItems = new ObservableCollection<Product>
        {
            new () { Name = "Trà sen nóng", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà vải", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen đá", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà dâu", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà đào cam sả", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen nóng", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà vải", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen đá", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà dâu", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà đào cam sả", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen nóng", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà vải", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen đá", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà dâu", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà đào cam sả", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen nóng", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà vải", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà sen đá", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà dâu", Price = 40, Image = "/Assets/TraSenNong.png" },
            new () { Name = "Trà đào cam sả", Price = 40, Image = "/Assets/TraSenNong.png" },
        };

    }
}
