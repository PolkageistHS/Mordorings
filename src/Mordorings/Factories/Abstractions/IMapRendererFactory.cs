namespace Mordorings.Factories;

public interface IMapRendererFactory
{
    IAutomapRenderer CreateAutomapRenderer();

    IHeatmapRenderer CreateHeatmapRenderer();
}
