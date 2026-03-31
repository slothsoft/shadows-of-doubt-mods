using System.Collections.Generic;
using System.Linq;
using SOD.Common;
using SOD.Common.Extensions;
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
            addresses.ForEach(a => a.GetOnSaleSince(saveData));
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

    internal static Time.TimeData GetOnSaleSince(this NewAddress newAddress, AddressMovesSaveData saveData)
    {
        var addressSale = saveData.AddressSales.SingleOrDefault(a => a.AddressName == newAddress.name);
        if (addressSale == null)
        {
            // this address must be new (?)
            addressSale = saveData.AddInitialData(newAddress.name);
            LiveYourLifePlugin.Logger.LogInfo($"{newAddress.name} is empty since {addressSale.Since.ToString(LiveYourLifePlugin.DateFormat)}");
        }

        return addressSale.Since;
    }
    
    private static AddressSaleSaveData AddInitialData(this AddressMovesSaveData saveData, string name, bool initial = false)
    {
        var addressSale = name.CreateInitialData(initial);
        saveData.AddressSales.Add(addressSale);
        return addressSale;
    }
    
    internal static AddressSaleSaveData CreateInitialData(this string addressName, bool initial = false)
    {
        // if its initial, then this plugin is new and the address is empty since the start of the game (Feb 1st 1979)
        var addressOnSaleSince = initial ? new Time.TimeData(1979, 2, 0, 0, 0) : Lib.Time.CurrentDateTime;
        return new AddressSaleSaveData(addressName, addressOnSaleSince);
    }
}