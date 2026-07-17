using ShoppingMall.Client.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        PinBox.PasswordChanged += (s, e) =>
        {
            if (DataContext is LoginViewModel vm)
                vm.Pin = PinBox.Password;
        };
    }
}
