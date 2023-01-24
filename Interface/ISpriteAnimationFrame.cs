using UnityEngine;
using System;

namespace SpriteAnimations.Interfaces
{   
    /// <summary> Common interface representing any animation frame. </summary>
    public interface ISpriteAnimationFrame
    {
        /// <summary> Sprite that is displayed during this frame. </summary>
        public Sprite Sprite { get; }

        /// <summary> Action that is performed when frame is entered. </summary>
        public ESpriteAnimationActions Action { get; }

        /// <summary> Custom data to pass with onAnimationAction event. </summary>
        public string ActionData { get; }
        
        // public event Action onEnter;
        // public event Action onLeave;
    }
}