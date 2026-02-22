using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using SOD.Common;
using SOD.Common.Helpers;

namespace LiveYourLife;

[BepInPlugin(Id, Name, Version)]
public class Plugin : BasePlugin
{
    private const string Id = "de.slothsoft.shadowsofdoubt.LiveYourLife";
    private const string Name = "LiveYourLife";
    private const string Version = "1.0.0";
    
    internal static ManualLogSource Logger;
    internal static bool furnishOnce = true;
        
    public override void Load()
    {
        Logger = Log;
        Logger.LogInfo($"{Name} Plugin loaded. (ID={Id} Version={Version})");
        Harmony.CreateAndPatchAll(typeof(Plugin));

        Lib.Time.OnMinuteChanged += Time_OnHourChanged;

    }
    
    private void Time_OnHourChanged(object sender, TimeChangedArgs e)
    {
        Logger.LogInfo($"{Name} tick ({furnishOnce})");
        if (!furnishOnce) return;
        
        var company = new MovingCompany
        {
            Log = Logger.LogInfo
        };
        company.MoveTenantsIfApplicable();
        

        furnishOnce = false;
    }
}