using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingMall.Client.Services;
using ShoppingMall.Client.ViewModels;
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
        services.AddTransient<LoginViewModel>();
        services.AddTransient<PosViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<MainWindow>();

        Services = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
