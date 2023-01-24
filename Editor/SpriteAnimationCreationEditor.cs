using UnityEngine;
using System.Reflection;
using System;
using System.IO;
using CommonUtils.Debug;
using System.Collections.Generic;

namespace SpriteAnimations.Inspector
{
    #if UNITY_EDITOR

    using UnityEditor;

    public class SpriteAnimationHelper : EditorWindow
    {
        // [MenuItem("Assets/Create/SpriteAnimations", false, 20)]
        [MenuItem("Assets/Create/SpriteAnimations/From Sprites", false, 0)]
        static void CreateAnimation(MenuCommand command) {

            // Grab all asset guids that are currently selected. quit if there are none.
            string[] assetGUIDS = Selection.assetGUIDs;
            if (assetGUIDS.Length == 0) {
                GameConsole.Warn("You must select at least one sprite to create an animation!");

                return;
            }

            List<Sprite> sprites = new List<Sprite>();
            string name = "";

            // Get the animation clip asset from unity asset database
            foreach (var guid in assetGUIDS)
            {
                string selectedAnimationClipPath = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(selectedAnimationClipPath);

                foreach (var obj in allAssets)
                {
                    if (obj == null || obj.GetType() != typeof(Sprite)) {
                        GameConsole.Warn("Only sprites get added to the animation!");
                        
                        continue;
                    }

                    sprites.Add((Sprite)obj);   

                    name = obj.name.Split("_")[0];
                }
            }

            if (sprites.Count == 0) {
                GameConsole.Warn("No sprites selected. Aborting animation creation.");

                return;
            }

            CreateSpriteAnimationForSprites(name, sprites);
        }

        private static void CreateSpriteAnimationForSprites(string name, List<Sprite> sprites) {
        
            // Instantiate new sprite animation scriptable object asset (but we don't create the file yet)
            SpriteAnimation SpriteAnimationInstance = ScriptableObject.CreateInstance<SpriteAnimation>();

            // Get folder in which we are currently located
            string path = GetCurrentFolder();
            GameConsole.Log("Creating animation at: " + path);

            // Get name of the folder we are currently located in
            string folderName = Path.GetFileNameWithoutExtension(path);

            // Create the ScriptableObject asset and initialize it (fill in the fields).
            string finalPath = name + ".spriteanim" + ".asset";
            ProjectWindowUtil.CreateAsset(SpriteAnimationInstance, finalPath);

            SpriteAnimationInstance.Init(name: name);

            for (int i = 0; i < sprites.Count; i++) {
                ESpriteAnimationActions action = ESpriteAnimationActions.NONE;
                if (i == 0) action = ESpriteAnimationActions.ENTER;
                else if (i == sprites.Count - 1) action = ESpriteAnimationActions.LEAVE;

                SpriteAnimationInstance.AddNewFrame(sprites[i], action);
            }

            GameConsole.Log("Succesfully created new Sprite Animation at: " + finalPath);
        }

        private static string GetCurrentFolder() {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);

            // We use reflection to grab a private field, since unity is cringe
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();

            return pathToCurrentFolder;
        }



        public static string UpperCaseFirstChar(string s) {
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return new string(a);
        }
    }

    #endif
}