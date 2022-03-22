using Mirror;
using UnityEngine;

namespace Game.Player.Spectating
{
    public class SpectatorLook : NetworkBehaviour
    {
        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;

        public float minimumX = -360f;
        public float maximumX = 360f;

        public float minimumY = -60f;
        public float maximumY = 60f;

        float rotationY = 0F;

        void Update()
        {
            if (MenuOpen.IsOpen)
                return;

            if (axes == RotationAxes.MouseXAndY)
            {
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * GameSettings.Sensitivity;
			
                rotationY += Input.GetAxis("Mouse Y") * GameSettings.Sensitivity;
                rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * GameSettings.Sensitivity, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * GameSettings.Sensitivity;
                rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }
        }
	
        void Start()
        {
            if (!hasAuthority) enabled = false;
            // Make the rigid body not change rotation
            if (TryGetComponent(out Rigidbody rb))
                rb.freezeRotation = true;
        }
    }
}