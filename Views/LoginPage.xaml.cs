using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI;
using kafi.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace kafi.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; }
    public LoginPage()
    {
        this.InitializeComponent();
        ViewModel = (LoginViewModel)App.Services.GetService(typeof(LoginViewModel))!;
        WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>>(this, (r, m) => OnValueChanged(m.Value));
        this.Unloaded += (s, e) => WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private void OnValueChanged(string value)
    {
        if (value == "Password recovery email has been sent successfully")
        {
            OtpInput.Focus(FocusState.Programmatic);
            Title.Text = "Xác Minh OTP";
        }
        else if (value == "OTP has been verified successfully")
        {
            AuthInput.Focus(FocusState.Programmatic);
            Title.Text = "Đặt Lại Mật Khẩu";
        }
        else if (value == "Password has been reset successfully")
        {
            Title.Text = "Đăng Nhập";
        }
    }

    private void Button_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is Button button)
        {
            var buttonPanel = button.Content as StackPanel;
            var buttonText = buttonPanel.FindChild<TextBlock>();
            var buttonIcon = buttonPanel.FindChild<FontIcon>();
            if (buttonText != null)
            {
                buttonText.Foreground = new SolidColorBrush(Colors.Black);
                buttonIcon.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
    }

    private void Button_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (sender is Button button)
        {
            var buttonPanel = button.Content as StackPanel;
            var buttonText = buttonPanel.FindChild<TextBlock>();
            var buttonIcon = buttonPanel.FindChild<FontIcon>();
            if (buttonText != null)
            {
                buttonText.Foreground = new SolidColorBrush(Colors.White);
                buttonIcon.Foreground = new SolidColorBrush(Colors.White);
            }
        }
    }

    private void CancelForgotPasswordButton_Click(object sender, RoutedEventArgs e)
    {
        Title.Text = "Đăng Nhập";
    }

    private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
    {
        Title.Text = "Quên Mật Khẩu";
    }

    private void OTPBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var currentBox = sender as TextBox;
        if (currentBox?.Text.Length == 1)
        {
            MoveFocusToNextBox(currentBox);
        }
        else if (currentBox?.Text.Length == 0)
        {
            MoveFocusToPreviousBox(currentBox);
        }

        if (currentBox?.Text.Length > 1)
        {
            HandlePaste(currentBox.Text);
        }
    }

    private void OTPBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Back &&
            sender is TextBox currentBox &&
            string.IsNullOrEmpty(currentBox.Text))
        {
            MoveFocusToPreviousBox(currentBox);
        }
    }

    private void MoveFocusToNextBox(TextBox currentBox)
    {
        var nextBox = GetNextTextBox(currentBox);
        nextBox?.Focus(FocusState.Programmatic);
    }
    private TextBox GetNextTextBox(TextBox currentBox)
    {
        List<TextBox> textBoxes = [otpBox1, otpBox2, otpBox3, otpBox4, otpBox5, otpBox6];
        int currentIndex = textBoxes.IndexOf(currentBox);
        if (currentIndex >= 0 && currentIndex < textBoxes.Count - 1)
        {
            return textBoxes[currentIndex + 1];
        }
        return null;
    }

    private void MoveFocusToPreviousBox(TextBox currentBox)
    {
        var previousBox = GetPreviousBox(currentBox);
        previousBox?.Focus(FocusState.Programmatic);
    }

    private TextBox GetPreviousBox(TextBox currentBox)
    {
        List<TextBox> textBoxes = [otpBox1, otpBox2, otpBox3, otpBox4, otpBox5, otpBox6];
        int currentIndex = textBoxes.IndexOf(currentBox);
        if (currentIndex > 0 && currentIndex < textBoxes.Count)
        {
            return textBoxes[currentIndex - 1];
        }
        return null;
    }

    private void HandlePaste(string pastedText)
    {
        var boxes = new[] { otpBox1, otpBox2, otpBox3, otpBox4, otpBox5, otpBox6 };

        if (pastedText.Length == boxes.Length)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].Text = pastedText[i].ToString();
            }
            boxes[^1].Focus(FocusState.Programmatic);
        }
    }

    public string GetOTP()
    {
        return otpBox1.Text + otpBox2.Text + otpBox3.Text + otpBox4.Text + otpBox5.Text + otpBox6.Text;
    }
}