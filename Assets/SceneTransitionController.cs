using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public ScreenFader fadeScreen;

    public void SwitchScene(int iScene)
    {
        StartCoroutine(SwitchSceneRoutine(iScene));
    }

    private IEnumerator SwitchSceneRoutine(int iScene)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        // Switch to a new scene.
        SceneManager.LoadScene(iScene);
    }
}
