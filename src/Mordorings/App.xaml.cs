using System.Windows;
using Config.Net;
using Microsoft.Extensions.Hosting;
using Mordorings.ViewModels;
using Mordorings.Windows;

namespace Mordorings;

// ReSharper disable AsyncVoidEventHandlerMethod
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
                                                               .AddMediators()
                                                               .AddOtherServices()
                    )
                    .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        _host.Services.GetRequiredService<MainWindow>().Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
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
                                                                                                .AddTransient<LevelRequirementsViewModel>()
                                                                                                .AddTransient<RaceGuildGraphViewModel>()
                                                                                                .AddTransient<GuildSpellsViewModel>()
                                                                                                .AddTransient<DungeonStateViewModel>()
                                                                                                .AddTransient<EditMapViewModel>()
                                                                                                .AddTransient<MonsterHeatMapViewModel>();

    public static IServiceCollection AddFactories(this IServiceCollection services) => services.AddSingleton<IViewModelFactory, ViewModelFactory>()
                                                                                               .AddSingleton<IDialogFactory, DialogFactory>()
                                                                                               .AddTransient<IMordorIoFactory, MordorIoFactory>()
                                                                                               .AddTransient<IMapRendererFactory, MapRendererFactory>()
                                                                                               .AddTransient<IAutomapRenderer, AutomapRenderer>()
                                                                                               .AddTransient<IHeatMapRenderer, HeatMapRenderer>();

    public static IServiceCollection AddMediators(this IServiceCollection services) => services.AddTransient<IMonsterHeatMapMediator, MonsterHeatMapPresenter>()
                                                                                               .AddTransient<IEditMapMediator, EditMapPresenter>()
                                                                                               .AddTransient<IDungeonStateMediator, DungeonStatePresenter>();

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
