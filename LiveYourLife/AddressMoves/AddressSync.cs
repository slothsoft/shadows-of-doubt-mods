using System;
using System.Collections.Generic;
using System.Linq;
using LiveYourLife.Common;
using SOD.Common.Extensions;

namespace LiveYourLife.AddressMoves;

// Sincs between the SaveData and the in-game list
internal class AddressSync(bool initital)
{
    public Action<string> Log { get; set; } = Console.WriteLine;
    
    public void Sync(AddressMovesSaveData saveData)
    {
        var addressesForSale = GameplayController.Instance.forSale.AsEnumerable()!.ToDictionary(a => a.name, a => a);
        var savedAddresses = saveData.AddressSales.ToDictionary(a => a.AddressName, a => a);

        SyncToSaveData(addressesForSale, savedAddresses, saveData);
        RemoveOrphansFromSaveData(addressesForSale, saveData);
        Log($"{saveData.AddressSales.Count} addresses present after sync");
    }
    
    private void SyncToSaveData(IDictionary<string, NewAddress> addressesForSale, IDictionary<string, AddressSaleSaveData> savedAddresses, AddressMovesSaveData saveData)
    {
        var additionalData = SyncToSaveData(addressesForSale, savedAddresses).ToList();
        saveData.AddressSales = saveData.AddressSales.Concat(additionalData).ToList();
        Log($"{additionalData.Count} addresses synced from the game to the save data");
    }
    
    private IEnumerable<AddressSaleSaveData> SyncToSaveData(IDictionary<string, NewAddress> addressesForSale, IDictionary<string, AddressSaleSaveData> savedAddresses)
    {
        foreach (var (name, _) in addressesForSale)
        {
            if (savedAddresses.ContainsKey(name)) continue;
            var addressSale = name.CreateInitialData(initital);
            Log($"{name} is empty since {addressSale.Since.ToString(LiveYourLifePlugin.DateFormat)}");
            yield return addressSale;
        }
    }
    
    private void RemoveOrphansFromSaveData(IDictionary<string, NewAddress> addressesForSale, AddressMovesSaveData saveData)
    {
        if (addressesForSale.Count == 0) return; // somehow addresses are sometimes empty, which shouldn't be possible
        
        var orphanedAddresses = saveData.AddressSales.Where(a => !addressesForSale.ContainsKey(a.AddressName)).ToArray();
        saveData.AddressSales.RemoveRange(orphanedAddresses);
        Log($"{orphanedAddresses.Length} orphaned addresses removed");
    }
}