using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.UI
{
    public class ScreenFader : MonoBehaviour
    {
        public bool fadeOnStart = true;
        public float fadeDuration = 2.0f;
        public Color fadeColourFrom;
        public Color fadeColourTo;
        private UnityEngine.Rendering.Universal.ColorAdjustments colourAdjustments;

        private void Start()
        {
            VolumeProfile volumeProfile = GetComponent<UnityEngine.Rendering.Volume>()?.profile;
            if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
            if (!volumeProfile.TryGet(out colourAdjustments))
                throw new System.NullReferenceException(nameof(colourAdjustments));

            if (fadeOnStart)
            {
                FadeIn();
            }
        }

        public void FadeIn()
        {
            Fade(1.0f, 0.0f, fadeColourFrom, fadeColourTo);
        }

        public void FadeOut()
        {
            Fade(0.0f, 1.0f, fadeColourTo, fadeColourFrom);
        }

        public void Fade(float alphaIn, float alphaOut, Color firstColour, Color secondColor)
        {
            StartCoroutine(FadeRoutine(alphaIn, alphaOut, firstColour, secondColor));
        }

        public IEnumerator FadeRoutine(float alphaIn, float alphaOut, Color firstColour, Color secondColour)
        {
            float timer = 0.0f;

            while (timer <= fadeDuration)
            {
                Color lerpedColor = Color.Lerp(firstColour, secondColour, Mathf.PingPong(timer, fadeDuration));
                colourAdjustments.colorFilter.Override(lerpedColor);

                timer += Time.deltaTime;
                yield return null;
            }

            colourAdjustments.colorFilter.Override(secondColour);
        }
    }
}
