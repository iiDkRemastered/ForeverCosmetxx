using BepInEx;
using BepInEx.Configuration;
using GorillaNetworking;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ForeverCosmetx
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public static bool Enabled;

        internal static List<CosmeticsController.CosmeticItem> realItems = new List<CosmeticsController.CosmeticItem>();
        internal static HashSet<string> realItemNames = new HashSet<string>();
        internal static string realConcatString = "";
        static readonly HashSet<string> addedItemNames = new HashSet<string>();

        ConfigEntry<bool> startEnabled;
        ConfigEntry<Key> toggleKey;

        void Awake()
        {
            instance = this;
            TelemetryClient.Attach();
            startEnabled = Config.Bind("General", "Enabled On Start", true, "Unlock every cosmetic as soon as the game loads.");
            toggleKey = Config.Bind("General", "Toggle Key", Key.F8, "Press this key in game to toggle every cosmetic on/off. Toggling off removes the unlocked cosmetics");
            Enabled = startEnabled.Value;
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            if (keyboard[toggleKey.Value].wasPressedThisFrame) SetEnabled(!Enabled);
        }

        public void SetEnabled(bool value)
        {
            if (Enabled == value) return;
            Enabled = value;

            if (Enabled) UnlockCosmetics();
            else RestoreRealCosmetics();
        }

        internal static void SnapshotRealInventory()
        {
            CosmeticsController c = CosmeticsController.instance;
            if (c == null || c.allCosmetics == null) return;

            realItems = new List<CosmeticsController.CosmeticItem>(c.unlockedCosmetics);
            realItemNames.Clear();
            foreach (CosmeticsController.CosmeticItem item in realItems) realItemNames.Add(item.itemName);
            realConcatString = c.concatStringCosmeticsAllowed ?? "";
            addedItemNames.Clear();

            if (Enabled) instance.UnlockCosmetics();
        }

        public void OnCosmeticsSynced()
        {
            if (Enabled) UnlockCosmetics();
        }

        public void UnlockCosmetics()
        {
            CosmeticsController c = CosmeticsController.instance;
            if (c == null || c.allCosmetics == null) return;

            MethodInfo unlockItem = typeof(CosmeticsController).GetMethod("UnlockItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (unlockItem == null) return;

            foreach (CosmeticsController.CosmeticItem cosmeticItem in c.allCosmetics)
            {
                if (cosmeticItem.isNullItem) continue;
                if (realItemNames.Contains(cosmeticItem.itemName) || addedItemNames.Contains(cosmeticItem.itemName)) continue;

                try
                {
                    unlockItem.Invoke(c, new object[] { cosmeticItem.itemName, false });
                    addedItemNames.Add(cosmeticItem.itemName);
                }
                catch { }
            }

            RefreshAfterInventoryChange();
        }

        public void RestoreRealCosmetics()
        {
            CosmeticsController c = CosmeticsController.instance;
            if (c == null || c.allCosmetics == null) return;

            c.unlockedCosmetics.Clear();
            c.unlockedHats.Clear();
            c.unlockedFaces.Clear();
            c.unlockedBadges.Clear();
            c.unlockedPaws.Clear();
            c.unlockedChests.Clear();
            c.unlockedFurs.Clear();
            c.unlockedShirts.Clear();
            c.unlockedPants.Clear();
            c.unlockedArms.Clear();
            c.unlockedBacks.Clear();
            c.unlockedTagFX.Clear();
            c.unlockedThrowables.Clear();

            foreach (CosmeticsController.CosmeticItem item in realItems)
            {
                if (item.isNullItem) continue;
                c.unlockedCosmetics.Add(item);

                if (item.itemCategory == CosmeticsController.CosmeticCategory.Hat) c.unlockedHats.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Face) c.unlockedFaces.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Badge) c.unlockedBadges.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Fur) c.unlockedFurs.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Chest) c.unlockedChests.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Shirt) c.unlockedShirts.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Back) c.unlockedBacks.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Arms) c.unlockedArms.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Pants) c.unlockedPants.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.TagEffect) c.unlockedTagFX.Add(item);
                else if (item.itemCategory == CosmeticsController.CosmeticCategory.Paw)
                {
                    if (item.isThrowable) c.unlockedThrowables.Add(item);
                    else c.unlockedPaws.Add(item);
                }
            }

            addedItemNames.Clear();
            c.concatStringCosmeticsAllowed = realConcatString;
            if (c.tryOnSet != null) c.tryOnSet.ClearSet(c.nullItem);

            RefreshAfterInventoryChange();
        }

        void RefreshAfterInventoryChange()
        {
            CosmeticsController c = CosmeticsController.instance;
            if (c == null) return;

            if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig != null)
            {
                try { c.currentWornSet.LoadFromPlayerPreferences(c); } catch { }
                try { c.UpdateWornCosmetics(true); } catch { }
            }

            try { c.UpdateWardrobeModelsAndButtons(); } catch { }
        }
    }
}
