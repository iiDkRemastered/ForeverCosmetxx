using GorillaNetworking;
using HarmonyLib;
using System;
using UnityEngine;

namespace ForeverCosmetx.Patches
{
    [HarmonyPatch(typeof(CosmeticsController.CosmeticSet), "LoadFromPlayerPreferences")]
    internal class NoNull
    {
        static bool Prefix(CosmeticsController.CosmeticSet __instance, CosmeticsController controller)
        {
            if (!Plugin.Enabled) return true;

            if (__instance.items == null) __instance.items = new CosmeticsController.CosmeticItem[16];
            int num = Math.Min(16, __instance.items.Length);
            for (int i = 0; i < num; i++)
            {
                CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
                __instance.items[i] = controller.GetItemFromDict(PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), "NOTHING"));
            }
            return false;
        }
    }
}
