using UnityEngine;

namespace ProbabilityWheel.GameModule.LogicModule
{
    public class WheelSceneStartup : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}