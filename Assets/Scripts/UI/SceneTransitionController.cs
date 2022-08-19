using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The SceneTransitionController class, is responsible for the processes and requests required to switch between scenes in the game level.
    /// </summary>
    public class SceneTransitionController : MonoBehaviour
    {
        #region Member Variables
        [SerializeField] private ScreenFader _fadeScreen;
        #endregion

        /// <summary>
        /// Starts the switch sequence coroutine that fades out the screen and loads a given scene.
        /// </summary>
        /// <param name="iScene">Passes the scene index as parameter</param>
        public void SwitchScene(int iScene)
        {
            StartCoroutine(SwitchSceneRoutine(iScene));
        }

        /// <summary>
        /// The switch scene routine performs a screen fade out for the number of seconds given.
        /// After the fade out, it request a change of scene using the given scene index.
        /// </summary>
        /// <param name="iScene">Passes the scene index as parameter</param>
        private IEnumerator SwitchSceneRoutine(int iScene)
        {
            _fadeScreen.FadeOut();
            yield return new WaitForSeconds(_fadeScreen.fadeDuration);

            // Switch to a new scene.
            SceneManager.LoadScene(iScene);
        }
    }
}
