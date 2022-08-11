using Assets.Scripts;
using UnityEngine;

public class ArmSwingMovement : MonoBehaviour
{
    private PlayerController_FSM _player;

    // Game Objects
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject CameraObject;
    public GameObject ForwardDir;

    // Positions
    // Hands
    private Vector3 _vPreviousLeftHandPos;
    private Vector3 _vPreviousRightHandPos;
    private Vector3 _vCurrentRightHandPos;
    private Vector3 _vCurrentLeftHandPos;

    // Player
    private Vector3 _vPlayerPreviousPos;
    private Vector3 _vPlayerCurrentPos;

    // Speed
    public float PlayerSpeed = 60.0f;
    public float HandSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        // Gets the previous positions
        _vPlayerPreviousPos = transform.position;
        _vPreviousLeftHandPos = LeftHand.transform.position;
        _vPreviousRightHandPos = RightHand.transform.position;

        _player = GetComponentInParent<PlayerController_FSM>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_player.IsMonitoringJump || _player.IsClimbing)
            return;

        // Get the angle of where the camera is looking - rotation on the y-axis
        // Forward direction
        float yRotation = CameraObject.transform.eulerAngles.y;
        ForwardDir.transform.eulerAngles = new Vector3(0, yRotation, 0);

        // Get the current hand position
        _vCurrentLeftHandPos = LeftHand.transform.position;
        _vCurrentRightHandPos = RightHand.transform.position;

        // Get current player position
        _vPlayerCurrentPos = transform.position;

        // Get distance/difference in position of hands and player since last frame
        float fPlayerDistMoved = Vector3.Distance(_vPlayerCurrentPos, _vPlayerPreviousPos);
        float fLeftHandDistMoved = Vector3.Distance(_vCurrentLeftHandPos, _vPreviousLeftHandPos);
        float fRightHandDistMoved = Vector3.Distance(_vCurrentRightHandPos, _vPreviousRightHandPos);

        HandSpeed = (fLeftHandDistMoved - fPlayerDistMoved) + (fRightHandDistMoved - fPlayerDistMoved);

        if (Time.timeSinceLevelLoad > 2.0f)
        {
            _player.Rigidbody.AddForce(HandSpeed * PlayerSpeed * ForwardDir.transform.forward);
        }

        // Set hands previous frame to the current frame
        _vPreviousLeftHandPos = _vCurrentLeftHandPos;
        _vPreviousRightHandPos = _vCurrentRightHandPos;

        // Set player previous position to current position
        _vPlayerPreviousPos = _vPlayerCurrentPos;
    }
}