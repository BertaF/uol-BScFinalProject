using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class CheckpointManager : MonoBehaviour
    {
        protected enum CoordinateSource
        {
            TransformPosition,
            ColliderPosition,
            Vector3Variable
        }

        [SerializeField] protected CoordinateSource _coordinateSource;
        [SerializeField] protected Vector3 _respawnCoordinates;

        private Vector3 _checkpoint = Vector3.zero;

        public void SetCheckpoint(Vector3 newCheckpoint) => _checkpoint = newCheckpoint;
        public Vector3 GetCurrentCheckpoint() => _checkpoint;

        public void TeleportToCheckpoint(Transform targetPoint)
        {
            targetPoint.transform.SetPositionAndRotation(_checkpoint, Quaternion.identity);
        }
    }

    [RequireComponent(typeof(Collider))]
    public class Checkpoint : CheckpointManager
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            switch (_coordinateSource)
            {
                case CoordinateSource.TransformPosition:
                    SetCheckpoint(transform.position);
                    break;
                case CoordinateSource.ColliderPosition:
                    SetCheckpoint(transform.position + GetComponent<Collider>().bounds.center);
                    break;
                case CoordinateSource.Vector3Variable:
                    SetCheckpoint(_respawnCoordinates);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // if my previous checkpoint was at the same level and now I'm below the checkpoint then means I fell
    }
}
