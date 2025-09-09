namespace Mordorings.Modules;

public class EditMapPresenter(IMordorIoFactory ioFactory, IMapRendererFactory mapRendererFactory) : IEditMapMediator
{
    private const string SpriteSheetFile = "Assets/DungeonSprites.bmp";

    private readonly MordorRecordWriter _writer = ioFactory.GetWriter();
    private readonly DATA11DungeonMap _map = ioFactory.GetReader().GetMordorRecord<DATA11DungeonMap>();

    public DungeonFloor[] DungeonFloors { get; private set; } = [];

    public IAutomapRenderer[] Renderers { get; private set; } = [];

    public void Initialize()
    {
        int floorCount = _map.Floors.Length;
        DungeonFloors = new DungeonFloor[floorCount];
        Renderers = new IAutomapRenderer[floorCount];

        Floor[] floors = _map.Floors;
        for (int i = 0; i < floors.Length; i++)
        {
            var dungeonFloor = new DungeonFloor(floors[i]);
            DungeonFloors[i] = dungeonFloor;
            IAutomapRenderer renderer = mapRendererFactory.CreateAutomapRenderer();
            renderer.LoadSpriteSheet(SpriteSheetFile);
            renderer.Initialize(dungeonFloor);
            renderer.DrawDungeonFloorMap();
            Renderers[i] = renderer;
        }
    }

    public void ResetFloor(int floorNum)
    {
        DungeonFloors[floorNum - 1] = new DungeonFloor(_map.Floors[floorNum - 1]);
    }

    public void SaveAll()
    {
        for (int i = 0; i < DungeonFloors.Length; i++)
        {
            _map.Floors[i] = DungeonFloors[i].Floor;
        }
        _writer.WriteMordorRecord(_map);
    }
}
