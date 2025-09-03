namespace Mordorings.Controls;

public interface IHeatmapRenderer : IMapRendererBase
{
    void Render(MonsterHeatmapFloor? floor);
}
