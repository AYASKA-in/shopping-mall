using ShoppingMall.Client.ViewModels;
using System.Windows;

namespace ShoppingMall.Client;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Closing += OnClosing;
    }

    private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var vm = DataContext as MainViewModel;
        if (vm == null) return;

        var posVm = vm.PosVM;
        if (posVm.CurrentTransaction != null || posVm.HasItems)
        {
            var result = MessageBox.Show(
                "An active transaction is in progress. Exit anyway?",
                "Confirm Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                e.Cancel = true;
        }
    }
}
