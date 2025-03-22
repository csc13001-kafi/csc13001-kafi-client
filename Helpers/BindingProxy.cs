using Microsoft.UI.Xaml;

namespace kafi.Helpers
{
    public class BindingProxy : DependencyObject
    {
        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}
