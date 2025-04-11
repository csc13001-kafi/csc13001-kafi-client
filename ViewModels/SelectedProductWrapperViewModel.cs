using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using kafi.Models;

namespace kafi.ViewModels;

public partial class SelectedProductWrapperViewModel(SelectedProduct selectedProduct) : ObservableObject
{
    private SelectedProduct Model { get; } = selectedProduct;
    public Product Product => Model.Product;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    public partial int Quantity { get; set; } = selectedProduct.Quantity;

    public int TotalPrice => Product.Price * Quantity;

    [ObservableProperty]
    public partial int OrderNumber { get; set; }
}

public partial class GroupedSelectedProducts(string categoryName, IEnumerable<SelectedProductWrapperViewModel> products) : ObservableCollection<SelectedProductWrapperViewModel>(products)
{
    public string CategoryName { get; set; } = categoryName;
}
