using System.Collections.Generic;
using System.Linq;
using SOD.Common;
using SOD.Common.Helpers;

namespace LiveYourLife.AddressMoves;

internal static class AddressMovesExtensions
{
    public static IEnumerable<NewAddress> WhereIsReadyForNewTenants(this IEnumerable<NewAddress> addresses, AddressMovesConfig config, AddressMovesSaveData saveData)
    {
        if (saveData.LastMove != null &&
            saveData.LastMove.Value.AddHours(config.TimeBetweenMovesInHours) > Lib.Time.CurrentDateTime)
        {
            // this is the easiest case: the last move was too recent
            AddressMovesFeature.Logger.LogInfo($"-> the last move was too recent (wait until {saveData.LastMove.Value.AddHours(config.TimeBetweenMovesInHours).ToString(LiveYourLifePlugin.DateFormat)})");
            return []; 
        }
        return addresses.Where(a => a.IsReadyForNewTenants(config, saveData));
    }
    
    private static bool IsReadyForNewTenants(this NewAddress newAddress, AddressMovesConfig config, AddressMovesSaveData saveData)
    {
        var onSaleSince = newAddress.GetOnSaleSince(saveData);
        // if min=7 and max=10, then 7, 8, 9, 10 should be valid here (after modulo = 0, 1, 2, 3)
        var shouldBeOnSaleForHours =
            newAddress.id % (config.MaxTimeUntilMoveInHours - config.MinTimeUntilMoveInHours + 1) +
            config.MinTimeUntilMoveInHours;
        var shouldBeOnSaleAfter = onSaleSince.AddHours(shouldBeOnSaleForHours);
        
        AddressMovesFeature.Logger.LogInfo($"- {newAddress.name} can be moved into after {shouldBeOnSaleAfter.ToString(LiveYourLifePlugin.DateFormat)}: is currently {shouldBeOnSaleAfter < Lib.Time.CurrentDateTime}");
        return shouldBeOnSaleAfter < Lib.Time.CurrentDateTime;
    }

    private static Time.TimeData GetOnSaleSince(this NewAddress newAddress, AddressMovesSaveData saveData)
    {
        var addressSale = saveData.AddressSales.SingleOrDefault(a => a.AddressId == newAddress.id);
        if (addressSale == null)
        {
            // this address must be new (?)
            addressSale = saveData.AddInitialData(newAddress.id);
            LiveYourLifePlugin.Logger.LogInfo($"{newAddress.name} is empty since {addressSale.Since.ToString(LiveYourLifePlugin.DateFormat)}");
        }

        return addressSale.Since;
    }
    
    internal static AddressSaleSaveData AddInitialData(this AddressMovesSaveData saveData, int id, bool initial = false)
    {
        // if its initial, then this plugin is new and the address is empty since the start of the game (Feb 1st 1979)
        var addressOnSaleSince = initial ? new Time.TimeData(1979, 2, 1, 0, 0) : Lib.Time.CurrentDateTime;
        var addressSale = new AddressSaleSaveData(id, addressOnSaleSince);
        saveData.AddressSales.Add(addressSale);
        return addressSale;
    }
}