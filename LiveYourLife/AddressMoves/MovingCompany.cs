using System;
using System.Linq;
using LiveYourLife.Common;
using SOD.Common;
using SOD.Common.Extensions;

namespace LiveYourLife.AddressMoves;

public class MovingCompany(AddressMovesConfig config, AddressMovesSaveData saveData)
{
    public Action<string> Log { get; set; } = Console.WriteLine;

    public void MoveTenantsIfApplicable()
    {
        foreach (var address in GameplayController.Instance.forSale.ToArray().WhereIsReadyForNewTenants(config, saveData))
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
        // LogNewAddressComparison(newTenants, targetAddress);

        Log($"    family moves from: {newTenants.name}");
        MoveHumans(newTenants, targetAddress);
        MoveTenantsInto(newTenants, targetAddress);
        MoveTenantsOut(newTenants);
        CloseSale(newTenants, targetAddress);

        // LogNewAddressComparison(newTenants, targetAddress);
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

    private void MoveHumans(NewAddress newTenants, NewAddress targetAddress)
    {
        Log("    move inhabitants");
        foreach (var inhabitant in newTenants.inhabitants.ToArray())
        {
            newTenants.RemoveInhabitant(inhabitant);
            targetAddress.AddInhabitant(inhabitant);
            inhabitant.home = targetAddress;
            inhabitant.residence = targetAddress.residence;
            Log($"        + {inhabitant.firstName} {inhabitant.surName}");
        }
        
        Log("    move owners");
        foreach (var owner in newTenants.owners.ToArray())
        {
            newTenants.RemoveOwner(owner);
            targetAddress.AddOwner(owner);
            Log($"        + {owner.firstName} {owner.surName}");
        }
    }

    private void MoveTenantsInto(NewAddress newTenants, NewAddress targetAddress)
    {
        GameplayController.Instance.forSale.Remove(targetAddress);
        
        Log("    furnish rooms");
        var generationController = GenerationController.Instance;
        foreach (var addressRoom in targetAddress.rooms)
        {
            addressRoom.RemoveAllInhabitantFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.remove);
            generationController.FurnishRoom(addressRoom);
            Log($"        + furnish {addressRoom.name} with {addressRoom.furniture.Count} furnitures");
        }
        generationController.GenerateAddressDecor(targetAddress);
            
        foreach (var addressRoom in targetAddress.rooms)
        {
            addressRoom.SetVisible(true, true, addressRoom.geometryLoaded);
            addressRoom.LoadRoomStuff(addressRoom.geometryLoaded);
        }
    }

    private void MoveTenantsOut(NewAddress oldAddress)
    {
        foreach (var addressRoom in oldAddress.rooms)
        {
            addressRoom.RemoveAllInhabitantFurniture(true, FurnitureClusterLocation.RemoveInteractablesOption.remove);
            addressRoom.SetVisible(true, true, addressRoom.geometryLoaded);
        }

        GameplayController.Instance.forSale.Add(oldAddress);
    }
    
    private void CloseSale(NewAddress newTenants, NewAddress targetAddress)
    {
        newTenants.saleNote = targetAddress.saleNote;
        targetAddress.saleNote = null;
        
        newTenants.saleNote.forSale = newTenants;
        saveData.LastMove = Lib.Time.CurrentDateTime;
    }
}