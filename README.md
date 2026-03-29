# Shadows of Doubt Mods

Mods for the game [Shadows of Doubt](https://store.steampowered.com/agecheck/app/986130/).

**Content:**
<!-- TOC -->
  * [Mods](#mods)
  * [How to Develop for SoD](#how-to-develop-for-sod)
    * [Useful Classes](#useful-classes)
    * [Useful Methods](#useful-methods)
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

