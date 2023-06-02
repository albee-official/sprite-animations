using UnityEngine;
using System;

namespace SpriteAnimations.Actions
{
    public interface ISpriteAnimationActionData { }

    [Serializable]
    public class DefaultActionData : ISpriteAnimationActionData {
        [SerializeField] public string Value = "";
    }

    [Serializable]
    public class IntegerActionData : ISpriteAnimationActionData {
        [SerializeField] public int Value = 0;
    }

    [Serializable]
    public class FloatActionData : ISpriteAnimationActionData {
        [SerializeField] public float Value = 0;
    }

    [Serializable]
    public class Range01ActionData : ISpriteAnimationActionData {
        [SerializeField] [Range(0f, 1f)] public float Value = 0;
    }
}