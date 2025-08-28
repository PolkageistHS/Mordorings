namespace MordorDataLibrary.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SavedWindowState
{
    public int Left { get; set; }

    public int Top { get; set; }

    public int Height { get; set; }

    public int Width { get; set; }

    public short WindowState { get; set; }
}
