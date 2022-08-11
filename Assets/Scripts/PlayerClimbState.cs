using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class PlayerClimbState : MonoBehaviour
{
    #region Member Variables
    [SerializeField] private float JumpHeight = 2.0f;
    [SerializeField] private float JumpFwd = 1.8f;
    [SerializeField] private GameObject xrOrigin;
    private const float GravityScale = 1.0f;
    [SerializeField] private float GroundThreshold = 5.0f;
    private float _groundPosition = 0.0f;

    private Rigidbody _rb;
    private PlayerController_FSM _playerController;
    private readonly List<VelocityController> _activeVelocities = new();
    #endregion

    private void Awake()
    {
        FindRigidbody();
        FindPlayerController();
    }

    private void FixedUpdate()
    {
        TryBeginClimb();

        if (_playerController.IsClimbing) { ApplyVelocity(); }

        TryEndClimb();
    }

    private void TryBeginClimb()
    {
        if (!CanClimb()) return;

        _groundPosition = _rb.transform.position.y;
        _playerController.IsClimbing = true;
    }

    private void TryEndClimb()
    {
        if (!CanClimb())
        {
            _playerController.IsClimbing = false;
            _groundPosition = 0.0f;
        }
    }

    private bool CanClimb()
    {
        return _activeVelocities.Count != 0;
    }

    private void ApplyVelocity()
    {
        Vector3 vVelocity = CollectControllerVelocity();
        Transform origin = xrOrigin.transform;
        vVelocity = origin.TransformDirection(vVelocity);

        if (_rb)
        {
            Vector3 vForce = Vector3.zero;

            // If in the air apply a forward force in the player's forward direction 
            if (_rb.transform.position.y > (/*_groundPosition +*/ GroundThreshold))
            {
                // The jump trajectory is performed in the direction the camera is looking.
                // Gets the angle of where the camera is looking - rotation on the y-axis.
                float yRotation = _playerController.CameraObject.transform.eulerAngles.y;
                _playerController.ForwardDir.transform.eulerAngles = new Vector3(0, yRotation, 0);

                float fJumpFwdForce = Mathf.Sqrt(JumpFwd * -2 * (Physics.gravity.y * GravityScale));
                vVelocity -= _playerController.ForwardDir.transform.forward * fJumpFwdForce;
            }

            float fJumpForce = Mathf.Sqrt(JumpHeight * -2 * (Physics.gravity.y * GravityScale));
            vForce = new Vector3(vVelocity.x, vVelocity.y * fJumpForce, vVelocity.z);
            _rb.AddForce(-vForce, ForceMode.Force);
        }
    }

    private Vector3 CollectControllerVelocity()
    {
        return _activeVelocities.Aggregate(Vector3.zero, (current, controller) => current + controller.Velocity);
    }

    private void FindRigidbody()
    {
        if (!_rb)
        {
            _rb = xrOrigin.GetComponent<Rigidbody>();
        }
    }
    private void FindPlayerController()
    {
        if (!_playerController)
        {
            _playerController = xrOrigin.GetComponent<PlayerController_FSM>();
        }
    }

    public void AddController(VelocityController controller)
    {
        if (!_activeVelocities.Contains(controller))
        {
            _activeVelocities.Add(controller);
        }
    }

    public void RemoveController(VelocityController controller)
    {
        if (_activeVelocities.Contains(controller))
        {
            _activeVelocities.Remove(controller);
        }
    }
}
