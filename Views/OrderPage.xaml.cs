using kafi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OrderPage : Page
{
    public OrderViewModel ViewModel
    {
        get;
    }
    public OrderPage()
    {
        ViewModel = App.Services.GetService(typeof(OrderViewModel)) as OrderViewModel;
        this.InitializeComponent();
        Loaded += OrderPage_Loaded;
    }

    private async void OrderPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
