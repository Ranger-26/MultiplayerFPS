using System;
using Game.UI;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Gunplay
{
    public class GunSway : MonoBehaviour
    {
        [Header("Sway Settings")]
        [SerializeField] private float smooth;
        [SerializeField] private float multiplier;

        Vector2 mouse;

        private void Start()
        {
            if (!GetComponentInParent<NetworkIdentity>().hasAuthority)
            {
                enabled = false;
            }
        }

        public void UpdateMouse(InputAction.CallbackContext callbackContext)
        {
            mouse = callbackContext.ReadValue<Vector2>();

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
