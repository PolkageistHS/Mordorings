namespace MordorDataLibrary.Models;

[DataRecordLength(StaticValues.Data11RecordLength)]
public class DATA11DungeonMap : IMordorDataFile
{
    [NewRecord]
    public short FloorCount { get; set; }

    [NewRecord]
    public short[] FloorOffsets { get; set; } = new short[15];

    public Floor[] Floors { get; set; } = new Floor[15];
}
