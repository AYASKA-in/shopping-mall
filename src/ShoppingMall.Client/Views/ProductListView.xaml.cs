using ShoppingMall.Client.ViewModels;
using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class ProductListView : UserControl
{
    public ProductListView(ProductListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.LoadAsync();
    }
}
