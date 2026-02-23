using BepInEx.Logging;
using SOD.Common;
using SOD.Common.Helpers;

namespace LiveYourLife.AddressMoves;

public static class AddressMovesFeature
{
    internal static ManualLogSource Logger => LiveYourLifePlugin.Logger;

    public static void Load()
    {
        Lib.SaveGame.OnAfterLoad += (_, e)
            // saved LiveYourLifeConfig data is loaded OnBeforeLoad, so if addresses have no timestamp here, the plug-in is new
            => new AddressSync(true)
            {
                Log = Logger.LogInfo,
            }.Sync(AddressMovesSaveData.Instance);
        Lib.Time.OnMinuteChanged += MoveTenantsIfApplicable;
    }

    private static void MoveTenantsIfApplicable(object sender, TimeChangedArgs e)
    {
        Logger.LogInfo($"{LiveYourLifePlugin.Name} tick on {Lib.Time.CurrentDateTime.ToString(LiveYourLifePlugin.DateFormat)}");
        var company = new MovingCompany(LiveYourLifeConfig.Instance.AddressMoves, AddressMovesSaveData.Instance)
        {
            Log = Logger.LogInfo
        };
        company.MoveTenantsIfApplicable();
    }
}