# Shadows of Doubt Mods

Mods for the game [Shadows of Doubt](https://store.steampowered.com/agecheck/app/986130/).

**Content:**
<!-- TOC -->
  * [Mods](#mods)
  * [How to Develop for SoD](#how-to-develop-for-sod)
    * [Useful Classes](#useful-classes)
    * [Useful Methods](#useful-methods)
    * [Useful Code Snippets](#useful-code-snippets)
      * [Find citizens who don't live with their partner](#find-citizens-who-dont-live-with-their-partner)
<!-- TOC -->

## Mods

- **[LiveYourLife](./LiveYourLife/ReadMe.md)** - lets NPCs move on with their lives

## How to Develop for SoD

- **[Official Documentation](https://shadowsofdoubt.notion.site/Shadows-of-Doubt-Mod-Documentation-33744ac558304d89997e88f2051280f7)**
- if you start in a new workspace, you need to [switch to the mono branch of Shadows of Doubt](https://shadowsofdoubt.notion.site/Accessing-the-Mono-Branch-129b1dba7a80427683c66ce1f670a218)
- **Publish** the project to create the mod DLL in mod folder: `dotnet publish .\LiveYourLife\LiveYourLife.csproj -c Release`


### Useful Classes

- `CityBuildings.Instance`
- `CityControls.Instance`
- `CityData.Instance`
- `GameplayController.Instance`
- `GenerationController.Instance`
- `InteriorCreator.Instance`
- `RoomsLoader.Instance`


### Useful Methods

- `NewRoom`
    - `RemoveAllInhabitantFurniture()`
    - `RemoveOccupant()`


### Useful Code Snippets

#### Find citizens who don't live with their partner

```c#    
Log("Finding citizens who don't live with their partner");
foreach (var citizen in CityData.Instance.citizenDirectory.Where(c => c != null && c.partner != null && c.residence != null))
{
    var citizenAddress = citizen.residence?.address;
    var partnerAddress = citizen.partner?.residence?.address;
    if (citizenAddress != null && partnerAddress != citizenAddress)
    {
        Log($"- {citizen.firstName} {citizen.surName} and {citizen.partner?.firstName} {citizen.partner?.surName} do not live together ({citizenAddress.name ?? "homeless"} vs. {partnerAddress?.name ?? "homeless"})");
    }
 }
```

# Other Mods

- [PiePieOnline's Mods](https://github.com/piepieonline/ShadowsOfDoubtMods)
- [Ven0maus's Mods](https://github.com/Ven0maus/SOD.Common/)
- [ReveredRedHood's Mods](https://github.com/ReveredRedHood/SpyraxiShadowsOfDoubtMods)
- [TRUEDARTT's Mods](https://github.com/TRUEDARTT/TRUEDARTT-s-Shadows-of-Doubt-MODS)
- [Wund3rcr4zy's By The Book](https://github.com/wund3rcr4zy/by-the-book)
  

