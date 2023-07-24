using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CrashUtils.WeaponManager.WeaponSetup
{
    public class Fist : WeaponSuper
    {
        public static bool OnPunch()
        {
            return InputSource.Punch.WasPerformedThisFrame;
        }

        public static bool OnPunchHeld()
        {
            return InputSource.Punch.IsPressed;
        }

        public static bool OnPunchReleased()
        {
            return InputSource.Punch.WasCanceledThisFrame;
        }
    }

}
