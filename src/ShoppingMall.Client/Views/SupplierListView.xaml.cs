using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class SupplierListView : UserControl
{
    public SupplierListView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ViewModels.SupplierListViewModel vm)
                await vm.LoadAsync();
        };
    }
}
