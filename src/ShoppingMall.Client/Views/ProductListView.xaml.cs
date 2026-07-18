using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class ProductListView : UserControl
{
    public ProductListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ViewModels.ProductListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
