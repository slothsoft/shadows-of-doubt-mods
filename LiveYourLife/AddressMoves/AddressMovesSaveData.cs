using System.Collections.Generic;

namespace LiveYourLife.AddressMoves;

public record AddressMovesSaveData
{
    public static AddressMovesSaveData Instance
    {
        get => LiveYourLifeSaveData.Instance.AddressMoves;
        set => LiveYourLifeSaveData.Instance.AddressMoves = value;
    }
    
    public IList<AddressSaleSaveData> AddressSales { get; set; } = [];
    public float? LastMove { get; set; }
}

public record AddressSaleSaveData(string AddressName, float Since);