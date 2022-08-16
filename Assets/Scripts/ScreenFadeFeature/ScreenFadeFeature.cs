using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.ScreenFadeFeature
{
    public class ScreenFadeFeature : ScriptableRendererFeature
    {
        public FadeSettings Settings;
        private ScreenFadePass _renderPass;

        public override void Create()
        {
            Debug.Log("ScreenFade - Creating screen fade pass render");

            // Create a new pass using the user settings.
            _renderPass = new ScreenFadePass(Settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (Settings.Valid())
            {
                Debug.Log("ScreenFade is valid");
                // Add the render pass to the render with the valid settings.
                renderer.EnqueuePass(_renderPass);
            }
        }
    }

    [Serializable]
    public class FadeSettings
    {
        public bool IsEnabled = true;
        public string ProfilerTag = "Screen Fade";

        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public Material Material = null;

        // The runtime material is a copy of the original material to avoid changing it by mistake.
        [NonSerialized] public Material RuntimeMaterial = null;

        public bool Valid()
        {
            // We need a valid material and the settings need to be enabled.
            return (RuntimeMaterial != null) && IsEnabled;
        }
    }
}
