using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The PlayerClimbState class, processes climb requests and is responsible for setting the physical forces required to perform a climb action.
    /// </summary>
    [RequireComponent(typeof(PlayerController_FSM))]

    public class PlayerClimbState : MonoBehaviour
    {
        #region Member Variables
        [Tooltip("Defines the max jump height value."), SerializeField]
        private float JumpHeight = 3.0f;

        [Tooltip("Defines the max jump forward value."), SerializeField]
        private float JumpFwd = 1.5f;

        [Tooltip("Reference to the XR Origin (headset position) used in spacial calculations."), SerializeField]
        private GameObject xrOrigin;

        [Tooltip("Affects how fast the player falls."), SerializeField]
        private float GravityScale = 1.0f;

        [Tooltip("The height difference from the ground to the current player Z position for the climbing forward force to be applied."), SerializeField]
        private float GroundThreshold = 1.0f;

        [Tooltip("The custom player capsule radius size used when climbing."), SerializeField]
        private float PlayerClimbCapsuleRadius = 0.2f;

        private float _groundPosition = 0.0f;
        private float _playerCapsuleColliderRadius;

        private PlayerController_FSM _playerController;
        private readonly List<VelocityController> _activeVelocities = new();
        #endregion

        /// <summary>
        /// Performs all the necessary variable initialisations required during the loading of the script.
        /// </summary>
        private void Awake()
        {
            FindPlayerController();
        }

        /// <summary>
        /// Performs all the necessary variable initialisations when entering the climb state.
        /// </summary>
        private void Start()
        {
            // Cache the player capsule radius before changing it.
            _playerCapsuleColliderRadius = _playerController.CapsuleCollider.radius;
        }

        /// <summary>
        /// For every fixed frame-rate frame it will determine if the player should be climbing.
        /// If a climb is requested, it calls the methods required to perform a climb.
        /// If a climb is not requested or has finished, it calls the methods required to end a climb and reset state variables.
        /// </summary>
        private void FixedUpdate()
        {
            TryBeginClimb();

            if (_playerController.IsClimbing)
            {
                // Reduce the player capsule collider slightly to prevent undesirable collisions.
                _playerController.CapsuleCollider.radius = PlayerClimbCapsuleRadius;

                ApplyVelocity();
            }

            TryEndClimb();
        }

        /// <summary>
        /// Checks if the player should be climbing.
        /// Sets the climbing boolean variable to true and caches the player current ground position.
        /// </summary>
        /// <returns>Void method, will early out if the player is not meant to be climbing</returns>
        private void TryBeginClimb()
        {
            if (!CanClimb()) return;

            _playerController.IsClimbing = true;

            if (!_playerController.IsClimbing)
            {
                // Store the player ground position so we know how far it has jumped.
                _groundPosition = _playerController.Rigidbody.transform.position.y;
            }
        }

        /// <summary>
        /// Checks if the climbing action has finished. This means checking that _activeVelocities list is empty.
        /// Also resets a few variables to their default values by calling the CleanUp method.
        /// </summary>
        /// <returns>Void method, will early out if the player is still climbing</returns>
        private void TryEndClimb()
        {
            if (CanClimb()) return;

            CleanUp();
        }

        /// <summary>
        /// Checks if the player wants to climb.
        /// </summary>
        /// <returns>True if there are velocity values stored in the _activeVelocities list, and false otherwise</returns>
        private bool CanClimb() => _activeVelocities.Count != 0;

        /// <summary>
        /// Applies upward and forward forces to the player rigidbody as part of the climbing motion.
        /// The velocity captured from the hand controllers is scaled by pre-defined jump force values and gravitional forces.
        /// </summary>
        private void ApplyVelocity()
        {
            // Transform the controller velocity vector direction relative to the XR Origin in world space.
            var tOrigin = xrOrigin.transform;
            var vVelocity = CollectControllerVelocity();
            vVelocity = tOrigin.TransformDirection(vVelocity);

            // If in the air apply a forward force in the player's forward direction
            if (_playerController.Rigidbody.transform.position.y > (_groundPosition + GroundThreshold))
            {
                // The jump trajectory is performed in the direction the camera is looking.
                // Gets the angle of where the camera is looking - rotation on the y-axis.
                var fCameraYRotation = _playerController.CameraObject.transform.eulerAngles.y;
                _playerController.ForwardDir.transform.eulerAngles = new Vector3(0, fCameraYRotation, 0);

                var fJumpFwdForce = Mathf.Sqrt(JumpFwd * -2 * (Physics.gravity.y * GravityScale));
                var vFwdForce = _playerController.ForwardDir.transform.forward * fJumpFwdForce;
                _playerController.Rigidbody.AddForce(vFwdForce, ForceMode.Force);
            }
            else
            {
                var fJumpForce = Mathf.Sqrt(JumpHeight * -2 * (Physics.gravity.y * GravityScale));
                var vPlayerUpDir = _playerController.CapsuleCollider.transform.up.normalized;
                var vJumpUpForce = Vector3.Project(vVelocity, vPlayerUpDir);
                _playerController.Rigidbody.AddForce(-vJumpUpForce * fJumpForce, ForceMode.Force);
            }
        }

        /// <summary>
        /// Gathers the velocity values of each controller, computes their cumulative values and stores the outcome in a list.
        /// </summary>
        /// <returns>A list with the sum of velocities generated per controller</returns>
        private Vector3 CollectControllerVelocity()
        {
            return _activeVelocities.Aggregate(Vector3.zero, (current, controller) => current + controller.Velocity);
        }

        /// <summary>
        /// Attempts to query the XR origin game object for the PlayerController_FSM component.
        /// </summary>
        /// <returns>Void method, but caches the PlayerController_FSM component into a variable of that type if found.</returns>
        private void FindPlayerController()
        {
            if (!_playerController)
            {
                _playerController = xrOrigin.GetComponent<PlayerController_FSM>();
            }
        }

        /// <summary>
        /// Stores the velocity vector associated with the controller that triggered the object interaction.
        /// </summary>
        /// <returns>Void method, that caches the velocity vector from this controller into a list.</returns>
        /// <param name="controllerVelocity">The velocity vector for this controller detected from the input action property</param>
        public void AddController(VelocityController controllerVelocity)
        {
            if (!_activeVelocities.Contains(controllerVelocity))
            {
                _activeVelocities.Add(controllerVelocity);
            }
        }

        /// <summary>
        /// Removes the velocity vector entry associated with this controller from the list of velocities.
        /// </summary>
        /// <returns>Void method, that removes this controller's velocity vector from the velocities list, if an entry is found.</returns>
        /// <param name="controllerVelocity">The velocity vector for this controller detected from the input action property</param>
        public void RemoveController(VelocityController controllerVelocity)
        {
            if (_activeVelocities.Contains(controllerVelocity))
            {
                _activeVelocities.Remove(controllerVelocity);
            }
        }

        /// <summary>
        /// Resets state specific variables to their default values, usually called when the climbing state ends.
        /// </summary>
        private void CleanUp()
        {
            _playerController.IsClimbing = false;
            _groundPosition = _playerController.Rigidbody.transform.position.y;
            _playerController.CapsuleCollider.radius = _playerCapsuleColliderRadius;
        }
    }
}
