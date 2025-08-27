namespace Mordorings.Modules.MonsterHeatMap;

public partial class MonsterHeatMapViewModel : MapViewModelBase
{
    public MonsterHeatMapViewModel(IMordorIoFactory ioFactory, IMapRenderFactory mapRenderFactory) : base(ioFactory, mapRenderFactory)
    {
        SelectedFloorNum = 1;
    }

    protected override void OnSelectedFloorNumChanged() { }

    public override string Instructions => "View the heat map of monsters on the map.";
}
