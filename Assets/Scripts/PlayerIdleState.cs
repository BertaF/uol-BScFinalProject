using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerIdleState : PlayerBaseState
    {
        #region Member Variables
        [SerializeField] public float jumpBaselineThreshold = 20.0f;

        private const int HeadsetHeightsListSize = 100;
        private readonly List<float> _headsetHeights = new(HeadsetHeightsListSize);
        private float _fPreviousHeadsetBaseline;
        private float _fPreviousHeadsetUpVelocity;

        // References to instances of player concrete states
        public readonly PlayerJumpState jumpState = new PlayerJumpState();
        #endregion

        public override void EnterState(PlayerController_FSM player)
        {
            // Make sure the headset heights list is empty before adding new values
            _headsetHeights.Clear();
            _fPreviousHeadsetUpVelocity = player.Rigidbody.velocity.y;
        }

        public override void OnUpdate(PlayerController_FSM player)
        {
            // Make the player capsule collider follow the camera (headset) position
            var center = player.XrOrigin.CameraInOriginSpacePos;
            player.CapsuleCollider.center = new Vector3(center.x, player.CapsuleCollider.height / 2.0f, center.z);

            // Use the camera floor height offset to reposition the player collider height
            player.CapsuleCollider.height = Mathf.Clamp(player.XrOrigin.CameraInOriginSpaceHeight, 1.0f, 3.0f);

            _headsetHeights.Add(player.CapsuleCollider.height);
            ComputeAverageHeadsetHeight(player);
        }

        public override void OnFixedUpdate(PlayerController_FSM player)
        {
            // If the headset has been lowered, enable the monitoring logic to check if we want to jump
            if (player.isGrounded && player.headsetBaseline < _fPreviousHeadsetBaseline)
            {
                player.StartCoroutine(MonitorPotentialJump(player));
            }
        }

        public override void OnCollisionEnter(PlayerController_FSM player)
        {
            player.isGrounded = true;
        }

        public override void OnCollisionExit(PlayerController_FSM player)
        {
            player.isGrounded = false;
        }

        // Calculates the average headset height from values gathered in the last 100 frames
        private void ComputeAverageHeadsetHeight(PlayerController_FSM player)
        {
            if (_headsetHeights.Count != HeadsetHeightsListSize) return;

            if (!player.isGrounded) return;

            // Cache the current headset baseline before updating it
            _fPreviousHeadsetBaseline = player.headsetBaseline;

            // Cache the new headset baseline average
            player.headsetBaseline = _headsetHeights.AsQueryable().Average();

            Debug.Log("[Headsetheight] Average headset height: " + player.headsetBaseline + ". Number of values gathered: " + _headsetHeights.Count);

            // Start gathering new headset height positions all over again
            _headsetHeights.Clear();
        }

        private IEnumerator MonitorPotentialJump(PlayerController_FSM player)
        {
            // Trigger a jump if the headset upwards acceleration is higher than the headset baseline
            float upAcceleration = (player.Rigidbody.velocity.y - _fPreviousHeadsetUpVelocity) / Time.fixedDeltaTime;
            _fPreviousHeadsetUpVelocity = player.Rigidbody.velocity.y;

            Debug.Log("[MonitorPotentialJump] Current up acceleration: " + upAcceleration + " > _headsetBaseline: " + player.headsetBaseline);

            if (upAcceleration > player.headsetBaseline + jumpBaselineThreshold)
            {
                player.StateTransition(jumpState);
                Debug.Log("[MonitorPotentialJump] Transitioning to jump state due to upAcceleration: " + upAcceleration + " > _headsetBaseline: " + player.headsetBaseline);
            }

            yield return null;
        }
    }
}
