using UnityEngine;
using System;
using TypeReferences;
using SpriteAnimations.Actions;

namespace SpriteAnimations.Interfaces
{   
    /// <summary> Common interface representing any animation frame. </summary>
    public interface ISpriteAnimationFrame
    {
        /// <summary> Sprite that is displayed during this frame. </summary>
        public Sprite Sprite { get; }

        /// <summary> Action that is performed when frame is entered. </summary>
        public ESpriteAnimationActions Action { get; }

        /// <summary> Action Data Type: </summary>
        public TypeReference ActionDataType { get; }

        /// <summary> Object data of the frame </summary>
        public SpriteAnimationActionData ActionData { get; }

        /// <summary> Tries to cast action data to it's proper type </summary>
        public dynamic CastedActionData();
        
        // public event Action onEnter;
        // public event Action onLeave;
    }
}