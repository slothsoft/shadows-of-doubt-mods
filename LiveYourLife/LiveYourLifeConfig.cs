using BepInEx.Configuration;
using LiveYourLife.AddressMoves;

namespace LiveYourLife;

public record LiveYourLifeConfig
{
    public static LiveYourLifeConfig Instance
    {
        get
        {
            field ??= new LiveYourLifeConfig();
            return field;
        }
    }

    public AddressMovesConfig  AddressMoves { get; set; } = new();
    
    internal void Register(ConfigFile config)
    {
        AddressMoves.Register(config);
    }
}