using BepInEx;
using CrashUtils.WeaponManager;
using CrashUtils.WeaponManager.WeaponSetup;
using HarmonyLib;
using UnityEngine;

namespace CrashRefrences
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class CrashUtils : BaseUnityPlugin
    {
        public static AssetBundle Assets;

        private static Harmony Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        public Module[] Modules =
        {
            new GunAdditives()
        };

        public void Start()
        {
            foreach (Module module in Modules)
            {
                module.Patch(Harmony);
            }
            
        }

        public void OnDestroy()
        {
            GunAdditives.SaveData();
        }
    }
}
