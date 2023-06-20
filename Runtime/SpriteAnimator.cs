using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using CommonUtils.Serialization;
using CommonUtils.Attributes;
using SpriteAnimations.Interfaces;

namespace SpriteAnimations
{
    /// <summary> Different modes for animator speed behaviour. </summary>
    public enum ESpriteAnimatorPlaybackModes {
        /// <summary> Uses constant FPS of the animator </summary>
        CONSTANT_FPS,

        /// <summary> Uses FPS provided by the animation </summary>
        DEFAULT,
    }

    /// <summary> Different modes for Play(). </summary>
    public enum ESpriteAnimatorPlayMode {
        /// <summary> Waits for current animation frame, then starts playing new animation. </summary>
        DEFAULT,

        /// <summary> Plays animation, if animator isn't busy. Doesn't change if the same animation is played already. </summary>
        PRESERVE,

        /// <summary> Plays animation instantly, overwriting current one. </summary>
        OVERWRITE,
    }


    /// <summary> Basic sprite animator. Can Play(), Stop(), Queue() etc. different SpriteAnimations. </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        //. Fields
        [field: Header("References")]
        [field: Space(6f)] [SerializeField] [ReadOnly] protected SpriteRenderer spriteRenderer;
        [field: Space(6f)] [SerializeField] [ReadOnly] protected Sprite defaultSprite;


        [field: Space(8f)]
        [field: Header("Settings")]
        [field: Space(6f)] [SerializeField] [Range(1, 60)] protected int defaultAnimationFPS = 10;
        [field: Space(2f)] [SerializeField] protected ESpriteAnimatorPlaybackModes AnimatorPlaybackModes = ESpriteAnimatorPlaybackModes.DEFAULT;


        [field: Space(8f)]
        [field: Header("Runtime Properties")]
        [field: Space(6f)] [SerializeField] [Range(.1f, 10f)] protected float animatorSpeedModifier = 1f;
        [field: Space(2f)] [SerializeField] public bool isPaused = false;
        [field: Space(2f)] [SerializeField] protected SpriteAnimation currentAnimation;
        [field: Space(2f)] [SerializeField] protected List<SpriteAnimation> animationQueue;


        [field: Space(24f)]
        [field: Header("Debug Info")]
        [field: Space(6f)] [SerializeField] [ReadOnly] private bool m_isPlayingAnimation = false;
        [field: Space(2f)] [SerializeField] [ReadOnly] private float m_lastSpriteUpdateTime = float.MinValue;
        [field: Space(2f)] [SerializeField] [ReadOnly] private float m_frameCheckInterval = 0;
        [field: Space(2f)] [SerializeField] [ReadOnly] private float m_nextFrameDuration = 0;
        [field: Space(2f)] [SerializeField] [ReadOnly] private SpriteAnimationFrame m_currentAnimationFrame;


        //. Properties
        /// <summary> Current animator FPS. </summary>
        public float AnimatorFPS {
            get {
                switch (AnimatorPlaybackModes)
                {
                    case ESpriteAnimatorPlaybackModes.CONSTANT_FPS:
                        return defaultAnimationFPS;

                    default: // or ESpriteAnimatorPlaybackModes.DEFAULT
                        if (currentAnimation == null) return defaultAnimationFPS;
                        return currentAnimation.FPS;
                }
            }
        }

        /// <summary> Current animator speed modifier. </summary>
        public float AnimatorSpeedModifier {
            get {
                return animatorSpeedModifier;
            }

            set {
                if (animatorSpeedModifier != value) RecalculateFrameCheckInterval();

                animatorSpeedModifier = value;
            }
        }

        /// <summary> Whether the animator is currently playing any animation. </summary>
        public bool isPlayingAnimation {
            get {
                return currentAnimation != null;
            }
        }

        /// <summary> Current animation frame, played by animator. </summary>
        public ISpriteAnimationFrame CurrentAnimationFrame {
            get {
                return m_currentAnimationFrame;
            }
        }

        /// <summary> Animations waiting in queue. Lower = closer. </summary>
        public ISpriteAnimation[] AnimationQueue {
            get {
                return animationQueue.ToArray();
            }
        }

        /// <summary> SpriteRenderer object that is displaying animation frames. </summary>
        public SpriteRenderer AnimationTarget {
            get {
                return spriteRenderer;
            }
        }


        // - Unity Methods
        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            defaultSprite = spriteRenderer.sprite;

