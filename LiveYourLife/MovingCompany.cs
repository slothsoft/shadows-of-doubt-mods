using System;
using System.Linq;
using LiveYourLife.Common;
using SOD.Common.Extensions;

namespace LiveYourLife;

public class MovingCompany
{
    public Action<string> Log { get; set; } = Console.WriteLine;

    public void MoveTenantsIfApplicable()
    {
        foreach (var address in GameplayController.Instance.forSale)
        {
            var newTenants = FindNewTenants(address);
            if (newTenants != null)
            {
                MoveTenants(newTenants, address);
                break; 
            }
        }
    }

    // works nicely:
    // Logger.LogInfo($"{Name} tick");
    // foreach (var address in GameplayController.Instance.forSale)
    // {
    //    Logger.LogInfo($"{address.name}: {address.GetPrice()}");
    // }
        
    public NewAddress? FindNewTenants(NewAddress targetAddress)
    {
        // CityData.Instance.addressDirectory
        // [Port Brewery]: 1742 addresses
        Log($"{targetAddress.name}: price {targetAddress.GetPrice()}, floor {targetAddress.floor.floor}");
        var addressFloor = targetAddress.floor.floor;
        
        // [Port Brewery]: 10 ~ 40 addresses
        var addressesOnSameFloor = CityData.Instance.addressDirectory
            // same floor means same economic class 
            .Where(a => a.floor.floor == addressFloor)
            // only take addresses into account where people live
            .WhereIsResidential()
            .ToArray();
        Log($"    addresses on same floor: {addressesOnSameFloor}");

        var familyIncomesOnSameFloor = addressesOnSameFloor
            // calculate family income
            .Select(a => a.CalculateFamilyIncome())
            .Where(fi => fi > 0)
            .ToArray();

        Log($"        family incomes on same floor: {familyIncomesOnSameFloor.Length}");
        var minFamilyIncome = familyIncomesOnSameFloor.Min();
        var maxFamilyIncome = familyIncomesOnSameFloor.Max();
        Log($"        minFamilyIncome: {minFamilyIncome}");
        Log($"        maxFamilyIncome: {maxFamilyIncome}");

        // [Port Brewery]: 1742 addresses
        return CityData.Instance.addressDirectory
            .Where(a =>
            {
                var familiyIncome = a.CalculateFamilyIncome();
                return familiyIncome >= minFamilyIncome && familiyIncome <= maxFamilyIncome;
            })
            // Port Brewery: around 300 to 400 families can move
            // return only the ones that don't live on the same floor already
            .Where(f => f.floor.floor != addressFloor)
            .RandomElementOrDefault();
    }

    private void MoveTenants(NewAddress newTenants, NewAddress targetAddress)
    {
        Log($"    family moves from: {newTenants.name}");
        MoveTenantsInto(newTenants, targetAddress);
        MoveTenantsOut(newTenants);
        
        newTenants.saleNote = targetAddress.saleNote;
        targetAddress.saleNote = null;
    }

    private void MoveTenantsInto(NewAddress newTenants, NewAddress targetAddress)
    {
        Log("    move inhabitants");
        foreach (var inhabitant in newTenants.inhabitants)
        {
            targetAddress.inhabitants.Add(inhabitant);
            Log($"        + {inhabitant.firstName} {inhabitant.surName}");
        }

        Log("    move owners");
        foreach (var owner in newTenants.owners)
        {
            targetAddress.owners.Add(owner);
            Log($"        + {owner.firstName} {owner.surName}");
        }
        
        Log("    furnish rooms");
        foreach (var addressRoom in targetAddress.rooms)
        {
            addressRoom.RemoveAllInhabitantFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.remove);
            GenerationController.Instance.FurnishRoom(addressRoom);
            Log($"        + furnish {addressRoom.name} with {addressRoom.furniture.Count} furnitures");
            addressRoom.LoadRoomStuff(addressRoom.geometryLoaded);
            addressRoom.SetVisible(true, true, addressRoom.geometryLoaded);
        }

        GameplayController.Instance.forSale.Remove(targetAddress);
    }

    private void MoveTenantsOut(NewAddress oldAddress)
    {
        oldAddress.inhabitants.Clear();
        oldAddress.owners.Clear();
        foreach (var addressRoom in oldAddress.rooms)
        {
            addressRoom.RemoveAllInhabitantFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.remove);
        }
        GameplayController.Instance.forSale.Add(oldAddress);
    }
}