namespace Mordorings.Modules;

public partial class DungeonStateModel : ObservableValidator
{
    public DungeonStateModel(Monster selectedMonster)
    {
        SelectedMonster = selectedMonster;
    }

    [ObservableProperty]
    private Monster _selectedMonster;

    [ObservableProperty]
    [Required]
    [Range(1, short.MaxValue, ErrorMessage = "Current Hits must be between 1 and 32,767")]
    private short? _currentHits;

    [ObservableProperty]
    [Required]
    [Range(1, short.MaxValue, ErrorMessage = "Max Hits must be between 1 and 32,767")]
    private short? _maxHits;

    [ObservableProperty]
    [Required]
    [Range(-19, short.MaxValue, ErrorMessage = "Attack must be between -19 and 32,767")]
    private short? _attack;

    [ObservableProperty]
    [Required]
    [Range(1, short.MaxValue, ErrorMessage = "Defense must be between 1 and 32,767")]
    private short? _defense;

    [ObservableProperty]
    [Required]
    [Range(1, 50, ErrorMessage = "Group Size must be between 1 and 50")]
    private short? _groupSize;

    [ObservableProperty]
    private bool _friendly;

    [ObservableProperty]
    [Range(0, 4, ErrorMessage = "Number to Join must be between 0 and 4")]
    private short? _numToJoin;

    [ObservableProperty]
    private MonsterAlignment _alignment;

    [ObservableProperty]
    private LockedState _lockedType = LockedState.NotLocked;

    [ObservableProperty]
    private ChestType _chestType = ChestType.Box;

    [ObservableProperty]
    private TrapType _trapType = TrapType.None;

    [ObservableProperty]
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Gold must be between 1 and 2,147,483,647")]
    private int _gold;

    public IEnumerable<MonsterAlignment> Alignments { get; } = Enum.GetValues<MonsterAlignment>();

    public IEnumerable<LockedState> LockedTypes { get; } = Enum.GetValues<LockedState>();

    public IEnumerable<ChestType> ChestTypes { get; } = Enum.GetValues<ChestType>().Where(chestType => chestType != ChestType.None);

    public IEnumerable<TrapType> TrapTypes { get; } = Enum.GetValues<TrapType>();

    private List<ValidationResult> _errors = [];

    public bool IsValid()
    {
        ValidateAllProperties();
        _errors = GetErrors().ToList();
        DoManualValidation();
        return _errors.Count == 0;
    }

    private void DoManualValidation()
    {
        if (NumToJoin > GroupSize)
            _errors.Add(new ValidationResult("Number to Join must be less than or equal to Group Size"));
    }

    public IEnumerable<ValidationResult> GetAllErrors() => _errors.ToList();

    partial void OnSelectedMonsterChanged(Monster value)
    {
        Alignment = (MonsterAlignment)value.Alignment;
        Attack = value.Attack;
        Defense = value.Defense;
        CurrentHits = value.Hits;
        MaxHits = value.Hits;
        GroupSize = value.GroupSize;
        Gold = MonsterLooting.CalculateGoldForMonster(value, value.GroupSize);
    }

    partial void OnGroupSizeChanged(short? value)
    {
        if (GroupSize == null)
            return;
        Gold = MonsterLooting.CalculateGoldForMonster(SelectedMonster, GroupSize.Value);
        if (NumToJoin != null && NumToJoin > GroupSize)
        {
            NumToJoin = value;
        }
    }

    partial void OnFriendlyChanged(bool value)
    {
        if (value)
        {
            NumToJoin ??= 0;
        }
        else
        {
            NumToJoin = null;
        }
    }
}
