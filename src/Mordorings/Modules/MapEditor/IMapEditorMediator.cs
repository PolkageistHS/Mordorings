namespace Mordorings.Modules;

public interface IMapEditorMediator
{
    /// <summary>
    /// Initializes map data and pre-renders all floors.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Gets all cached dungeon floors (one per floor) used for editing.
    /// </summary>
    DungeonFloor[] DungeonFloors { get; }

    /// <summary>
    /// Gets all automap renderers (one per floor) that are initialized and drawn.
    /// </summary>
    IAutomapRenderer[] Renderers { get; }

    /// <summary>
    /// Resets a specific floor from the original map data and returns the refreshed DungeonFloor.
    /// </summary>
    void ResetFloor(int floorNum);

    /// <summary>
    /// Writes all cached floors back to the underlying map file.
    /// </summary>
    void SaveAll();
}
