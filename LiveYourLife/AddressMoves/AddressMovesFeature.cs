using BepInEx.Logging;
using LiveYourLife.Common;
using LiveYourLife.CustomDocuments;
using SOD.Common;
using SOD.Common.Helpers;

namespace LiveYourLife.AddressMoves;

public static class AddressMovesFeature
{
    internal static ManualLogSource Logger => LiveYourLifePlugin.Logger;

    public static void Load()
    {
        Lib.Time.OnTimeInitialized += (_, e)
            // saved LiveYourLifeConfig data is loaded OnBeforeLoad, so if addresses have no timestamp here, the plug-in is new
            => {
            BaseCustomDocument.RegisterTypes();
            var saveData = AddressMovesSaveData.Instance;
            LiveYourLifePlugin.Logger.LogInfo($"{saveData.AddressSales.Count} addresses loaded");
            new AddressSync(true)
            {
                Log = Logger.LogInfo,
            }.Sync(saveData);
        };
        Lib.Time.OnHourChanged += MoveTenantsIfApplicable;
    }

    private static void MoveTenantsIfApplicable(object? sender, TimeChangedArgs e)
    {
        Logger.LogInfo($"{LiveYourLifePlugin.Name} tick on {SessionData.Instance.gameTime.AsGameTimeString()}");
        var movers = new MovingCompany(LiveYourLifeConfig.Instance.AddressMoves, AddressMovesSaveData.Instance)
        {
            Log = Logger.LogInfo
        };
        movers.MoveTenantsIfApplicable();
    }
}
