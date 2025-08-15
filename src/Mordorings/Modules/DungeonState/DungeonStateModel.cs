using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Calculations;
using CommunityToolkit.Mvvm.ComponentModel;
using MordorDataLibrary.Models;
using ChestType = MordorDataLibrary.Models.ChestType;

namespace Mordorings.Modules.DungeonState;

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
    private string? _currentHits;

    [ObservableProperty]
    [Required]
    private string? _maxHits;

    [ObservableProperty]
    [Required]
    private string? _attack;

    [ObservableProperty]
    [Required]
    private string? _defense;

    [ObservableProperty]
    [Required]
    private string? _groupSize;

    [ObservableProperty]
    private bool _friendly;

    [ObservableProperty]
    private string? _numToJoin;

    [ObservableProperty]
    private MonsterAlignment _alignment;

    [ObservableProperty]
    private LockedState _lockedType = LockedState.NotLocked;

    [ObservableProperty]
    private ChestType _chestType = ChestType.Box;

    [ObservableProperty]
    private TrapType _trapType = TrapType.None;

    [ObservableProperty]
    private string? _gold;

    [Range(1, short.MaxValue, ErrorMessage = "Current Hits must be between 1 and 32,767")]
    public short? CurrentHitsVal => ConvertToShort(CurrentHits);

    [Range(1, short.MaxValue, ErrorMessage = "Max Hits must be between 1 and 32,767")]
    public short? MaxHitsVal => ConvertToShort(MaxHits);

    [Range(-19, short.MaxValue, ErrorMessage = "Attack must be between -19 and 32,767")]
    public short? AttackVal => ConvertToShort(Attack);

    [Range(1, short.MaxValue, ErrorMessage = "Defense must be between 1 and 32,767")]
    public short? DefenseVal => ConvertToShort(Defense);

    [Range(1, 50, ErrorMessage = "Group Size must be between 1 and 50")]
    public short? GroupSizeVal => ConvertToShort(GroupSize);

    [Range(0, 4, ErrorMessage = "Number to Join must be between 0 and 4")]
    public short? NumToJoinVal => ConvertToShort(NumToJoin);

    [Range(1, int.MaxValue, ErrorMessage = "Gold must be between 1 and 2,147,483,647")]
    public int? GoldVal => ConvertToInt(Gold);

    public IEnumerable<MonsterAlignment> Alignments { get; } = Enum.GetValues<MonsterAlignment>();

    public IEnumerable<LockedState> LockedTypes { get; } = Enum.GetValues<LockedState>();

    public IEnumerable<ChestType> ChestTypes { get; } = Enum.GetValues<ChestType>();

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
        if (NumToJoinVal > GroupSizeVal)
            _errors.Add(new ValidationResult("Number to Join must be less than or equal to Group Size"));
    }

    public IEnumerable<ValidationResult> GetAllErrors() => _errors.ToList();

    [DebuggerStepThrough]
    private static short? ConvertToShort(string? val)
    {
        if (val is null)
            return null;
        return short.TryParse(val, out short result) ? result : null;
    }

    [DebuggerStepThrough]
    private static int? ConvertToInt(string? val)
    {
        if (val is null)
            return null;
        return int.TryParse(val, out int result) ? result : null;
    }

    partial void OnSelectedMonsterChanged(Monster value)
    {
        Alignment = (MonsterAlignment)value.Alignment;
        Attack = value.Attack.ToString();
        Defense = value.Defense.ToString();
        CurrentHits = value.Hits.ToString();
        MaxHits = value.Hits.ToString();
        GroupSize = value.GroupSize.ToString();
        Gold = MonsterLooting.CalculateGoldForMonster(value, value.GroupSize).ToString();
    }

    partial void OnGroupSizeChanged(string? value)
    {
        if (GroupSizeVal == null)
            return;
        Gold = MonsterLooting.CalculateGoldForMonster(SelectedMonster, GroupSizeVal.Value).ToString();
        if (NumToJoinVal != null && NumToJoinVal > GroupSizeVal)
        {
            NumToJoin = value;
        }
    }

    partial void OnFriendlyChanged(bool value)
    {
        if (value)
        {
            NumToJoin ??= "0";
        }
        else
        {
            NumToJoin = null;
        }
    }
}
