using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerIdleState : PlayerBaseState
    {
        #region Member Variables
        private const int HeadsetHeightsListSize = 50;
        private readonly List<float> _headsetHeights = new(HeadsetHeightsListSize);
        private Vector3 vHeadsetVelocity;
        private Vector3 vPreviousHeadsetPos;
        private Vector3 vUpHeadsetVelocity;

        // References to instances of player concrete states
        public readonly PlayerJumpState JumpState = new PlayerJumpState();
        #endregion

        public override void EnterState(PlayerController_FSM player)
        {
            // Make sure the headset heights list is empty before adding new values
            _headsetHeights.Clear();

           vHeadsetVelocity = new(0.0f, 0.0f, 0.0f);
           vPreviousHeadsetPos = new(0.0f, 0.0f, 0.0f);
           vUpHeadsetVelocity = new(0.0f, 0.0f, 0.0f);
        }

        public override void OnUpdate(PlayerController_FSM player)
        {
            // Make the player capsule collider follow the camera (headset) position
            var center = player.XrOrigin.CameraInOriginSpacePos;
            player.CapsuleCollider.center = new Vector3(center.x, player.CapsuleCollider.height / 2.0f, center.z);

            // Use the camera floor height offset to reposition the player collider height
            player.CapsuleCollider.height = Mathf.Clamp(player.XrOrigin.CameraInOriginSpaceHeight, 1.0f, 3.0f);

            // Update the list of headset heights which is reset every HeadsetHeightsListSize frames
            _headsetHeights.Add(player.CapsuleCollider.height);
            ComputeAverageHeadsetHeight(player);

            if (Time.deltaTime > 0.0f)
            {
                // Get the vector with the difference over time between previous and current headset position
                Vector3 vCurrHeadsetPos = player.CapsuleCollider.transform.position;
                vHeadsetVelocity = (vCurrHeadsetPos - vPreviousHeadsetPos) / Time.deltaTime;
                vPreviousHeadsetPos = player.CapsuleCollider.transform.position;

                // Get how much velocity is projected towards the upwards position
                vUpHeadsetVelocity = Vector3.Project(vHeadsetVelocity, player.CapsuleCollider.transform.up.normalized);
            }

#if UNITY_EDITOR
            DebugRender.LogMessage("[Headset Velocity] vUpHeadsetVelocity: " + vUpHeadsetVelocity);
#endif
        }

        public override void OnFixedUpdate(PlayerController_FSM player)
        {
            // If the headset has been lowered, enable the monitoring logic to check if we want to jump
            float fCurrentHeadsetHeight = player.CapsuleCollider.height;
            //float fHeadsetBaselineDiff = Mathf.Abs(player.HeadsetBaseline - fCurrentHeadsetHeight);
            bool bLoweredHeadset = fCurrentHeadsetHeight < player.HeadsetBaseline;

            if (player.IsGrounded /*&& fHeadsetBaselineDiff > player.HeightDiffToTriggerJump*/ && bLoweredHeadset)
            {
                player.StartCoroutine(MonitorPotentialJump(player));
            }
            else
            {
                player.IsMonitoringJump = false;
            }
        }

        public override void OnCollisionEnter(PlayerController_FSM player)
        {
            player.IsGrounded = true;
        }

        public override void OnCollisionExit(PlayerController_FSM player)
        {
            player.IsGrounded = false;
        }

        // Calculates the average headset height from values gathered in the last 50 frames
        private void ComputeAverageHeadsetHeight(PlayerController_FSM player)
        {
            if (_headsetHeights.Count != HeadsetHeightsListSize) return;

            if (!player.IsGrounded) return;

            // Cache the current headset baseline before updating it
            player.PreviousHeadsetBaseline = player.HeadsetBaseline;

            // Cache the new headset baseline average
            player.HeadsetBaseline = _headsetHeights.AsQueryable().Average();

#if UNITY_EDITOR
            DebugRender.LogMessage("[Headsetheight] Average headset height: " + player.HeadsetBaseline + ". Number of values gathered: " + _headsetHeights.Count);
#endif

            // Start gathering new headset height positions all over again
            _headsetHeights.Clear();
        }

        private IEnumerator MonitorPotentialJump(PlayerController_FSM player)
        {
            player.IsMonitoringJump = true;

#if UNITY_EDITOR
            DebugRender.LogMessage("[MonitorPotentialJump] Current up velocity: " + vHeadsetVelocity + " > player.JumpBaselineThreshold: " + player.JumpBaselineThreshold);
#endif
            // Trigger a jump if the headset upwards acceleration is higher than the threshold
            if (vUpHeadsetVelocity.sqrMagnitude > player.JumpBaselineThreshold)
            {
                player.StateTransition(JumpState);
            }

            yield return null;
        }
    }
}
