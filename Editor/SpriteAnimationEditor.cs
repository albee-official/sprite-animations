using UnityEngine;

namespace SpriteAnimations.Inspector
{
    #if UNITY_EDITOR

    using UnityEditor;
    using System.Collections.Generic;

    [CustomEditor(typeof(SpriteAnimation))]
    public class SpriteAnimationEditor : Editor {
        protected static float FPS = 10;
        protected static int CurrentFrame;
        protected static bool ShouldPlay = true;
        protected readonly List<Sprite> Sprites = new List<Sprite>();
        protected SerializedProperty Frames;

        private readonly List<SerializedProperty> _propertiesToDraw = new List<SerializedProperty>();

        private void OnEnable() {
            RegisterProperty("_Name");
            RegisterProperty("fps");
            RegisterProperty("playbackMode");
            Frames = RegisterProperty("frames");
        }

        public override bool HasPreviewGUI() {
            return true;
        }

        public override bool RequiresConstantRepaint()
        {
            return FPS > 0 && ShouldPlay;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawCustomProperties();
            EditorGUILayout.Separator();
            UpdateSpritesCache();

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawPlaybackControls()
        {
            EditorGUILayout.BeginHorizontal();
            ShouldPlay = EditorGUILayout.ToggleLeft("Play", ShouldPlay, GUILayout.MaxWidth(100));
            if (ShouldPlay)
                FPS = EditorGUILayout.FloatField("Frames per seconds", FPS);
            else
                CurrentFrame = EditorGUILayout.IntSlider(CurrentFrame, 0, Sprites.Count - 1);
            EditorGUILayout.EndHorizontal();
        }

        public override void OnPreviewGUI(Rect position, GUIStyle background)
        {
            DrawPlaybackControls();

            if (Sprites.Count == 0) return;
            int index = ShouldPlay
                ? (int) (EditorApplication.timeSinceStartup * FPS % Sprites.Count)
                : CurrentFrame;

            DrawTexturePreview(position, Sprites[index]);
        }

        protected void DrawCustomProperties()
        {
            foreach (var property in _propertiesToDraw)
            {
                if (property != null)
                    EditorGUILayout.PropertyField(property);
            }
        }


        /// <summary>
        ///     Shamelessly stolen from <a href="https://github.com/aarthificial/reanimation/blob/master/Editor/Helpers.cs">Aarthificial</a> which was shamelessly stolen from <a href="https://forum.unity.com/threads/drawing-a-sprite-in-editor-window.419199/#post-3059891">Woofy</a> in the first place. 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="sprite"></param>
        public static void DrawTexturePreview(Rect position, Sprite sprite)
        {
            var fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            var size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            var coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            var center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
    
        protected SerializedProperty RegisterProperty(string propertyName)
        {
            var property = serializedObject.FindProperty(propertyName);
            _propertiesToDraw.Add(property);
            return property;
        }

        protected virtual void UpdateSpritesCache()
        {
            Sprites.Clear();
            for (var i = 0; i < Frames.arraySize; i++)
            {
                var frameProp = Frames.GetArrayElementAtIndex(i);
                var sprite = frameProp.FindPropertyRelative("sprite").objectReferenceValue as Sprite;
                if (sprite != null)
                    Sprites.Add(sprite);
            }
        }
    
    
    }

    #endif
}