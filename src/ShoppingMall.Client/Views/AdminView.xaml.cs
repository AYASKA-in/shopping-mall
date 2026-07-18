using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class AdminView : UserControl
{
    public AdminView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ViewModels.AdminViewModel vm)
                await vm.LoadTabAsync();
        };
    }
}
