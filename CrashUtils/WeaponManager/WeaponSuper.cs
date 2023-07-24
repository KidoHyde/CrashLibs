using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CrashUtils.WeaponManager.WeaponSetup
{
    public class WeaponSuper
    {
        protected static PlayerInput InputSource;
        public static WeaponSuper Instance;
        public int OrderInSlot = -1;

        public virtual GameObject Create(Transform parent)
        {
            InputSource = InputManager.Instance.InputSource;
            return new GameObject();
        }

        public virtual int Slot()
        {
            return 0;
        }

        public virtual int WheelOrder()
        {
            return 0;
        }

        public virtual string Pref()
        {
            return "default0";
        }

        public int Enabled()
        {
            return PrefsManager.Instance.GetInt("weapon." + Pref());
        }
    }

}
