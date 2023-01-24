using UnityEngine;
using SpriteAnimations;
using SpriteAnimations.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SpriteAnimations.Inspector
{
    #if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(SpriteAnimationFrame))]
    public class SpriteAnimationFrameDrawer : PropertyDrawer
    {
        private const float Size = 80;
        private const float Padding = 12;
        private const float Spacing = 2;
        private const float Margin = 2;
        private const float SpriteHeight = 20;
        private const float ActionHeight = 20;
        private const float DataHeight = 20;
        protected virtual IEnumerable<string> PropertiesToDraw => new string[] { };
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float childPropertiesHeight = PropertiesToDraw.Sum(
                name => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name))
            );
            return Mathf.Max(childPropertiesHeight + Spacing * 6, Size) + Margin * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.y += Margin;
            position = DrawPreview(position, property);

            SerializedProperty spriteProp = property.FindPropertyRelative("sprite");
            SerializedProperty actionProp = property.FindPropertyRelative("action");
            SerializedProperty dataProp = property.FindPropertyRelative("data");

            Rect spritePos = DrawSpriteProp(position, spriteProp);
            DrawActionProp(spritePos, actionProp, dataProp);
            

            EditorGUI.EndProperty();
        }

        protected virtual Rect DrawSpriteProp(Rect position, SerializedProperty property)
        {
            Rect pos = position;
            pos.y += Padding;
            pos.height = SpriteHeight;
            EditorGUI.PropertyField(pos, property);

            return pos;
        }

        protected virtual Rect DrawActionProp(Rect position, SerializedProperty actionProp, SerializedProperty dataProp)
        {
            Rect pos = position;
            pos.y += Spacing + SpriteHeight;
            pos.height = ActionHeight;
            EditorGUI.PropertyField(pos, actionProp);

            if (actionProp.enumValueIndex == 2) {
                pos.y += Spacing + ActionHeight;
                pos.height = DataHeight;
                EditorGUI.PropertyField(pos, dataProp);
            }

            return pos;
        }

        protected virtual Rect DrawPreview(Rect position, SerializedProperty property)
        {
            return DrawSpritePreview(position, property, "sprite");
        }

        protected Rect DrawSpritePreview(Rect position, SerializedProperty property, string name)
        {
            var spriteProp = property.FindPropertyRelative(name);
            var sprite = spriteProp.objectReferenceValue as Sprite;
            if (sprite == null || position.width < 300) return position;
            var spritePosition = position;
            spritePosition.width = Size - 2 * Padding;
            spritePosition.height = Size - 2 * Padding;
            position.width -= Size;
            spritePosition.x += position.width + Padding;
            spritePosition.y += Padding;
            SpriteAnimationEditor.DrawTexturePreview(spritePosition, sprite);
            return position;
        }
    }

    #endif
}