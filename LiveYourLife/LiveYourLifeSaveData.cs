using LiveYourLife.AddressMoves;

namespace LiveYourLife;

public record LiveYourLifeSaveData
{
    public static LiveYourLifeSaveData Instance
    {
        get => LiveYourLifePlugin.SaveDataManager.SaveData;
        set => LiveYourLifePlugin.SaveDataManager.SaveData = value;
    }

    public AddressMovesSaveData AddressMoves { get; set; } = new();
}