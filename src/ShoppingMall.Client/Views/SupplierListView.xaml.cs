using ShoppingMall.Client.ViewModels;
using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class SupplierListView : UserControl
{
    public SupplierListView(SupplierListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.LoadAsync();
    }
}
