using Unity.XR.CoreUtils;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController_FSM : MonoBehaviour
    {
        #region Member Variables
        // References to instances of player concrete states
        public readonly PlayerIdleState idleState = new PlayerIdleState();
        public Rigidbody Rigidbody { get; private set; }
        public PlayerBaseState CurrentState { get; private set; }
        public XROrigin XrOrigin { get; private set; }
        public CapsuleCollider CapsuleCollider { get; private set; }

        [HideInInspector]
        public float headsetBaseline;
        [HideInInspector]
        public bool isGrounded;
        #endregion

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            XrOrigin = GetComponent<XROrigin>();
            CapsuleCollider = GetComponent<CapsuleCollider>();

            headsetBaseline = CapsuleCollider.height;
        }

        private void Start()
        {
            // Sets the initial state to be idle
            StateTransition(idleState);
        }

        private void FixedUpdate()
        {
            CurrentState.OnFixedUpdate(this);
        }

        private void OnCollisionEnter()
        {
            CurrentState.OnCollisionEnter(this);
        }

        private void OnCollisionExit()
        {
            CurrentState.OnCollisionExit(this);
        }

        private void Update()
        {
            CurrentState.OnUpdate(this);
        }

        // Handles state transitions from a current state to the next
        public void StateTransition(PlayerBaseState state)
        {
            CurrentState = state;
            CurrentState.EnterState(this);
        }
    }
}
