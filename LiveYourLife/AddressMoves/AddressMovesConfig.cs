using BepInEx.Configuration;

namespace LiveYourLife.AddressMoves;

public record AddressMovesConfig
{
    public const int DefaultMinTimeUntilMoveInHours = 7 * 24;
    public const int DefaultMaxTimeUntilMoveInHours = 14 * 24;
    public const int DefaultTimeBetweenMovesInHours = 1 * 24;
    
    internal void Register(ConfigFile config)
    {
        var minTimeUntilMove = config.Bind(new ConfigDefinition(nameof(AddressMovesConfig), nameof(MinTimeUntilMoveInHours)), DefaultMinTimeUntilMoveInHours);
        minTimeUntilMove.SettingChanged += (sender, args) => MinTimeUntilMoveInHours = minTimeUntilMove.Value;
        MinTimeUntilMoveInHours = minTimeUntilMove.Value;
        
        var maxTimeUntilMove = config.Bind(new ConfigDefinition(nameof(AddressMovesConfig), nameof(MaxTimeUntilMoveInHours)), DefaultMaxTimeUntilMoveInHours);
        maxTimeUntilMove.SettingChanged += (sender, args) => MaxTimeUntilMoveInHours = maxTimeUntilMove.Value;
        MaxTimeUntilMoveInHours = maxTimeUntilMove.Value;
        
        var timeBetweenMoves = config.Bind(new ConfigDefinition(nameof(AddressMovesConfig), nameof(TimeBetweenMovesInHours)), DefaultTimeBetweenMovesInHours);
        timeBetweenMoves.SettingChanged += (sender, args) => TimeBetweenMovesInHours = timeBetweenMoves.Value;
        TimeBetweenMovesInHours = timeBetweenMoves.Value;
    }

    public int MinTimeUntilMoveInHours = DefaultMinTimeUntilMoveInHours;
    public int MaxTimeUntilMoveInHours = DefaultMaxTimeUntilMoveInHours;
    public int TimeBetweenMovesInHours = DefaultTimeBetweenMovesInHours;
}