using System;
using System.Collections.Generic;
using SpriteAnimations.Interfaces;
using System.Reflection;
using UnityEngine;
using TypeReferences;
using SpriteAnimations.Actions;
using Painkiller;

namespace SpriteAnimations
{
    public enum ESpriteAnimationActions
    {
        /// <summary> Do nothing. </summary>
        NONE,

        /// <summary> Invoke onAnimationEnter event on animator. </summary>
        ENTER,

        /// <summary> Invoke onAnimationAction event on animator and pass some data to it. </summary>
        ACTION,

        /// <summary> Invoke onAnimationLeave event on animator. </summary>
        LEAVE,
    }

    /// <summary> Basic sprite animation frame. Has custom PropertyDrawer, so I wouldn't recommend using something other without concrete reason. </summary>
    [Serializable]
    public class SpriteAnimationFrame : ISpriteAnimationFrame
    {
        [field: Space(2f)] [SerializeField] protected Sprite sprite;
        [field: Space(2f)] [SerializeField] protected ESpriteAnimationActions action = ESpriteAnimationActions.NONE;

        [Inherits(typeof(SpriteAnimationActionData))]
        [field: Space(2f)] [SerializeReference] protected TypeReference actionDataType = new TypeReference(typeof(DefaultActionData));
        [field: Space(2f)] [SerializeReference] protected SpriteAnimationActionData data = null;

        public Sprite Sprite { get => sprite; }
        public ESpriteAnimationActions Action { get => action; }
        public TypeReference ActionDataType { get => actionDataType; }
        public SpriteAnimationActionData ActionData { get => data; }

        public SpriteAnimationFrame() { }

        public SpriteAnimationFrame(Sprite sprite, ESpriteAnimationActions action = ESpriteAnimationActions.NONE) {
            this.sprite = sprite;
            this.action = action;
            this.data = (DefaultActionData)Activator.CreateInstance(actionDataType);
        }

        public dynamic CastedActionData() {
            return Convert.ChangeType(data, actionDataType);
        }

        // [field: Space(2f)] [SerializeField] public event Action onEnter;
        // [field: Space(2f)] [SerializeField] public event Action onLeave;

        // public void Enter() {
        //     onEnter?.Invoke();
        // }

        // public void Leave() {
        //     onLeave?.Invoke();
        // }
    }
}
