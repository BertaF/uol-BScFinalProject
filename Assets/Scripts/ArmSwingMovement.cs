using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmSwingMovement : MonoBehaviour
{
    // Game Objects
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject cameraObject;
    public GameObject forwardDir;

    // Positions
    // Hands
    private Vector3 vPreviousLeftHandPos;
    private Vector3 vPreviousRightHandPos;
    private Vector3 vCurrentRightHandPos;
    private Vector3 vCurrentLeftHandPos;

    // Player
    private Vector3 vPlayerPreviousPos;
    private Vector3 vPlayerCurrentPos;

    // Speed
    public float playerSpeed = 60.0f;
    public float handSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // Gets the previous positions
        vPlayerPreviousPos = transform.position;
        vPreviousLeftHandPos = leftHand.transform.position;
        vPreviousRightHandPos = rightHand.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        // Get the angle of where the camera is looking - rotation on the y-axis
        // Forward direction
        float yRotation = cameraObject.transform.eulerAngles.y;
        forwardDir.transform.eulerAngles = new Vector3(0, yRotation, 0);

        // Get the current hand position
        vCurrentLeftHandPos = leftHand.transform.position;
        vCurrentRightHandPos = rightHand.transform.position;

        // Get current player position
        vPlayerCurrentPos = transform.position;

        // Get distance/difference in position of hands and player since last frame
        float fPlayerDistMoved = Vector3.Distance(vPlayerCurrentPos, vPlayerPreviousPos);
        float fLeftHandDistMoved = Vector3.Distance(vCurrentLeftHandPos, vPreviousLeftHandPos);
        float fRightHandDistMoved = Vector3.Distance(vCurrentRightHandPos, vPreviousRightHandPos);

        handSpeed = (fLeftHandDistMoved - fPlayerDistMoved) + (fRightHandDistMoved - fPlayerDistMoved);

        if (Time.timeSinceLevelLoad > 2.0f)
        {
            transform.position += forwardDir.transform.forward * handSpeed * playerSpeed * Time.deltaTime;
        }

        // Set hands previous frame to the current frame
        vPreviousLeftHandPos = vCurrentLeftHandPos;
        vPreviousRightHandPos = vCurrentRightHandPos;

        // Set player previous position to current position
        vPlayerPreviousPos = vPlayerCurrentPos;
    }
}