using Microsoft.Extensions.DependencyInjection;

namespace Mordorings.Factories;

public class MapRendererFactory(IServiceProvider services) : IMapRendererFactory
{
    public IAutomapRenderer CreateAutomapRenderer() => services.GetRequiredService<IAutomapRenderer>();

    public IHeatMapRenderer CreateHeatMapRenderer() => services.GetRequiredService<IHeatMapRenderer>();
}
