using System.Windows.Controls;

namespace ShoppingMall.Client.Views;

public partial class ReportsView : UserControl
{
    public ReportsView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ViewModels.ReportsViewModel vm)
                await vm.LoadReportAsync();
        };
    }
}
