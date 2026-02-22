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
        foreach (var address in GameplayController.Instance.forSale.ToArray())
        {
            var newTenants = FindNewTenants(address);
            if (newTenants != null)
            {
                MoveTenants(newTenants, address);
            }
        }
    }
        
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
        Log($"    addresses on same floor: {addressesOnSameFloor.Length}");

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
            .WhereIsResidential()
            // Port Brewery: around 300 to 400 families can move
            // return only the ones that don't live on the same floor already
            .Where(f => f.floor.floor != addressFloor)
            .RandomElementOrDefault();
    }

    private void MoveTenants(NewAddress newTenants, NewAddress targetAddress)
    {
        LogNewAddressComparison(newTenants, targetAddress);
        
        Log($"    family moves from: {newTenants.name}");
        MoveTenantsInto(newTenants, targetAddress);
        MoveTenantsOut(newTenants);
        
        newTenants.saleNote = targetAddress.saleNote;
        targetAddress.saleNote = null;
        
        LogNewAddressComparison(newTenants, targetAddress);
    }

    private void LogNewAddressComparison(NewAddress valueA, NewAddress valueB)
    {
        Log($"id\t\t{valueA.id}\t\t{valueB.id}");
        Log($"generatedRoomConfigs\t\t{valueA.generatedRoomConfigs}\t\t{valueB.generatedRoomConfigs}");
        Log($"editorID\t\t{valueA.editorID}\t\t{valueB.editorID}");
        Log($"isOutsideAddress\t\t{valueA.isOutsideAddress}\t\t{valueB.isOutsideAddress}");
        Log($"isLobbyAddress\t\t{valueA.isLobbyAddress}\t\t{valueB.isLobbyAddress}");
        Log($"normalizedLandValue\t\t{valueA.normalizedLandValue}\t\t{valueB.normalizedLandValue}");
        Log($"hiddenSpareKey\t\t{valueA.hiddenSpareKey}\t\t{valueB.hiddenSpareKey}");
        Log($"owners\t\t{valueA.owners.Count}\t\t{valueB.owners.Count}");
        Log($"inhabitants\t\t{valueA.inhabitants.Count}\t\t{valueB.inhabitants.Count}");
        Log($"favouredCustomers\t\t{valueA.favouredCustomers.Count}\t\t{valueB.favouredCustomers.Count}");
        Log($"addressPreset\t\t{valueA.addressPreset}\t\t{valueB.addressPreset}");
        Log($"residence\t\t{valueA.residence}\t\t{valueB.residence}");
        Log($"interiorLightsEnabled\t\t{valueA.interiorLightsEnabled}\t\t{valueB.interiorLightsEnabled}");
        Log($"saleNote\t\t{valueA.saleNote}\t\t{valueB.saleNote}");
        Log($"alarmActive\t\t{valueA.alarmActive}\t\t{valueB.alarmActive}");
        Log($"passcode\t\t{valueA.passcode.id}\t\t{valueB.passcode.id}");
    }

    private void MoveTenantsInto(NewAddress newTenants, NewAddress targetAddress)
    {
        Log("    move inhabitants");
        foreach (var inhabitant in newTenants.inhabitants)
        {
            targetAddress.AddInhabitant(inhabitant);
            inhabitant.home = targetAddress;
            Log($"        + {inhabitant.firstName} {inhabitant.surName}");
        }

        Log("    move owners");
        foreach (var owner in newTenants.owners)
        {
            targetAddress.AddOwner(owner);
            owner.residence = targetAddress.residence;
            Log($"        + {owner.firstName} {owner.surName}");
        }
        
        Log("    furnish rooms");
        foreach (var addressRoom in targetAddress.rooms)
        {
            addressRoom.RemoveAllInhabitantFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.remove);
            GenerationController.Instance.FurnishRoom(addressRoom);
            Log($"        + furnish {addressRoom.name} with {addressRoom.furniture.Count} furnitures");
        }
        GenerationController.Instance.GenerateAddressDecor(targetAddress);
        foreach (var addressRoom in targetAddress.rooms)
        {
            addressRoom.SetVisible(true, true, addressRoom.geometryLoaded);
        }
        GameplayController.Instance.forSale.Remove(targetAddress);
    }

    private void MoveTenantsOut(NewAddress oldAddress)
    {
        foreach (var owner in oldAddress.owners.ToArray())
        {
            oldAddress.RemoveOwner(owner);
        }
        foreach (var inhabitant in oldAddress.inhabitants.ToArray())
        {
            oldAddress.RemoveInhabitant(inhabitant);
        }
        foreach (var addressRoom in oldAddress.rooms)
        {
            addressRoom.RemoveAllInhabitantFurniture(true, FurnitureClusterLocation.RemoveInteractablesOption.remove);
            addressRoom.SetVisible(true, true, addressRoom.geometryLoaded);
        }
        GameplayController.Instance.forSale.Add(oldAddress);
    }
}