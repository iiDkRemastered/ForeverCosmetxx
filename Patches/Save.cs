using HarmonyLib;

namespace ForeverCosmetx.Patches
{
    [HarmonyPatch(typeof(BuilderSetManager), "OnGotInventoryItems")]
    internal class Save
    {
        static void Postfix() => Plugin.SnapshotRealInventory();
    }
}
