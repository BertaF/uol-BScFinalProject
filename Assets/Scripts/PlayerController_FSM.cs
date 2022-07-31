using Unity.XR.CoreUtils;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController_FSM : MonoBehaviour
    {
        #region Member Variables
        [SerializeField] public float JumpBaselineThreshold = 1.0f;
        // This is based on a seated position, so will need to be adjusted if standing
        [SerializeField] public float HeightDiffToTriggerJump = 0.15f;

        // References to instances of player concrete states
        public readonly PlayerIdleState IdleState = new();
        public Rigidbody Rigidbody { get; private set; }
        public PlayerBaseState CurrentState { get; private set; }
        public XROrigin XrOrigin { get; private set; }
        public CapsuleCollider CapsuleCollider { get; private set; }

        [SerializeField] public GameObject CameraObject;
        [SerializeField] public GameObject ForwardDir;

        [HideInInspector] public float HeadsetBaseline;
        [HideInInspector] public float PreviousHeadsetBaseline;
        [HideInInspector] public float UpAcceleration;
        [HideInInspector] public float PreviousHeadsetUpVelocity;
        [HideInInspector] public bool IsGrounded;
        [HideInInspector] public bool IsMonitoringJump;
        #endregion

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            XrOrigin = GetComponent<XROrigin>();
            CapsuleCollider = GetComponent<CapsuleCollider>();

            IsGrounded = false;
            IsMonitoringJump = false;
            HeadsetBaseline = CapsuleCollider.height;
            PreviousHeadsetBaseline = CapsuleCollider.height;

            UpAcceleration = 0.0f;
            PreviousHeadsetUpVelocity = Rigidbody.velocity.y;
        }

        private void Start()
        {
            // Sets the initial state to be idle
            StateTransition(IdleState);
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
