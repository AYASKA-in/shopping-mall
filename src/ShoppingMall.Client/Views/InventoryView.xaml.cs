using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class InventoryView : UserControl
{
    public InventoryView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ViewModels.InventoryViewModel vm)
                await vm.LoadStockAsync();
        };
    }
}
