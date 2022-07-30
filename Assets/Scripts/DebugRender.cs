using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class DebugRender : MonoBehaviour
    {
        #region Member Variables
        private PlayerController_FSM _playerFsm;
        [SerializeField] private TMPro.TextMeshProUGUI _debugOverlay;
        #endregion

        private void Start()
        {
            _playerFsm = _debugOverlay.GetComponentInParent<PlayerController_FSM>();

            if (_debugOverlay == null)
            {
                _debugOverlay = GetComponent<TMPro.TextMeshProUGUI>();
            }
        }

        private void Update()
        {
            var stringBuilder = new StringBuilder(500);
            float fHeadsetBaselineDiff = Mathf.Abs(_playerFsm.HeadsetBaseline - _playerFsm.CapsuleCollider.height);
            float fCurrentHeadsetHeight = _playerFsm.CapsuleCollider.height;

            stringBuilder.AppendLine($"Monitoring Jump: {_playerFsm.IsMonitoringJump}");
            stringBuilder.AppendLine($"Headset Baseline: {_playerFsm.HeadsetBaseline}");
            stringBuilder.AppendLine($"Current Headset Height: {fCurrentHeadsetHeight}");
            stringBuilder.AppendLine($"Headset Height Diff: {fHeadsetBaselineDiff}");

            stringBuilder.AppendLine($"Headset Up Acceleration: {_playerFsm.Rigidbody.velocity.y}");
            stringBuilder.AppendLine($"Is Grounded: {_playerFsm.IsGrounded}");

            _debugOverlay.text = stringBuilder.ToString();
        }
        public static void LogMessage(string message)
        {
#if UNITY_EDITOR
            Debug.Log(message);
#endif
        }
        public static void LogWarning(string message)
        {
#if UNITY_EDITOR
            Debug.LogWarning(message);
#endif
        }
        public static void LogError(string message)
        {
#if UNITY_EDITOR
            Debug.LogWarning(message);
#endif
        }
    }
}
