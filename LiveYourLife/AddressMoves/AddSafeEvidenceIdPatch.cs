using System;
using HarmonyLib;

namespace LiveYourLife.AddressMoves;

/*
 * Safe keypads created during gameplay can display unrelated evidence, such as "Entry wound", instead of the keypad
 * information.
 *
 * InteractableCreator.CreateInteractableLock() creates a transient lock Interactable without assigning an ID, leaving
 * it at 0. MainSetupStart explicitly excludes ID-0 objects from the object registries. Locks are recreated from their
 * parent objects when loading.
 *
 * However, MainSetupEnd requests evidence using "I" + interactable.id. Every such lock therefore requests the same
 * evidence key, "I0". GetOrCreateEvidenceForInteractable returns any existing evidence under that key without checking
 * whether it belongs to the requested preset.
 *
 * This was observed on a newly generated safe: its keypad had the correct owner and passcode, but id=0 and
 * evidencePreset=EntryWound.
 *
 * The workaround changes only the evidence key during lock creation. It derives a stable key from the furniture ID,
 * preset, controller pairing and local position, preserving the lock's transient ID-0 lifecycle and leaving existing
 * "I0" evidence untouched.
 *
 * This prevents collisions for newly created locks; it does not repair evidence already assigned to existing locks.
 */
[HarmonyPatch(
    typeof(Toolbox),
    nameof(Toolbox.GetOrCreateEvidenceForInteractable))]
internal static class LockEvidencePatch
{
    [HarmonyPrefix]
    private static void Prefix(Interactable? interactable, ref string newID)
    {
        if (interactable == null || interactable.id != 0)
            return;

        if (interactable.preset == null ||
            interactable.preset.name != "Keypad")
            return;

        var furniture = interactable.furnitureParent;
        if (furniture == null)
            return;

        var position = interactable.lPos;

        newID = FormattableString.Invariant(
            $"LiveYourLife.Lock.{furniture.id}.{interactable.preset.name}.{interactable.pt}.{position.x:R}.{position.y:R}.{position.z:R}");
    }
}