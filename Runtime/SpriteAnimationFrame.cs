using System;
using System.Collections.Generic;
using SpriteAnimations.Interfaces;
using SpriteAnimations.Inspector;
using UnityEngine;

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
        [field: Space(2f)] [SerializeField] protected ESpriteAnimationActions action;
        [field: Space(2f)] [SerializeField] protected string data;

        public Sprite Sprite { get { return sprite; } }
        public ESpriteAnimationActions Action { get { return action; } }
        public string ActionData { get { return data; } }

        public SpriteAnimationFrame() {
        }

        public SpriteAnimationFrame(Sprite sprite, ESpriteAnimationActions action = ESpriteAnimationActions.NONE) {
            this.sprite = sprite;
            this.action = action;
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
