using System.Windows;
using Config.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mordorings.Configs;
using Mordorings.Factories;
using Mordorings.Modules.DungeonState;
using Mordorings.ViewModels;
using Mordorings.Windows;

namespace Mordorings;

using DungeonStateViewModel = DungeonStateViewModel;

public partial class App
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
                    .ConfigureServices(collection => collection.AddWindowServices()
                                                               .AddUserControls()
                                                               .AddViewModels()
                                                               .AddFactories()
                                                               .AddOtherServices())
                    .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            await _host.StartAsync();
            _host.Services.GetRequiredService<MainWindow>().Show();
            base.OnStartup(e);
        }
        catch
        {
            // TODO handle exception
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
        catch
        {
            // TODO handle exception
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWindowServices(this IServiceCollection services) => services.AddSingleton<MainWindow>();

    public static IServiceCollection AddUserControls(this IServiceCollection services) => services.AddScoped<DungeonStateControl>();

    public static IServiceCollection AddViewModels(this IServiceCollection services) => services.AddSingleton<MainWindowViewModel>()
                                                                                                .AddSingleton<MapMenuViewModel>()
                                                                                                .AddSingleton<EditMenuViewModel>()
                                                                                                .AddSingleton<CalculationsMenuViewModel>()
                                                                                                .AddSingleton<SimulationsMenuViewModel>()
                                                                                                .AddTransient<DungeonStateViewModel>()
                                                                                                ;

    public static IServiceCollection AddFactories(this IServiceCollection services) => services.AddSingleton<IViewModelFactory, ViewModelFactory>()
                                                                                               .AddSingleton<IDungeonStateFactory, DungeonStateFactory>()
                                                                                               .AddSingleton<IDialogFactory, DialogFactory>()
                                                                                               .AddTransient<IMordorIoFactory, MordorIoFactory>()
                                                                                               ;

    public static IServiceCollection AddOtherServices(this IServiceCollection services)
    {
        IMordoringSettings settings = new ConfigurationBuilder<IMordoringSettings>().UseJsonFile(".\\settings.json").Build();
        if (string.IsNullOrWhiteSpace(settings.DataFileFolder))
        {
            settings.DataFileFolder = @"C:\Mordor\Data";
        }
        return services.AddSingleton<IViewModelFactory, ViewModelFactory>()
                       .AddSingleton(settings)
                       ;
    }
}
