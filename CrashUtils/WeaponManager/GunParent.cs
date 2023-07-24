using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


 namespace CrashUtils.WeaponManager.WeaponSetup
{
     public class Gun : WeaponSuper
     {
         public static bool OnFire()
         {
             return InputSource.Fire1.WasPerformedThisFrame;
         }

         public static bool OnAltFire()
         {
             return InputSource.Fire2.WasPerformedThisFrame;
         }

         public static bool OnFireHeld()
         {
             return InputSource.Fire1.IsPressed;
         }

         public static bool OnAltFireHeld()
         {
             return InputSource.Fire2.IsPressed;
         }

         public static bool OnFireReleased()
         {
             return InputSource.Fire1.WasCanceledThisFrame;
         }

         public static bool OnAltFireReleased()
         {
             return InputSource.Fire2.WasCanceledThisFrame;
         }
     }
 }


