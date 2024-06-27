using UnityEngine;

namespace ProbabilityWheel.Imports.PickerWheel.Scripts
{
    [System.Serializable]
    public class WheelPiece
    {
        public UnityEngine.Sprite Icon;
        public string Label;

        [Tooltip("Reward amount")] public int Amount;
        [Tooltip("Probability in %")] [Range(0f, 100f)]
        public float Chance = 100f;

        [HideInInspector] public int Index;
        [HideInInspector] public double Weight = 0f;
    }
}