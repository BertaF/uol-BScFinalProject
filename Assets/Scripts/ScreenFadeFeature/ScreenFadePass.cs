using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.ScreenFadeFeature
{
    public class ScreenFadePass : ScriptableRenderPass
    {
        private FadeSettings _settings;

        public ScreenFadePass(FadeSettings newSettings)
        {
            _settings = newSettings;
            renderPassEvent = newSettings.RenderPassEvent;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Set command to store instructions
            CommandBuffer command = CommandBufferPool.Get(_settings.ProfilerTag);

            // Get the locations of our textures
            RenderTargetIdentifier source = BuiltinRenderTextureType.CameraTarget;
            RenderTargetIdentifier destination = BuiltinRenderTextureType.CurrentActive;

            // Copy texture info with added material
            command.Blit(source, destination, _settings.RuntimeMaterial);
            context.ExecuteCommandBuffer(command);

            // Release command
            CommandBufferPool.Release(command);
        }
    }
}

