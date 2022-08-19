using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The Checkpoint class derives from CheckpointManager. Its main responsibility is to request a checkpoint position update.
    /// Handles the on collision trigger events when the player touches a checkpoint collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]

    public class Checkpoint : CheckpointManager
    {
        /// <summary>
        /// Checks if this checkpoint collider touched the player collider, if so it request a checkpoint position update.
        /// </summary>
        /// <param name="other">The parameter passed is the game object this checkpoint collided with. Should be the player capsule collider.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) { return; }

            SetCheckpoint(transform);
        }
    }
}
