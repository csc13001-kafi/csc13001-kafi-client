using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using kafi.ViewModels;
using kafi.Models;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MenuPage : Page
{
    public MenuViewModel ViewModel { get; }

    public MenuPage()
    {
        ViewModel = App.Services.GetService(typeof(MenuViewModel)) as MenuViewModel;
        this.InitializeComponent();
        Loaded += MenuPage_Loaded;
    }

    private async void MenuPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadDataCommand.ExecuteAsync(null);
    }

    private void Category_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs e)
    {
        if (sender.SelectedItems.Count > 0 && sender.SelectedItems[0] is Category category)
        {
            ViewModel.FilterByCategoryCommand.Execute(category);
        }
    }
}
