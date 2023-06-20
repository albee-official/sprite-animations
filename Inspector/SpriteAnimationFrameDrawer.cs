using UnityEngine;
using SpriteAnimations;
using SpriteAnimations.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TypeReferences;
using System.Reflection;
using System;
using SpriteAnimations.Actions;

namespace SpriteAnimations.Inspector
{
    #if UNITY_EDITOR

    using UnityEditor;

    [CustomPropertyDrawer(typeof(SpriteAnimationFrame))]
    public class SpriteAnimationFrameDrawer : PropertyDrawer
    {
        private const float SIZE = 96;
        private const float PADDING = 8;
        private const float SPACING = 2;
        private const float MARGIN = 4;
        private const float LEFT_PROPERTY_HEIGHT = 20;

        protected virtual IEnumerable<string> PropertiesToDraw => new string[] { };
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float sz = EditorGUI.GetPropertyHeight(property, includeChildren: true);
            
            sz += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("sprite"));
            sz += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("action"));
            sz += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("actionDataType"));
            sz += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("data"), includeChildren: true);

            // return Mathf.Max(childPropertiesHeight + SPACING * 6, SIZE) + MARGIN * 2;
            return Math.Max(sz, SIZE) + PADDING * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.y += MARGIN;
            position = DrawPreview(position, property);

            SerializedProperty spriteProp = property.FindPropertyRelative("sprite");
            SerializedProperty actionProp = property.FindPropertyRelative("action");
            SerializedProperty actionDataTypeProp = property.FindPropertyRelative("actionDataType");
            SerializedProperty dataProp = property.FindPropertyRelative("data");

            TypeReference typeRef = actionDataTypeProp.managedReferenceValue as TypeReference;
            SpriteAnimationActionData actionData = dataProp.managedReferenceValue as SpriteAnimationActionData;

            if (typeRef == null || typeRef.ToString() == "None") actionDataTypeProp.managedReferenceValue = new TypeReference(typeof(SpriteAnimations.Actions.DefaultActionData));
            if (actionData == null || actionData.GetType() != typeRef.Type) {
                dataProp.managedReferenceValue = Activator.CreateInstance(typeRef);
                dataProp = property.FindPropertyRelative("data");
            }

            Rect spritePos = DrawSpriteProp(position, spriteProp);
            DrawActionProp(spritePos, actionProp, actionDataTypeProp, dataProp);

            EditorGUI.EndProperty();
        }

        protected virtual Rect DrawSpriteProp(Rect position, SerializedProperty property)
        {
            Rect pos = position;
            pos.y += PADDING;
            pos.height = LEFT_PROPERTY_HEIGHT;
            EditorGUI.PropertyField(pos, property);

            return pos;
        }

        protected virtual Rect DrawActionProp(Rect position, SerializedProperty actionProp, SerializedProperty actionDataTypeProp, SerializedProperty dataProp)
        {
            Rect pos = position;
            pos.y += SPACING + LEFT_PROPERTY_HEIGHT;
            pos.height = LEFT_PROPERTY_HEIGHT;
            EditorGUI.PropertyField(pos, actionProp);

            if (actionProp.enumValueIndex == 2) {
                // try displaying them but we can't know for sure they will
                try {
                    pos.y += SPACING + LEFT_PROPERTY_HEIGHT;
                    pos.height = LEFT_PROPERTY_HEIGHT;
                    EditorGUI.PropertyField(pos, actionDataTypeProp, GUIContent.none);

                    pos.y += SPACING + LEFT_PROPERTY_HEIGHT;
                    pos.height = LEFT_PROPERTY_HEIGHT;
                    EditorGUI.PropertyField(pos, dataProp, includeChildren: true);

                }
                catch (System.Exception) { }
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
            spritePosition.width = SIZE - 2 * PADDING;
            spritePosition.height = SIZE - 2 * PADDING;
            position.width -= SIZE;
            spritePosition.x += position.width + PADDING;
            spritePosition.y += PADDING;
            SpriteAnimationEditor.DrawTexturePreview(spritePosition, sprite);
            return position;
        }
    }

    #endif
}