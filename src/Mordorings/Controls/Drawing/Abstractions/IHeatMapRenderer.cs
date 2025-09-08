namespace Mordorings.Controls;

public interface IHeatMapRenderer : IMapRendererBase
{
    void Render(MonsterHeatMapFloor? floor);
}
