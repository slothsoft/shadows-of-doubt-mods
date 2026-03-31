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
- **Build** the project to create the mod DLL in mod folder


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