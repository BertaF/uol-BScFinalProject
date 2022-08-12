using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJumpState : PlayerBaseState
    {
        #region Member Variables
        [SerializeField] public float JumpFwdScaleFactor = 1.8f;
        [SerializeField] public float JumpHeight = 1.5f;
        [SerializeField] public float GravityScale = 1.0f;

        private HapticsController haptics;
        #endregion

        public override void EnterState(PlayerController_FSM player)
        {
            haptics = player.GetComponent<HapticsController>();

            // *-2 is supposed to be the gravity value.
            float fJumpForce = Mathf.Sqrt(JumpHeight * -2 * (Physics.gravity.y * GravityScale));
            player.Rigidbody.AddForce(new Vector3(0, fJumpForce, 0), ForceMode.Impulse);

#if UNITY_EDITOR
            DebugRender.LogMessage("Starting jumping with force vector: " + fJumpForce);
#endif
        }

        public override void OnUpdate(PlayerController_FSM player)
        {}

        public override void OnFixedUpdate(PlayerController_FSM player)
        {
            /* The jump trajectory is performed in the direction the camera is looking.
            Gets the angle of where the camera is looking - rotation on the y-axis.*/
            float yRotation = player.CameraObject.transform.eulerAngles.y;
            player.ForwardDir.transform.eulerAngles = new Vector3(0, yRotation, 0);

            // Apply a forward movement to the player forward vector every frame.
            float fJumpFwdForce = Mathf.Sqrt(JumpFwdScaleFactor * -2 * (Physics.gravity.y * GravityScale));
            player.Rigidbody.AddForce(player.ForwardDir.transform.forward * fJumpFwdForce, ForceMode.Force);
        }

        public override void OnCollisionEnter(Collision other, PlayerController_FSM player)
        {
            if (player.JumpLanding)
            {
                Debug.Log("Sending Haptics");
                player.JumpLanding.Invoke();
            }

            haptics.SendHaptics();

            //player.IsGrounded = true;

            // Transition back to the idle state once the jump has finished / player landed on ground
            player.StateTransition(player.IdleState);
        }

        public override void OnCollisionExit(Collision other, PlayerController_FSM player)
        {
            //player.IsGrounded = false;
        }
    }
}