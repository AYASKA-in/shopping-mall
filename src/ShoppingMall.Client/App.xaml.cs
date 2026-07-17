using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingMall.Client.Services;
using ShoppingMall.Client.ViewModels;
using ShoppingMall.Client.Views;
using System.Windows;

namespace ShoppingMall.Client;

public partial class App : Application
{
    public IServiceProvider Services { get; }

    public App()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<AppConfiguration>();
        services.AddHttpClient<ApiClient>();
        services.AddSingleton<Offline.OfflineCache>();
        services.AddSingleton<CartService>();
        services.AddSingleton<ThermalPrinterService>();
        services.AddSingleton<BackgroundSyncService>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<SetupWizardViewModel>();
        services.AddTransient<PosViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<MainWindow>();

        Services = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var config = Services.GetRequiredService<AppConfiguration>();
        var cfg = config.Load();

        if (!cfg.IsConfigured)
        {
            var wizard = Services.GetRequiredService<SetupWizardViewModel>();
            var window = new Window
            {
                Title = "Shopping Mall POS - Setup",
                Width = 560,
                Height = 520,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Content = new SetupWizardView { DataContext = wizard }
            };

            wizard.SetupCompleted += (_, _) =>
            {
                window.Close();
                OpenMainWindow();
            };

            window.ShowDialog();
        }
        else
        {
            OpenMainWindow();
        }
    }

    private void OpenMainWindow()
    {
        var syncService = Services.GetRequiredService<BackgroundSyncService>();
        syncService.Start();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
