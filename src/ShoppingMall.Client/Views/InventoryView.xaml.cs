using ShoppingMall.Client.ViewModels;

namespace ShoppingMall.Client.Views;

public partial class InventoryView
{
    public InventoryView(InventoryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
