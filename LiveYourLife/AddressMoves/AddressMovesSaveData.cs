using System.Collections.Generic;
using Time = SOD.Common.Helpers.Time;

namespace LiveYourLife.AddressMoves;

public record AddressMovesSaveData
{
    public static AddressMovesSaveData Instance
    {
        get => LiveYourLifeSaveData.Instance.AddressMoves;
        set => LiveYourLifeSaveData.Instance.AddressMoves = value;
    }
    
    public IList<AddressSaleSaveData> AddressSales { get; set; } = [];
    public Time.TimeData? LastMove { get; set; }
}

public record AddressSaleSaveData(int AddressId, Time.TimeData Since);