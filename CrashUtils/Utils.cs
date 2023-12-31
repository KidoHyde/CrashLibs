﻿using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CrashUtils
{
 
     public static class PathUtils
     {
         public static string GameDirectory()
         {
             string path = Application.dataPath;
             if (Application.platform == RuntimePlatform.OSXPlayer)
             {
                 path = Utility.ParentDirectory(path, 2);
             }
             else if (Application.platform == RuntimePlatform.WindowsPlayer)
             {
                 path = Utility.ParentDirectory(path, 1);
             }

             return path;
         }

         public static string ModDirectory()
         {
             return Path.Combine(GameDirectory(), "BepInEx", "plugins");
         }

         public static string ModPath()
         {
             return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
         }
     }

     public static class GameObjectUtils
     {
         public static GameObject ChildByName(this GameObject from, string name)
         {
             List<GameObject> children = new List<GameObject>();
             int count = 0;
             while (count < from.transform.childCount)
             {
                 children.Add(from.transform.GetChild(count).gameObject);
                 count++;
             }

             if (count == 0)
             {
                 return null;
             }

             for (int i = 0; i < children.Count; i++)
             {
                 if (children[i].name == name)
                 {
                     return children[i];
                 }
             }
             return null;
         }

         public static List<GameObject> FindSceneObjects(string sceneName)
         {
             List<GameObject> objs = new List<GameObject>();
             foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
             {
                 if (obj.scene.name == sceneName)
                 {
                     objs.Add(obj);
                 }
             }

             return objs;
         }

         public static List<GameObject> ChildrenList(this GameObject from)
         {
             List<GameObject> children = new List<GameObject>();
             int count = 0;
             while (count < from.transform.childCount)
             {
                 children.Add(from.transform.GetChild(count).gameObject);
                 count++;
             }

             return children;
         }
     }
 
}
