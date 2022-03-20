using System;
using Mirror;
using UnityEngine;

namespace Game.Player.Gunplay
{
    public class GunSway : MonoBehaviour
    {
        [Header("Sway Settings")]
        [SerializeField] private float smooth;
        [SerializeField] private float multiplier;

        private void Start()
        {
            if (!GetComponentInParent<NetworkIdentity>().hasAuthority)
            {
                enabled = false;
            }
        }


        private void Update()
        {
            if (MenuOpen.IsOpen)
                return;

            // get mouse input
            float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

            // calculate target rotation
            Quaternion rotationX = Quaternion.AngleAxis(mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(-mouseX, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            // rotate 
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
    }
}
