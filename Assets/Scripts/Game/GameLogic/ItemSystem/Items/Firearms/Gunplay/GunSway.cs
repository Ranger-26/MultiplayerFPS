using Game.UI;
using Inputs;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Items.Firearms.Gunplay
{
    public class GunSway : MonoBehaviour
    {
        [Header("Sway Settings")]
        [SerializeField] private float smooth;
        [SerializeField] private float multiplier;

        Vector2 mouse;

        InputAction action;

        private void Awake()
        {
            action = GameInputManager.Actions.Player.Look;
        }

        private void Start()
        {
            if (!GetComponentInParent<NetworkIdentity>().hasAuthority)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            mouse = action.ReadValue<Vector2>();

            if (MenuOpen.IsOpen)
                return;

            // get mouse input
            float mouseX = mouse.x * multiplier;
            float mouseY = mouse.y * multiplier;

            // calculate target rotation
            Quaternion rotationX = Quaternion.AngleAxis(mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(-mouseX, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            // rotate 
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
    }
}
