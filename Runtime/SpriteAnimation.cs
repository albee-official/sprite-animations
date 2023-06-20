using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SpriteAnimations.Interfaces;

namespace SpriteAnimations
{   
    /// <summary> Different sprite animation behaviours </summary>
    public enum ESpriteAnimationPlaybackModes {

        /// <summary> Play animation one time. </summary>
        SINGLE,

        /// <summary> Play animation indefinetely, until Skip() or Stop() methods are called. </summary>
        LOOP,
    }

    /// <summary> Basic sprite animations class. Has custom editor, so I wouldn't recommend changing it without a concrete reason. </summary>
    [CreateAssetMenu(fileName = "New Sprite Animation", menuName = "SpriteAnimations/Blank", order = 0)]
    public class SpriteAnimation : ScriptableObject, ISpriteAnimation {
        
        [field: Header("Animation Info")]
        [field: Space(6f)] [SerializeField] protected string _Name = "Unknown Animation";
        [field: Space(6f)] [SerializeField] protected bool pointingRight = false;
        [field: Space(2f)] [SerializeField] [Range(1, 60)] protected int fps = 10;
        [field: Space(2f)] [SerializeField] protected ESpriteAnimationPlaybackModes playbackMode = ESpriteAnimationPlaybackModes.SINGLE;


        [field: Space(8f)]
        [field: Header("Animation Properties")]
        [field: Space(6f)] [SerializeField] protected SpriteAnimationFrame[] frames;

        private int _currentFrameIndex = 0;

        #region Properties

        public string Name { get { return _Name; } }
        public int CurrentFrameIndex { get { return _currentFrameIndex; } }
        public bool PointingRight { get { return pointingRight; } }
        public int FPS { get { return fps; } }
        public ESpriteAnimationPlaybackModes PlaybackMode { get { return playbackMode; } }
        public ISpriteAnimationFrame[] Frames { get { return frames; } }

        public ISpriteAnimationFrame AnimationFrame {
            get {
                if (frames.Length == 0) {
                    Debug.LogWarning("Animation [" + Name + "] has no frames!");
                    return null;
                }

                return frames[CurrentFrameIndex];
            }
        }

        #endregion
        
        public ISpriteAnimationFrame RequestFrame() {
            if (frames.Length == 0) return null;

            int i = CurrentFrameIndex;

            switch (playbackMode) {
                case ESpriteAnimationPlaybackModes.SINGLE:
                    _currentFrameIndex = CurrentFrameIndex + 1;
                    if (_currentFrameIndex > frames.Length) {
                        Reset();

                        return null;
                    }

                    break;
                
                case ESpriteAnimationPlaybackModes.LOOP:
                    Advance();
                    
                    break;
            }

            return frames[i];
        }

        public void Advance() {
            if (frames.Length == 0) return;

            _currentFrameIndex = (CurrentFrameIndex + 1) % frames.Length;
        }

        public void Reset() {
            _currentFrameIndex = 0;
        }
        
        public SpriteAnimation NewInstance() {
            SpriteAnimation newAnimation = Instantiate(this);

            return newAnimation;
        }

        public void Init(string name) {
            _Name = name;
        }

        public void AddNewFrame(Sprite sprite, ESpriteAnimationActions action = ESpriteAnimationActions.NONE) {
            if (sprite == null) return;

            List<SpriteAnimationFrame> framesList = (frames != null) ? (new List<SpriteAnimationFrame>(frames)) : (new List<SpriteAnimationFrame>());

            framesList.Add(new SpriteAnimationFrame(sprite, action));

            frames = framesList.ToArray();
        }

        public void ResetAnimationFrames() {
            _currentFrameIndex = 0;
            frames = new SpriteAnimationFrame[0];
        }

        public void SetAnimationFrames(ISpriteAnimationFrame[] frames) {
            _currentFrameIndex = 0;
            var fr = new SpriteAnimationFrame[frames.Length];
            for (int i = 0; i < frames.Length; i++) {
                fr[i] = (SpriteAnimationFrame)frames[i];
            }

            this.frames = fr;
        }
    }
}