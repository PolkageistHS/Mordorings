using Mordorings.Controls;

namespace Mordorings.Factories;

public class MapRenderFactory : IMapRenderFactory
{
    public IAutomapRenderer CreateAutomapRenderer() => new AutomapRenderer();
}
