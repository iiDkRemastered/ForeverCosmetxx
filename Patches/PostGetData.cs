using GorillaNetworking;
using HarmonyLib;

namespace ForeverCosmetx.Patches
{
    [HarmonyPatch(typeof(CosmeticsController), "Awake")]
    internal class PostGetData
    {
        static void Postfix(CosmeticsController __instance) => __instance.OnCosmeticsUpdated += Plugin.instance.OnCosmeticsSynced;
    }
}
