using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    /// <summary>
    /// The VelocityController class, is responsible for reading the velocity input values from the active controller.
    /// It initialises the Velocity variable to those velocity vector values.
    /// </summary>
    public class VelocityController : MonoBehaviour
    {
        [SerializeField] private InputActionProperty _velocityInput;
        public Vector3 Velocity => _velocityInput.action.ReadValue<Vector3>();
    }
}
