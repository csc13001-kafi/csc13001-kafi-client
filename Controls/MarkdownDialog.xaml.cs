using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace kafi.Controls;

public sealed partial class MarkdownDialog : ContentDialog
{
    public static readonly DependencyProperty MarkdownTextProperty =
        DependencyProperty.Register(nameof(MarkdownText), typeof(string), typeof(MarkdownDialog), new PropertyMetadata(string.Empty));

    public string MarkdownText
    {
        get => (string)GetValue(MarkdownTextProperty);
        set => SetValue(MarkdownTextProperty, value);
    }

    public MarkdownDialog(XamlRoot root)
    {
        this.InitializeComponent();
        this.DefaultButton = ContentDialogButton.Close;
        this.CloseButtonText = "Đóng";
        this.XamlRoot = root;
    }
}