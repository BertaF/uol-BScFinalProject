using Assets.Scripts.Audio;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The CheckpointManager class, keeps track of the most recent checkpoint.
    /// Processes screen fading, player teleport requests used on high falls.
    /// </summary>
    public class CheckpointManager : MonoBehaviour
    {
        #region Member Variables
        [Header("Checkpoint Variables")]
        [SerializeField] private float _screenFadeDuration;
        [SerializeField] private PlayerController_FSM _player;
        [SerializeField] private ScreenFader _screenFader;

        [Header("Checkpoint Game Events")]
        [SerializeField] private GameEvent _checkpointTeleport;
        [SerializeField] private GameEvent _reachedCheckpoint;
        [SerializeField] public GameEvent GameEnd;

        [Header("Checkpoint Audio Game Events")]
        [SerializeField] public GameEvent _audioTransitionPoint;

        private Transform _checkpoint;
        private bool _startedFadeSequence;
        #endregion

        /// <summary>
        /// Initialises the _screenFadeDuration variable to the fade duration defined in the ScreenFader class.
        /// </summary>
        private void Start()
        {
            if (!_screenFader || !_player) return;

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

            if (_startedFadeSequence) { return; }

            // Triger the teleport SFX.
            _checkpointTeleport?.Invoke();

            FadeSequence();

            _startedFadeSequence = false;
        }

        /// <summary>
        /// Sets the player transform (position and rotation) to be the same as the nearest checkpoint.
        /// </summary>
        /// <param name="targetEntity">Passes the player transform as parameter</param>
        public void TeleportToCheckpoint(Transform targetEntity)
        {
            targetEntity.SetPositionAndRotation(_checkpoint.position, Quaternion.identity);
        }

        /// <summary>
        /// Sequence that fades out the screen, teleports the player to the nearest checkpoint and fades the screen back in.
        /// </summary>
        public void FadeSequence()
        {
            _startedFadeSequence = true;

            _screenFader.FadeOut();

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
        public void SetCheckpoint(Transform newCheckpoint)
        {
            if (_checkpoint == newCheckpoint) return;

            // Play an audio SFX when reaching a checkpoint for the first time
            _reachedCheckpoint?.Invoke();

            // Cache the new checkpoint position
            _checkpoint = newCheckpoint;
        }
    }
}
