using Mirror;
using UnityEngine;

namespace Game.Player.Movement
{
    public class PlayerLook : NetworkBehaviour
    {
        public static PlayerLook Instance;

        public float lookSpeed = 1.0f;
        public float lookXLimit = 90.0f;

        Transform Player;

        float rotationX = 0;
        float rotationY = 0;
        float addY;

        private void Awake()
        {


            Player = transform.parent;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            rotationY = Input.GetAxis("Mouse X") * lookSpeed + addY;
            addY = 0f;
            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            Player.transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public void MoveCamera(float x, float y)
        {
            rotationX += -x;
            addY = y;

            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            Player.transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public void MoveCamera(float x)
        {
            rotationX += -x;

            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            Player.transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public void MoveCamera(Vector2 move)
        {
            rotationX += -move.x;
            addY = move.y;

            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            Player.transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public Vector2 GetCameraFacing()
        {
            return new Vector2(rotationX, rotationY);
        }
    }
}
