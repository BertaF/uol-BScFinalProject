using UnityEngine;
using UnityEngine.Rendering.Universal;
using Pixelplacement;

namespace Assets.Scripts.ScreenFadeFeature
{
    public class ScreenFade : MonoBehaviour
    {
        // References
        public UniversalRendererData RendererData;

        // Settings
        [Range(0, 1)] public float Duration = 0.5f;

        private Material _fadeMaterial;

        private void Start()
        {
            Debug.Log("Inside ScreenFade - Renderer data: " + RendererData.name);
            // Accesses the render features in the render data and finds the screen fade feature.
            SetupFadeFeature();
        }

        private void SetupFadeFeature()
        {
            // Look for the screen fade feature
            ScriptableRendererFeature feature = RendererData.rendererFeatures.Find(item => item is ScreenFadeFeature);

            // Ensure the feature found is the correct one.
            if (feature is ScreenFadeFeature screenFade)
            {
                // Duplicate the material so we don't change the renderer's asset directly.
                _fadeMaterial = Instantiate(screenFade.Settings.Material);
                screenFade.Settings.RuntimeMaterial = _fadeMaterial;

                Debug.Log("ScreenFade - Found screen fade feature, with material: " + _fadeMaterial.name);
            }
        }

        public float FadeIn()
        {
            Debug.Log("ScreenFade - Calling Fade In");

            // Fade to black
            Tween.ShaderFloat(_fadeMaterial, "_Alpha", 1, Duration, 0);
            return Duration;
        }

        public float FadeOut()
        {
            // Fade to clear
            Tween.ShaderFloat(_fadeMaterial, "_Alpha", 0, Duration, 0);
            return Duration;
        }
    }
}
