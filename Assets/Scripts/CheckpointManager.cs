using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class CheckpointManager : MonoBehaviour
    {
        private Vector3 _checkpoint = Vector3.zero;

        public void SetCheckpoint(Vector3 newCheckpoint) => _checkpoint = newCheckpoint;
        public Vector3 GetCurrentCheckpoint() => _checkpoint;

        public void TeleportToCheckpoint(Transform targetPoint)
        {
            targetPoint.transform.SetPositionAndRotation(_checkpoint, Quaternion.identity);
        }
    }
}
