using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace SpriteAnimations.Interfaces
{
    /// <summary> Common interface for sprite animations. SpriteAnimator can use any object, that is implementing this interface. </summary>
    public interface ISpriteAnimation {
        
        /// <summary> Name of the animation. </summary>
        /// <remarks> *used to compare animations within animator while using PRESERVE playmode </remarks>
        public string Name { get; }

        /// <summary> Animation frames. Contain a sprite and an action for each animation. </summary>
        public ISpriteAnimationFrame[] Frames { get; }

        /// <summary> Current Animation frame. </summary>
        public ISpriteAnimationFrame AnimationFrame { get; }

        /// <summary> Index of the current frame. </summary>
        public int CurrentFrameIndex { get; }

        /// <summary> FPS determines how much frames per second should animator play. </summary>
        /// <remarks> *can be overwritten by CONSTANT_FPS play mode. </remarks>
        public int FPS { get; }

        /// <summary> Determines whether an animation should loop or not. </summary>
        public ESpriteAnimationPlaybackModes PlaybackMode { get; }


        /// <summary> Used by animator to get next frame. </summary>
        public ISpriteAnimationFrame RequestFrame();

        /// <summary> Advances the animation to next frame. </summary>
        public void Advance();

        /// <summary> Resets animation to default state. </summary>
        public void Reset();
    }
}