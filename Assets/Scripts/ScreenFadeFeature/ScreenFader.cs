using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenFader : MonoBehaviour
{
    public bool fadeOnStart = true;

    public float fadeDuration = 2.0f;

    public Color fadeColourFrom;
    public Color fadeColourTo;

    private Renderer renderer;

    private UnityEngine.Rendering.Universal.ColorAdjustments colourAdjustments;

    // Start is called before the first frame update
    private void Start()
    {
        renderer = GetComponent<Renderer>();

        VolumeProfile volumeProfile = GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
        if (!volumeProfile.TryGet(out colourAdjustments)) throw new System.NullReferenceException(nameof(colourAdjustments));

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

    // Update is called once per frame
    public void Fade(float alphaIn, float alphaOut, Color firstColour, Color secondColor)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut, firstColour, secondColor));
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut, Color firstColour, Color secondColour)
    {
        float timer = 0.0f;

        while (timer <= fadeDuration)
        {
            Color newColour = fadeColourFrom;
            newColour.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);
            renderer.material.SetColor("_Color", newColour);

            Color lerpedColor = Color.Lerp(firstColour, secondColour, Mathf.PingPong(timer, fadeDuration));
            colourAdjustments.colorFilter.Override(lerpedColor);

            timer += Time.deltaTime;
            yield return null;
        }

        Color newColour2 = fadeColourFrom;
        newColour2.a = alphaOut;
        renderer.material.SetColor("_Color", newColour2);

        colourAdjustments.colorFilter.Override(secondColour);
    }
}
