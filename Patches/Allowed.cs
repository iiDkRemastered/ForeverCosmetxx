using HarmonyLib;

namespace ForeverCosmetx.Patches
{
    [HarmonyPatch(typeof(VRRig), "IsItemAllowed")]
    internal class Allowed
    {
        static void Postfix(ref bool __result)
        {
            if (Plugin.Enabled) __result = true;
        }
    }
}
