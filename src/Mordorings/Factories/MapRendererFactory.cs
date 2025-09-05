using Microsoft.Extensions.DependencyInjection;

namespace Mordorings.Factories;

public class MapRendererFactory(IServiceProvider services) : IMapRendererFactory
{
    public IAutomapRenderer CreateAutomapRenderer() => services.GetRequiredService<IAutomapRenderer>();

    public IHeatmapRenderer CreateHeatmapRenderer() => services.GetRequiredService<IHeatmapRenderer>();
}
