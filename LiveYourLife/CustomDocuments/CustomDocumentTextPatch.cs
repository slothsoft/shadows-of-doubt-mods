using HarmonyLib;

namespace LiveYourLife.CustomDocuments;

// We have a special handling, but all other normal DDS messages keep their original rendering and link handling.
[HarmonyPatch(typeof(Strings), nameof(Strings.GetTextForComponent))]
internal static class CustomDocumentTextPatch
{
    [HarmonyPrefix]
    private static bool Prefix(string msgID, object obj, ref string __result)
    {
        foreach (var customDocument in BaseCustomDocument.AllTypes)
        {
            if (msgID == customDocument.MessageId)
            {
                var note = obj as Interactable;
                __result = customDocument.ReadText(note) ?? $"Custom document text unavailable (type= {customDocument.TypeName})";
                return false;   
            }
        }
        
        return true;
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
        var (registered, failed) = BaseCustomDocument.RegisterTypes();
        LiveYourLifePlugin.Logger.LogDebug($"Registered {registered + failed} custom document types: {registered} registered, {failed} failed");
    }
}
