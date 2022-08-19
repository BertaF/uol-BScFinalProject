using System;
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
        private const int HeadsetHeightsListSize = 50;
        private readonly List<float> _headsetHeights = new(HeadsetHeightsListSize);
        private readonly List<Vector3> _headsetPositions = new(HeadsetHeightsListSize);
        private Vector3 _vHeadsetVelocity;
        private Vector3 _vPreviousHeadsetPos;
        private Vector3 _vUpHeadsetVelocity;

        // References to instances of player concrete states
        public readonly PlayerJumpState JumpState = new PlayerJumpState();
        #endregion

        public override void EnterState(PlayerController_FSM player)
        {
            // Make sure the headset heights list is empty before adding new values
            _headsetHeights.Clear();
            _headsetPositions.Clear();

            _vHeadsetVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            _vPreviousHeadsetPos = new Vector3(0.0f, 0.0f, 0.0f);
            _vUpHeadsetVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public override void OnUpdate(PlayerController_FSM player)
        {
            // Avoid all computations if not on ground
            if (!player.IsGrounded)
                return;

            // Make the player capsule collider follow the camera (headset) position
            var cameraPos = player.XrOrigin.CameraInOriginSpacePos;
            player.CapsuleCollider.center = new Vector3(cameraPos.x, player.CapsuleCollider.height / 2.0f, cameraPos.z);

            // Use the camera floor height offset to reposition the player collider height
            player.CapsuleCollider.height = Mathf.Clamp(player.XrOrigin.CameraInOriginSpaceHeight, 1.0f, 3.0f);

            var playerPos = player.CapsuleCollider.transform.position;
            var playerHeight = player.CapsuleCollider.height;

            if (!player.IsMonitoringJump)
            {
                // When not monitoring for jumps, update the list of headset heights which is reset every HeadsetHeightsListSize frames.
                _headsetHeights.Add(player.CapsuleCollider.height);
                var vWorldPosition = player.CapsuleCollider.transform.TransformDirection(new Vector3(playerPos.x, playerHeight, playerPos.z));
                _headsetPositions.Add(vWorldPosition);
            }

            ComputeAverageHeadsetHeight(player);

            var vCurrHeadsetPos = new Vector3(playerPos.x, playerHeight, playerPos.z);

            if (Time.deltaTime > 0.0f)
            {
                // Get the vector with the difference over time between previous and current headset position
                _vHeadsetVelocity = (vCurrHeadsetPos - _vPreviousHeadsetPos) / Time.deltaTime;

                // Cache the current headset position which will be the previous in the next frame
                _vPreviousHeadsetPos = vCurrHeadsetPos;

                // Get how much velocity is projected towards the upwards position
                _vUpHeadsetVelocity = Vector3.Project(_vHeadsetVelocity, player.CapsuleCollider.transform.up.normalized);
                player.UpAcceleration = _vUpHeadsetVelocity.magnitude;
            }

#if UNITY_EDITOR
            if (_headsetPositions.Count > 1)
            {
                DebugRender.RenderHeadsetBaseline(_headsetPositions, player.HeadsetBaseline, player.CapsuleCollider.height);
            }
#endif
        }

        public override void OnFixedUpdate(PlayerController_FSM player)
        {
            // Avoid all computations if not on ground 
            if (!player.IsGrounded)
            {
                DebugRender.LogMessage("[OnFixedUpdate] Player is not grounded, NOT UPDATING : " + player.IsGrounded);
                return;
            }

            // If the headset has been lowered, enable the monitoring logic to check if we want to jump
            float fHeadsetBaselineDiff = Mathf.Abs(player.HeadsetBaseline - player.CapsuleCollider.height);
            bool bLoweredHeadset = player.CapsuleCollider.height < player.HeadsetBaseline;

            if (player.IsGrounded && bLoweredHeadset && fHeadsetBaselineDiff > player.HeightDiffToTriggerJump)
            {
                player.StartCoroutine(MonitorPotentialJump(player));
            }
            else
            {
                player.IsMonitoringJump = false;
            }
        }

        public override void OnCollisionEnter(Collision other, PlayerController_FSM player)
        {}

        public override void OnCollisionExit(Collision other, PlayerController_FSM player)
        {}

        // Calculates the average headset height from values gathered in the last 50 frames
        private void ComputeAverageHeadsetHeight(PlayerController_FSM player)
        {
            if (_headsetHeights.Count != HeadsetHeightsListSize) return;
            if (_headsetPositions.Count != HeadsetHeightsListSize) return;

            // Cache the current headset baseline before updating it
            player.PreviousHeadsetBaseline = player.HeadsetBaseline;

            // Cache the new headset baseline average
            player.HeadsetBaseline = _headsetHeights.AsQueryable().Average();

            // Start gathering new headset height positions all over again
            _headsetHeights.Clear();
            _headsetPositions.Clear();
        }

        private IEnumerator MonitorPotentialJump(PlayerController_FSM player)
        {
            player.IsMonitoringJump = true;

            // Trigger a jump if the headset upwards acceleration is higher than the threshold
            if (_vUpHeadsetVelocity.y > 0.0f && _vUpHeadsetVelocity.sqrMagnitude > player.JumpBaselineThreshold)
            {
                player.StateTransition(JumpState);
            }

            yield return null;
        }
    }
}
