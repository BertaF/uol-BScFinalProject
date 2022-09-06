using System.Collections;
using Unity.VisualScripting;
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
        private GameObject _helpMenu;
        private GameObject _mainMenu;
        #endregion

        /// <summary>
        /// Initialises the help and main menu variables. Disables the help meny by default.
        /// </summary>
        private void Awake()
        {
            _helpMenu = GameObject.FindGameObjectWithTag("HelpMenu");
            _mainMenu = GameObject.FindGameObjectWithTag("MainMenu");

            // Disable the help menu by default
            if (_helpMenu)
            {
                _helpMenu.SetActive(false);
            }
        }

        /// <summary>
        /// Starts the switch sequence coroutine that fades out the screen and loads a given scene.
        /// </summary>
        /// <param name="iScene">Passes the scene index as parameter</param>
        public void SwitchScene(int iScene)
        {
            StartCoroutine(SwitchSceneRoutine(iScene));
        }

        /// <summary>
        /// Enables or disables the help menu. Also disables the main menu if the helper is active, and vice-versa
        /// </summary>
        /// <param name="shouldEnable">A boolean is passed to decide if the help menu object should be enabled/disabled.</param>
        public void ToggleHelpMenu(bool shouldEnable)
        {
            if (!_helpMenu || !_mainMenu) return;

            if (_helpMenu.activeSelf == shouldEnable || _mainMenu.activeSelf != shouldEnable) return;

            _helpMenu.SetActive(shouldEnable);
            _mainMenu.SetActive(!shouldEnable);
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
