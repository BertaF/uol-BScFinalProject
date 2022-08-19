using System.Collections;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The CheckpointManager class, keeps track of the most recent checkpoint.
    /// Processes screen fading and player teleport requests used on high falls.
    /// </summary>
    public class CheckpointManager : MonoBehaviour
    {
        #region Member Variables
        [SerializeField] private float _screenFadeDuration;
        [SerializeField] private PlayerController_FSM _player;
        [SerializeField] private ScreenFader _screenFader;

        private Transform _checkpoint;
        #endregion

        /// <summary>
        /// Initialises the _screenFadeDuration variable to the fade duration defined in the ScreenFader class.
        /// </summary>
        private void Start()
        {
            // If not yet initialised, use the duration directly from the screen fader class.
            if (_screenFadeDuration == 0.0f)
            {
                _screenFadeDuration = _screenFader.fadeDuration;
            }
        }

        /// <summary>
        /// If the player has fallen from a high platform, it starts the screen fading and player teleport sequence using a coroutine.
        /// </summary>
        private void FixedUpdate()
        {
            if (_checkpoint == null) { return; }

            if (!_player.HasFallen()) { return; }

            StartCoroutine(FadeSequence());
        }

        /// <summary>
        /// Sets the player transform (position and rotation) to be the sames as the nearest checkpoint.
        /// </summary>
        /// <param name="targetEntity">Passes the player transform as parameter</param>
        public void TeleportToCheckpoint(Transform targetEntity)
        {
            targetEntity.SetPositionAndRotation(_checkpoint.position, Quaternion.identity);
        }

        /// <summary>
        /// A coroutine sequence that fades out the screen, teleports the player to the nearest checkpoint and fades the screen back in.
        /// </summary>
        public IEnumerator FadeSequence()
        {
            _screenFader.FadeOut();
            
            yield return new WaitForSeconds(_screenFadeDuration);

            TeleportToCheckpoint(targetEntity: _player.transform);

            _screenFader.FadeIn();
        }

        /// <summary>
        /// Getter method for the last player checkpoint.
        /// </summary>
        public Transform GetCurrentCheckpoint() => _checkpoint;

        /// <summary>
        /// Setter method that updates the player checkpoint position.
        /// </summary>
        /// <param name="newCheckpoint">Passes the transform of the most recent checkpoint</param>
        public void SetCheckpoint(Transform newCheckpoint) => _checkpoint = newCheckpoint;
    }
}
