using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerJumpState : PlayerBaseState
    {
        #region Member Variables
        [SerializeField] public float jumpFwdScaleFactor = 1.8f;
        [SerializeField] public float jumpHeight = 2.0f;
        [SerializeField] public float gravityScale = 1.0f;
        [SerializeField] public float jumpForce = 500.0f;
        [SerializeField] public float fallingGravityScale = 10.0f;
        #endregion

        public override void EnterState(PlayerController_FSM player)
        {
            // Vector3 vJumpForce = Physics.gravity * (1 - gravityScale) * rb.mass;
            // *-2 is suppose to be the gravity value.
            float fJumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics.gravity.y * gravityScale));
            player.Rigidbody.AddForce(new Vector3(0, fJumpForce, 0), ForceMode.Impulse);
            Debug.Log("Starting jumping with force vector: " + fJumpForce);
        }

        public override void OnUpdate(PlayerController_FSM player)
        {}

        public override void OnFixedUpdate(PlayerController_FSM player)
        {
            // Apply a forward movement to the player forward vector every frame
            float fJumpFwdForce = Mathf.Sqrt(jumpFwdScaleFactor * -2 * (Physics.gravity.y * gravityScale));
            player.Rigidbody.AddForce(new Vector3(0, 0, fJumpFwdForce), ForceMode.Force);
        }

        public override void OnCollisionEnter(PlayerController_FSM player)
        {
            player.isGrounded = true;

            // Transition back to the idle state once the jump has finished / player landed on ground
            player.StateTransition(player.idleState);
        }

        public override void OnCollisionExit(PlayerController_FSM player)
        {
            player.isGrounded = false;
        }
    }
}