namespace Mordorings.Modules;

public partial class MapObjects : ObservableObject
{
    public event EventHandler? ObjectsUpdated;

    [ObservableProperty]
    private bool _teleporterRandom;

    partial void OnTeleporterRandomChanged(bool value)
    {
        OnPropertyChanged(nameof(IsTeleporterFixed));
        NotifyChanged();
    }

    public bool IsTeleporterFixed => !TeleporterRandom;

    [ObservableProperty]
    private int? _teleporterX;

    partial void OnTeleporterXChanged(int? value) => NotifyChanged();

    [ObservableProperty]
    private int? _teleporterY;

    partial void OnTeleporterYChanged(int? value) => NotifyChanged();

    [ObservableProperty]
    private int? _teleporterZ;

    partial void OnTeleporterZChanged(int? value) => NotifyChanged();

    [ObservableProperty]
    private int _chuteDepth;

    partial void OnChuteDepthChanged(int value) => NotifyChanged();

    private void NotifyChanged()
    {
        ObjectsUpdated?.Invoke(this, EventArgs.Empty);
    }
}
