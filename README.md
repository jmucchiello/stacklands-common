# COMMON MOD STUFF

This isn't actually a MOD. It is a code repository for common things used in Stacklands Mods. The files are included in the various mods I write as links:

```
/github/stacklands-common
/github/stacklands-resources
/github/stacklands-strictmode
```

The csproj files in resources and strictmode include the files in common by link: `../stacklands-common/ConfigEntryBool.cs`, for example.

## ConfigEntryBool
Differs from ConfigEntry<bool> in that the On and Off text is blue. Making the variable text blue is a feature of most of these library functions

## ConfigToggleEnum<T>
This is basically the same as `ConfigEntryBool` except that you can choose from among the entries in an Enum rather than choosing from On/Off.

## ConfigEntryEnum<T> And ConfigStringList
Create a popup screen where you can choice from the list of given choices

## ConfigEntryHelper
Base class for all Config classes. Contains static text manipulation for color and text alignment.

## ConfigEntryModalHelper
Base class for `ConfigEntryEnum<T>` and `ConfigStringList` encapsulating the Stacklands Modal dialog.

## Instance
A bunch of short cuts. Typing out `WorldManager.instance`. takes a lot longer than `I.WM`.

* I.WM => WorldManger.Instance
* I.GameState => WM.CurrentGameState;
* I.CRV => WM.CurrentRunVariables;
* I.GDL => WM.GameDataLoader;
* I.PFM => PrefabManager.instance;
* I.GS => GameScreen.instance;
* I.MOS => ModOptionsScreen.instance;
* I.Modal => ModalScreen.instance;

* I.Log => TheMod.instance.Log<br/>Hooked automatically using reflection.
* I.Xlat => Sokloc.Translate<br/>Also, doesn't convert missing entries to ---MISSING---. It does write to the log when it fails to find an entry.

## WorldManagerPatches
Patches to load/save/new/update/play methods in `WorldManager`. You don't have to call HarmonyPatch to use this. Only the methods that have Actions added are patched when you call `ApplyPatches` Usage:
```
void Awake()
{
	WorldManagerPatches.LoadSaveGame += WM_OnLoadSave;
	WorldManagerPatches.Update += WM_OnUpdate; 
	WorldManagerPatches.ApplyPatches(Harmony); // don't forget to pass Mod.Harmony
}
private static void WM_OnLoadSave(WorldManager wm, SaveRound round)
{
// do stuff
}
private static void WM_OnUpdate(WorldManager wm)
{
// do stuff
}
```
# Challenge Mods
This mod is part of my series of mods that allow you to control the difficulty of the game. Try these other mods for increased control.

* [Can't Stop](https://steamcommunity.com/sharedfiles/filedetails/?id=3047503037) - Want to Pause the game? You can't. (coming soon)
* Cursed Worlds DLC Difficulty - Modify the difficulty of various aspects of the Spirit World DLC. (This Mod)
* [Enemy Difficulty](https://steamcommunity.com/sharedfiles/filedetails/?id=3044524742)- Change the power of all spawned enemies. (coming soon)
* Peaceful Mode Toggle - Turn Peaceful Mode on and off without starting a new game. (coming sooner or later)
* [Spawned Enemies Control](https://steamcommunity.com/sharedfiles/filedetails/?id=3044203151) - Control the speed and location of end of month events.
* [Strict Mode](https://steamcommunity.com/sharedfiles/filedetails/?id=3026405806) - Can't use ideas you have not found in game.
* Time Keeper - Track time playing a save file and the ability to lock the settings of my Challenge Mods in the save file (coming sooner or later)
