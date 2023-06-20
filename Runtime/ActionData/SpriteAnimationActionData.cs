using UnityEngine;
using System;

namespace SpriteAnimations.Actions
{
    [Serializable]
    public abstract class SpriteAnimationActionData { }

    [Serializable]
    public class DefaultActionData : SpriteAnimationActionData {
        [SerializeField] public string Value = "";
    }

    [Serializable]
    public class IntegerActionData : SpriteAnimationActionData {
        [SerializeField] public int Value = 0;
    }

    [Serializable]
    public class FloatActionData : SpriteAnimationActionData {
        [SerializeField] public float Value = 0;
    }

    [Serializable]
    public class Range01ActionData : SpriteAnimationActionData {
        [SerializeField] [Range(0f, 1f)] public float Value = 0;
    }
}