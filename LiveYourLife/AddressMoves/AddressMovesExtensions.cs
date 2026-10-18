using System.Collections.Generic;
using System.Linq;
using LiveYourLife.Common;
using SOD.Common.Extensions;

namespace LiveYourLife.AddressMoves;

internal static class AddressMovesExtensions
{
    public static IEnumerable<NewAddress> WhereIsReadyForNewTenants(this IEnumerable<NewAddress> addresses, AddressMovesConfig config, AddressMovesSaveData saveData)
    {
        if (saveData.LastMove != null &&
            saveData.LastMove.Value + config.TimeBetweenMovesInHours > SessionData.Instance.gameTime)
        {
            // this is the easiest case: the last move was too recent
            AddressMovesFeature.Logger.LogInfo($"-> the last move was too recent (wait until {(saveData.LastMove.Value + config.TimeBetweenMovesInHours).AsGameTimeString()})");
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
        var shouldBeOnSaleAfter = onSaleSince + shouldBeOnSaleForHours;
        
        AddressMovesFeature.Logger.LogInfo($"- {newAddress.name} can be moved into after {shouldBeOnSaleAfter.AsGameTimeString()}");
        return shouldBeOnSaleAfter <= SessionData.Instance.gameTime;
    }

    internal static float GetOnSaleSince(this NewAddress newAddress, AddressMovesSaveData saveData)
    {
        var addressSale = saveData.AddressSales.SingleOrDefault(a => a.AddressName == newAddress.name);
        if (addressSale != null)
        {
            return addressSale.Since;
        }

        // this address must be new
        addressSale = saveData.AddInitialData(newAddress.name);
        LiveYourLifePlugin.Logger.LogInfo($"{newAddress.name} is empty since {addressSale.Since.AsGameTimeString()}");
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
        var addressOnSaleSince = initial ? 0f : SessionData.Instance.gameTime;
        return new AddressSaleSaveData(addressName, addressOnSaleSince);
    }
}