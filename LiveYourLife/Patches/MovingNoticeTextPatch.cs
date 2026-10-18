using HarmonyLib;
using LiveYourLife.AddressMoves;

namespace LiveYourLife.Patches;

// All normal DDS messages keep their original rendering and link handling.
[HarmonyPatch(typeof(Strings), nameof(Strings.GetTextForComponent))]
internal static class MovingNoticeTextPatch
{
    [HarmonyPrefix]
    private static bool Prefix(string msgID, object obj, ref string __result)
    {
        if (msgID != MovingNotice.MessageId)
            return true;
        
        var note = obj as Interactable;
        __result = MovingNotice.ReadText(note) ?? "Moving notice text unavailable.";
        return false;
    }
}

// Re-register the runtime-only layout whenever the game's DDS files are loaded.
// The document itself carries its text in serialized Interactable.pv data.
[HarmonyPatch(typeof(Toolbox), nameof(Toolbox.LoadDDSFilesFromPath))]
internal static class MovingNoticeDocumentPatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        MovingNotice.RegisterDocument();
    }
}
