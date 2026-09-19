using GorillaNetworking;
using GorillaNetworking.Store;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForeverCosmetx.Patches
{
    [HarmonyPatch(typeof(CosmeticsController))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    public class PostGetData
    {
        private static void Postfix(CosmeticsController __instance)
        {
            __instance.V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess += Plugin.instance.UnlockCosmetics;
        }
    }
}
