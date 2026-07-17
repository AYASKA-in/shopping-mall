using ShoppingMall.Client.ViewModels;
using System.Windows;

namespace ShoppingMall.Client;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
