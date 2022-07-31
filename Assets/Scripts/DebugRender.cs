using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class DebugRender : MonoBehaviour
    {
        #region Member Variables
        private PlayerController_FSM _playerFsm;
        [SerializeField] private TMPro.TextMeshProUGUI _debugOverlay;
        [SerializeField] public List<LineRenderer> PLines;
        private static List<LineRenderer> _lines;
        #endregion

        private void Start()
        {
            _lines = PLines;

            foreach (var line in _lines)
            {
                line.enabled = true;
                line.positionCount = 2;
                line.useWorldSpace = true;

                if (line.gameObject.CompareTag("HeadsetBaseline"))
                {
                    line.startColor = Color.white;
                    line.endColor = Color.blue;
                }

                if (line.gameObject.CompareTag("HeadsetCurrentPosLine"))
                {
                    line.startColor = Color.white;
                    line.endColor = Color.green;
                }
            }

            _playerFsm = _debugOverlay.GetComponentInParent<PlayerController_FSM>();

            if (_debugOverlay == null)
            {
                _debugOverlay = GetComponent<TMPro.TextMeshProUGUI>();
            }
        }

        private void Update()
        {
            var stringBuilder = new StringBuilder(500);

            stringBuilder.AppendLine($"Monitoring Jump: {_playerFsm.IsMonitoringJump}");
            stringBuilder.AppendLine($"Headset Baseline: {_playerFsm.HeadsetBaseline}");
            stringBuilder.AppendLine($"Headset Up Acceleration: {_playerFsm.UpAcceleration}");
            stringBuilder.AppendLine($"Is Grounded: {_playerFsm.IsGrounded}");

            _debugOverlay.text = stringBuilder.ToString();

            foreach (var line in _lines)
            {
                if (line.gameObject.CompareTag("HeadsetCurrentPosLine"))
                {
                    line.SetPosition(0, new Vector3(transform.position.x - 0.5f, _playerFsm.HeadsetBaseline, transform.position.z - 1.0f));
                    line.SetPosition(1, new Vector3(transform.position.x + 0.5f, _playerFsm.HeadsetBaseline, transform.position.z - 1.0f));
                }
                else if (line.gameObject.CompareTag("HeadsetBaseline"))
                {
                    line.SetPosition(0, new Vector3(transform.position.x - 1.0f, _playerFsm.CapsuleCollider.height, transform.position.z - 1.0f));
                    line.SetPosition(1, new Vector3(transform.position.x + 1.0f, _playerFsm.CapsuleCollider.height, transform.position.z - 1.0f));
                }
            }
        }

        public static void RenderHeadsetBaseline(List<Vector3> headsetPositions, float fBaseline, float fCurrentHeight)
        {
            foreach (var line in _lines)
            {
                Vector3 vStartPos;
                Vector3 vEndPos;

                if (line.gameObject.CompareTag("HeadsetCurrentPosLine"))
                {
                    vStartPos = new Vector3(line.GetPosition(0).x, fCurrentHeight, line.GetPosition(0).z);
                    vEndPos = new Vector3(line.GetPosition(1).x, fCurrentHeight, line.GetPosition(1).z);

                    line.SetPosition(0, vStartPos);
                    line.SetPosition(1, vEndPos);
                }
                else if (line.gameObject.CompareTag("HeadsetBaseline"))
                {
                    vStartPos = new Vector3(line.GetPosition(0).x, fBaseline, line.GetPosition(0).z);
                    vEndPos = new Vector3(line.GetPosition(1).x, fBaseline, line.GetPosition(1).z);

                    line.SetPosition(0, vStartPos);
                    line.SetPosition(1, vEndPos);
                }
            }
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
