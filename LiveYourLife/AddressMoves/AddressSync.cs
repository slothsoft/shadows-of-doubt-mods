using System;
using System.Collections.Generic;
using System.Linq;
using SOD.Common.Extensions;

namespace LiveYourLife.AddressMoves;

// Sincs between the SaveData and the in-game list
internal class AddressSync(bool initital)
{
    public Action<string> Log { get; set; } = Console.WriteLine;
    
    public void Sync(AddressMovesSaveData saveData)
    {
        var addressesForSale = GameplayController.Instance.forSale.AsEnumerable()!.ToDictionary(a => a.id, a => a);
        var savedAddresses = saveData.AddressSales.ToDictionary(a => a.AddressId, a => a);

        SyncToSaveData(addressesForSale, savedAddresses, saveData);
        RemoveOrphansFromSaveData(addressesForSale, saveData);
    }
    
    private void SyncToSaveData(IDictionary<int, NewAddress> addressesForSale, IDictionary<int, AddressSaleSaveData> savedAddresses, AddressMovesSaveData saveData)
    {
        foreach (var (id, addressForSale) in addressesForSale)
        {
            if (!savedAddresses.ContainsKey(id))
            {
                var addressSale = saveData.AddInitialData(id, initital);
                LiveYourLifePlugin.Logger.LogInfo($"{addressForSale.name} is empty since {addressSale.Since.ToString(LiveYourLifePlugin.DateFormat)}");
            }   
        }
    }
    
    private void RemoveOrphansFromSaveData(IDictionary<int, NewAddress> addressesForSale, AddressMovesSaveData saveData)
    {
        foreach (var addressSaleSaveData in saveData.AddressSales.Where(a => !addressesForSale.ContainsKey(a.AddressId)))
        {
            saveData.AddressSales.Remove(addressSaleSaveData);
        }
    }
}