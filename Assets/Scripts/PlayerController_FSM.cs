using Unity.XR.CoreUtils;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
    public class PlayerController_FSM : MonoBehaviour
    {
        #region Member Variables
        [Header("Player Settings")]
        // This is based on a seated position, so will need to be adjusted if standing
        [SerializeField] public float HeightDiffToTriggerJump = 0.15f;
        [SerializeField] public float JumpBaselineThreshold = 1.0f;
        [SerializeField] public float DistToGround = 1.5f;
        [SerializeField] public bool ForceFall = false;

        [SerializeField] public GameObject CameraObject;
        [SerializeField] public GameObject ForwardDir;
        [SerializeField] public float CheckpointFallThreshold;
        [SerializeField] public float JumpTriggerSoundThreshold;

        [HideInInspector] public float HeadsetBaseline;
        [HideInInspector] public float PreviousHeadsetBaseline;
        [HideInInspector] public float UpAcceleration;
        [HideInInspector] public float PreviousHeadsetUpVelocity;
        [HideInInspector] public bool IsGrounded;
        [HideInInspector] public bool IsMonitoringJump;
        [HideInInspector] public bool IsJumping;
        [HideInInspector] public bool IsClimbing;

        [Header("Player Game Events")]
        [SerializeField] public GameEvent JumpLanding;

        // References to instances of player concrete states
        public readonly PlayerIdleState IdleState = new();
        public Rigidbody Rigidbody { get; private set; }
        public PlayerBaseState CurrentState { get; private set; }
        public XROrigin XrOrigin { get; private set; }
        public CapsuleCollider CapsuleCollider { get; private set; }

        private Vector3 _currentPlayerPos;
        private bool _wasInAir;
        #endregion

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            XrOrigin = GetComponent<XROrigin>();
            CapsuleCollider = GetComponent<CapsuleCollider>();

            _wasInAir = false;
            IsGrounded = false;
            IsMonitoringJump = false;
            IsJumping = false;
            IsClimbing = false;
            HeadsetBaseline = CapsuleCollider.height;
            PreviousHeadsetBaseline = CapsuleCollider.height;

            UpAcceleration = 0.0f;
            PreviousHeadsetUpVelocity = 0.0f;
            _currentPlayerPos = Vector3.zero;
        }

        private void Start()
        {
            DoGroundCheck();

            // Sets the initial state to be idle
            StateTransition(IdleState);
        }

        private void FixedUpdate()
        {
            DoGroundCheck();
            CurrentState.OnFixedUpdate(this);
        }

        private void OnCollisionEnter(Collision other)
        {
            CurrentState.OnCollisionEnter(other, this);
        }

        private void OnCollisionExit(Collision other)
        {
            CurrentState.OnCollisionExit(other, this);
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
        private void DoGroundCheck()
        {
            IsGrounded = Physics.Raycast(CapsuleCollider.transform.position + CapsuleCollider.center, Vector3.down, out var rayHit, DistToGround + 0.1f);

            if (IsGrounded)
            {
                if (_wasInAir)
                {
                    // Play an audio landing SFX when returning to the ground
                    JumpLanding?.Invoke();
                }

                _wasInAir = false;
                _currentPlayerPos = CapsuleCollider.transform.position;
            }
            else
            {
                _wasInAir = true;
            }
        }

        public bool HasFallen()
        {
            if (ForceFall)
            {
                return true;
            }

            if (IsGrounded || IsClimbing) { return false; }

            // Checks if the current player position is below the previous.
            float fDot = Vector3.Dot(CapsuleCollider.transform.position - _currentPlayerPos, Vector3.up);
            if (!(fDot <= 0.0f)) { return false; }

            // Make sure there is a significant height difference between the previous and current positions.
            return Mathf.Abs(CapsuleCollider.transform.position.y - _currentPlayerPos.y) >= CheckpointFallThreshold;
        }
    }
}
