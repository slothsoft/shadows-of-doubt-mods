using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using LiveYourLife.AddressMoves;
using LiveYourLife.Common;

namespace LiveYourLife;

[BepInPlugin(Id, Name, Version)]
public class LiveYourLifePlugin : BasePlugin
{
    private const string Id = "de.slothsoft.shadowsofdoubt.LiveYourLife";
    internal const string Name = "LiveYourLife";
    private const string Version = "1.0.0";
    internal const string DateFormat = "yyyy-MM-dd HH:mm";

    internal static ManualLogSource Logger = null!;
    internal static SaveDataManager<LiveYourLifeSaveData> SaveDataManager = null!;

    public override void Load()
    {
        Logger = Log;
        Logger.LogInfo($"{Name} Plugin loaded. (ID={Id} Version={Version})");
        Harmony.CreateAndPatchAll(typeof(LiveYourLifePlugin));

        SaveDataManager = new SaveDataManager<LiveYourLifeSaveData>(Name);
        
        LiveYourLifeConfig.Instance.Register(Config);
        AddressMovesFeature.Load();
    }
}