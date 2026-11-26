using UnityEngine;

namespace GameTime
{
    public class GameTimeVisualsManager : MonoBehaviour
    {
        [Header("Sun")]
        [SerializeField] private Gradient sunGradient;
        private float sunIntensity;

        [Header("Fog & Ambient")]
        [SerializeField] private Gradient ambientGradient;
        [SerializeField] private Gradient fogGradient;

        private void Awake()
        {
            if(RenderSettings.sun != null)
                sunIntensity = RenderSettings.sun.intensity;
        }

        private void Update()
        {
            if (GameTimeManager.Instance == null)
                return;

            float progress = GameTimeManager.Instance.GetInGameTimeProgress01();

            if (RenderSettings.sun != null)
            {
                Color sunColor = sunGradient.Evaluate(progress);
                RenderSettings.sun.color = sunColor;
                RenderSettings.sun.intensity = sunIntensity * sunColor.a;
            }

            RenderSettings.ambientLight = ambientGradient.Evaluate(progress);
            RenderSettings.fogColor = fogGradient.Evaluate(progress);
    }
    }
}