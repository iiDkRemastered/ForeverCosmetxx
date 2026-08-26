using BepInEx;
using GorillaNetworking;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

/*
 * I'm tired of the 4000 different Cosmetx "Fixes".
 * Some of them even have MALWARE in them.
 * To get this stressed out over cosmetics is terrible.
 * And the fact that they are all unoptimized and terrible puts salt in the wound.
 * 
 * This Cosmetx mod should work FOREVER unless CosmeticsController gets severely updated in a bad way.
 */

namespace ForeverCosmetx
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        void Awake()
        {
            instance = this;
            HarmonyPatches.ApplyHarmonyPatches();
        }

        public void UnlockCosmetics()
        {
            MethodInfo UnlockItem = typeof(CosmeticsController).GetMethod("UnlockItem", BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (CosmeticsController.CosmeticItem cosmeticItem in CosmeticsController.instance.allCosmetics)
            {
                if (!CosmeticsController.instance.concatStringCosmeticsAllowed.Contains(cosmeticItem.itemName))
                {
                    try
                    {
                        UnlockItem.Invoke(CosmeticsController.instance, new object[] { cosmeticItem.itemName, false });
                    } catch { }
                }
            }

            CosmeticsController.instance.OnCosmeticsUpdated.Invoke();
        }
    }
}
