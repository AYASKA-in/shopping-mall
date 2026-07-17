using ShoppingMall.Client.ViewModels;
using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class CustomerView : UserControl
{
    public CustomerView(CustomerViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
