using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RetailPOS.Persistence.Contexts;
using RetailPOS.Persistence.DependencyInjection;

namespace RetailPOS.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddPersistence(options =>
                    options.UseSqlite("Data Source=retailpos.db"));

                // ViewModels
                services.AddTransient<RetailPOS.WPF.ViewModels.ProductsViewModel>();
                services.AddSingleton<RetailPOS.WPF.ViewModels.MainWindowViewModel>();

                // Views / Main window
                services.AddSingleton<MainWindow>();
                services.AddTransient<RetailPOS.WPF.Views.ProductsView>();
            })
            .Build();

        await _host.StartAsync();

        using (var scope = _host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RetailPosDbContext>();
            dbContext.Database.Migrate();
        }

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}

