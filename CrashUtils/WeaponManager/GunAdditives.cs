using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CrashUtils.WeaponManager.WeaponSetup
{
    public class GunAdditives : Module
    {
        private static string SavePath = Path.Combine(PathUtils.ModPath(), "save");
        internal static List<WeaponSuper> WeaponList = new List<WeaponSuper>();
        internal static List<WeaponSuper> WeaponAdditionList = new List<WeaponSuper>();
        internal static Dictionary<string, int> WeaponOwned = new Dictionary<string, int>();
        internal static int SlotVar = 0;

        public static void LoadData()
        {
            if (File.Exists(SavePath + GameProgressSaver.currentSlot))
            {
                List<string> silly = File.ReadLines(SavePath + GameProgressSaver.currentSlot).ToList();
                foreach (string line in silly)
                {
                    string[] stuff = line.Split('~');
                    if (WeaponOwned.ContainsKey(stuff[0]))
                    {
                        WeaponOwned[stuff[0]] = Convert.ToInt32(stuff[1]);
                    }
                    else
                    {
                        WeaponOwned.Add(stuff[0], Convert.ToInt32(stuff[1]));
                    }
                }
            }
        }

        public static void SaveData()
        {
            List<string> data = new List<string>();

            foreach (var kvp in WeaponOwned)
            {
                string toAdd = $"{kvp.Key}~{kvp.Value}";
                data.Add(toAdd);
            }
            File.WriteAllLines(SavePath + GameProgressSaver.currentSlot, data);
        }

        public static void Register(WeaponSuper weapon)
        {
            if (weapon.WheelOrder() <= 5)
            {
                WeaponList.Add(weapon);
            }
            else
            {
                Debug.Log("Added Weapon -------------");
                WeaponAdditionList.Add(weapon);
            }

            if (!WeaponOwned.ContainsKey("weapon." + weapon.Pref()))
            {
                WeaponOwned.Add("weapon." + weapon.Pref(), 0);
            }
        }

        public override void Patch(Harmony harmony)
        {
            harmony.PatchAll(typeof(GunRegPatches));
        }

        public class GunRegPatches
        {
            [HarmonyPatch(typeof(GameProgressSaver), nameof(GameProgressSaver.SetSlot))]
            [HarmonyPrefix]
            public static void SaveOnSlotChange()
            {
                SaveData();
                string[] keys = WeaponOwned.Keys.ToArray();

                foreach (var thing in keys)
                {
                    WeaponOwned[thing] = 0;
                }
            }

            [HarmonyPatch(typeof(GameProgressSaver), nameof(GameProgressSaver.SetSlot))]
            [HarmonyPostfix]
            public static void LoadOnSlotChange()
            {
                LoadData();
            }

            [HarmonyPatch(typeof(GameProgressSaver), nameof(GameProgressSaver.GetGeneralProgress))]
            [HarmonyPostfix]
            public static void LoadOnStart()
            {
                try
                {
                    LoadData();
                }
                catch (Exception ex) { Debug.LogError(ex.ToString()); }
            }

            [HarmonyPatch(typeof(GunSetter), nameof(GunSetter.ResetWeapons))]
            [HarmonyPostfix]
            public static void GiveGuns(GunSetter __instance)
            {
				List<List<GameObject>> SlotToList = __instance.GetComponent<GunControl>().slots;

				foreach (WeaponSuper weapon in WeaponList)
                {
                    bool IsFist = weapon.GetType() == typeof(Fist) || weapon.GetType().IsSubclassOf(typeof(Fist));

                    
                    if (GameProgressSaver.CheckGear(weapon.Pref()) == 1)
                    {
                        GameObject created = weapon.Create(__instance.transform);
                        created.SetActive(false);
                        SlotToList[weapon.Slot()].Add(created);
                    }
                    else
                    {
                        WeaponOwned["weapon." + weapon] = 0;
                    }
                    
                    
                }

                foreach (List<GameObject> slot in SlotToList)
                {
                    while (slot.Remove(null))
                        ;
                }
            }

            [HarmonyPatch(typeof(FistControl), nameof(FistControl.ResetFists))]
            [HarmonyPostfix]
            public static void GiveFists(FistControl __instance)
            {
                foreach (WeaponSuper weapon in WeaponList)
                {
                    bool IsFist = weapon.GetType() == typeof(Fist) || weapon.GetType().IsSubclassOf(typeof(Fist));

                    if (weapon.Enabled() > 0 && IsFist)
                    {
                        if (GameProgressSaver.CheckGear(weapon.Pref()) == 1)
                        {
                            GameObject created = weapon.Create(__instance.transform);
                            created.SetActive(false);

                            FistControl.Instance.spawnedArms.Add(created);
                            FistControl.Instance.spawnedArmNums.Add(weapon.Slot());
                        }
                        else
                        {
                            WeaponOwned["weapon." + weapon.Pref()] = 0;
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(GameProgressSaver), nameof(GameProgressSaver.CheckGear))]
            [HarmonyPrefix]
            public static bool CheckGearForCustoms(ref int __result, string gear)
            {
                foreach (WeaponSuper weapon in WeaponList)
                {
                    if (weapon.Pref() == gear)
                    {
                        __result = WeaponOwned["weapon." + weapon.Pref()];
                        return false;
                    }
                }
                return true;
            }

            [HarmonyPatch(typeof(GameProgressSaver), nameof(GameProgressSaver.AddGear))]
            [HarmonyPrefix]
            public static bool AddGearForCustoms(string gear)
            {
                foreach (WeaponSuper weapon in WeaponList)
                {
                    if (weapon.Pref() == gear)
                    {
                        WeaponOwned["weapon." + weapon.Pref()] = 1;
                        SaveData();
                        return false;
                    }
                }
                return true;
            }


            [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start))]
            [HarmonyPostfix]
            public static void AddNewSlots(GunControl __instance)
            {
                Debug.Log("Got here");

                
                foreach (WeaponSuper weapon in WeaponAdditionList)
                {
                    GameObject created = weapon.Create(__instance.transform);
                    created.SetActive(false);
                    created.name = weapon.Pref();

                    // Add empty slots until the slot for the weapon is created
                    while (__instance.slots.Count < (weapon.WheelOrder() + 1))
                        __instance.slots.Add(new List<GameObject>());
                    __instance.slots[weapon.WheelOrder()].Add(created);
                }



                foreach (var slot in __instance.slots)
                {
                    while (slot.Remove(null))
                        ;
                }

                //this.SwitchWeapon(6, this.slot6, false, true, false);
                //this.SwitchWeapon(this.lastUsedSlot, this.slots[this.lastUsedSlot - 1], true, false, false);

                //properly sets last used weapon in a hacky way if it's out of the "Standard" array index
                if (SlotVar > 6)
                {
                    __instance.SwitchWeapon(SlotVar, __instance.slots[SlotVar - 1]);
                }

                __instance.UpdateWeaponList(true);

            }

            [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start))]
            [HarmonyPrefix]
            public static void SaveLastWeapon(GunControl __instance)
            {
               
                SlotVar = PlayerPrefs.GetInt("CurSlo", 1);

            }

            


        }
    }

}
