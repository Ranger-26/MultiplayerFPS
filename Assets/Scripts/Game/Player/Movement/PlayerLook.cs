using Mirror;
using UnityEngine;

namespace Game.Player.Movement
{
    public class PlayerLook : NetworkBehaviour
    {
        public static PlayerLook Instance;

        public float lookSpeed = 1.0f;
        public float lookXLimit = 90.0f;

        Transform cam;

        float rotationX = 0;
        float rotationY = 0;
        float addY;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                enabled = false;
            }

            cam = transform.GetChild(0);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            if (!isLocalPlayer) enabled = false;
        }

        private void Update()
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            rotationY = Input.GetAxis("Mouse X") * lookSpeed + addY;
            addY = 0f;
            cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public void MoveCamera(float x, float y)
        {
            rotationX += -x;
            addY = y;

            cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public void MoveCamera(float x)
        {
            rotationX += -x;

            cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public void MoveCamera(Vector2 move)
        {
            rotationX += -move.x;
            addY = move.y;

            cam.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public Vector2 GetCameraFacing()
        {
            return new Vector2(rotationX, rotationY);
        }
    }
}
