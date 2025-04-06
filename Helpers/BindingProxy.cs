using Microsoft.UI.Xaml;

namespace kafi.Helpers;

public class BindingProxy : DependencyObject
{
    private static BindingProxy? _instance;
    public static BindingProxy Instance => _instance ??= new BindingProxy();

    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(
            "Data",
            typeof(object),
            typeof(BindingProxy),
            new PropertyMetadata(null, OnDataChanged)
        );

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }
}
