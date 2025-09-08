namespace Mordorings.Factories;

public interface IMapRendererFactory
{
    IAutomapRenderer CreateAutomapRenderer();

    IHeatMapRenderer CreateHeatMapRenderer();
}
