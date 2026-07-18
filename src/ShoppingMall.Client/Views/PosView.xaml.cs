using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class PosView : UserControl
{
    public PosView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ViewModels.PosViewModel vm && vm.CurrentTransaction == null)
                await vm.CreateNewTransactionAsync();
        };
    }
}
