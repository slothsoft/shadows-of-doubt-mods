# Shadows of Doubt Mods

"Build" project to create mod in mod folder.

Official Documentation: https://shadowsofdoubt.notion.site/Shadows-of-Doubt-Mod-Documentation-33744ac558304d89997e88f2051280f7

## To-Do

- [ ] spawn decor for new address
- [ ] add correct "for sale" for old address
- [x] only move after a while
- [ ] the "initial" date is wrong (current instead of game start)
- [ ] we should have some way to get to moving data
- [ ] block persons from moving a lot

Extended:

- two people could move in together (are there couples that do not live together?)
- two people could separate (or move in with paramour)
- homeless people could move in
- two people could get married (one of them changes their last name)
- can more than two people live at an address? especially if an address has more than one bedroom

Classes that might be useful:

- `CityBuildings.Instance`
- `CityControls.Instance`
- `CityData.Instance`
- `GameplayController.Instance`
- `GenerationController.Instance`
- `InteriorCreator.Instance`
- `RoomsLoader.Instance`

Methods that might be useful:

- `NewRoom`
    - `RemoveAllInhabitantFurniture()`
    - `RemoveOccupant()`

