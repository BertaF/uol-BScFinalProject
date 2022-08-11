using UnityEngine;
using UnityEngine.InputSystem;

public class VelocityController : MonoBehaviour
{
    [SerializeField] private InputActionProperty _velocityInput;
    public Vector3 Velocity => _velocityInput.action.ReadValue<Vector3>();
}