            RecalculateFrameCheckInterval();
        }

        private void Start() {
            m_nextFrameDuration = m_frameCheckInterval;
        }

        private void Update() {
            if (isPaused || !m_isPlayingAnimation) return;

            if (Time.time > m_lastSpriteUpdateTime + m_nextFrameDuration) {
                m_isPlayingAnimation = NextFrame();
            }
        }


        // - Class Methods
        /// <summary> Attempts to get next animation frame. Returns whether it played an animation frame. </summary>
        public bool NextFrame() {
            if (currentAnimation == null) {
                currentAnimation = RequestAnimationFromQueue();

                if (currentAnimation == null) {
                    spriteRenderer.sprite = defaultSprite;
                    return false;
                }
            }

            m_currentAnimationFrame = (SpriteAnimationFrame)currentAnimation.RequestFrame();
            if (m_currentAnimationFrame == null) {
                currentAnimation = null;
                return NextFrame();
            }

            spriteRenderer.sprite = m_currentAnimationFrame.Sprite;
            
            m_lastSpriteUpdateTime = Time.time;
            m_nextFrameDuration = RecalculateFrameCheckInterval();
            
            bool handled = HandleAnimationAction(m_currentAnimationFrame);
            
            return handled;
        }

        /// <summary> Plays provided animation. </summary>
        /// <remarks> -<br />
        ///     Mode: <br />
        ///         Default - Plays animation after current frame has passed. <br />
        ///         Overwrite - Plays animation instantly, without waiting for current frame.
        ///  </remarks>
        public bool Play(ISpriteAnimation animation = null, ESpriteAnimatorPlayMode mode = ESpriteAnimatorPlayMode.DEFAULT) {
            isPaused = false;
            m_isPlayingAnimation = true;
            
            if (animation == null) {
                return false;
            }
            SpriteAnimation anim;
            switch (mode) {
                case ESpriteAnimatorPlayMode.DEFAULT:
                    anim = (SpriteAnimation)animation;
                    currentAnimation = anim.NewInstance();

                    currentAnimation.Reset();

                    return true;
                
                case ESpriteAnimatorPlayMode.PRESERVE:
                    if (currentAnimation != null && currentAnimation.Name == animation.Name) return true;

                    anim = (SpriteAnimation)animation;
                    currentAnimation = anim.NewInstance();

                    return true;

                case ESpriteAnimatorPlayMode.OVERWRITE:
                    anim = (SpriteAnimation)animation;

                    Queue(anim.NewInstance());

                    m_isPlayingAnimation = NextFrame();
                    return true;
            }
            
            return true;
        }

        /// <summary> Adds new animation to the queue. </summary>
        public bool Queue(ISpriteAnimation animation) {
            if (animation == null) return false;

            animationQueue.Add((SpriteAnimation)animation);

            return true;
        }
        
        /// <summary> Skips to the next animation in queue </summary>
        public bool Skip() {
            currentAnimation = null;
            m_isPlayingAnimation = NextFrame();

            return true;
        }

        /// <summary> Stops the animator and clears the queue. </summary>
        public bool Stop() {
            currentAnimation = null;
            animationQueue.Clear();

            return true;
        }

        private SpriteAnimation RequestAnimationFromQueue() {
            if (animationQueue.Count == 0) return null;

            SpriteAnimation anim = animationQueue[0];
            animationQueue.RemoveAt(0);

            return anim;
        }

        private bool HandleAnimationAction(ISpriteAnimationFrame frame) {
            if (frame == null) return false;

            switch (frame.Action) {
                case ESpriteAnimationActions.NONE: return true;

                case ESpriteAnimationActions.ENTER:
                    onAnimationEnter?.Invoke(currentAnimation, frame);
                    return true;

                case ESpriteAnimationActions.LEAVE:
                    onAnimationLeave?.Invoke(currentAnimation, frame);
                    return true;

                case ESpriteAnimationActions.ACTION:
                    onAnimationAction?.Invoke(currentAnimation, frame, frame.ActionDataType, frame.ActionData);
                    return true;
            }
            
            return true;
        }


        // - Utility Methods
        protected float RecalculateFrameCheckInterval() {
            m_frameCheckInterval = 1 / (AnimatorFPS * animatorSpeedModifier);
            return m_frameCheckInterval;
        }


        // - Events
        public delegate void SpriteAnimationEventHandler(ISpriteAnimation animation, ISpriteAnimationFrame frame);
        public delegate void SpriteAnimationActionHandler(ISpriteAnimation animation, ISpriteAnimationFrame frame, Type dataType, object data);

        public event SpriteAnimationEventHandler onAnimationEnter;
        public event SpriteAnimationEventHandler onAnimationLeave;
        public event SpriteAnimationActionHandler onAnimationAction;



        #if UNITY_EDITOR

        private void OnValidate() {
            Awake();
        }

        #endif
    }
}
