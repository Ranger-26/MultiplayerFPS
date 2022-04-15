using Mirror;
using UnityEngine;

namespace Game.Player.Movement
{
    public class PlayerLook : NetworkBehaviour
    {
        public float lookSpeed => GameSettings.Sensitivity;

        public float lookXLimit = 90.0f;

        public Camera cam;
        public Transform cameraHolder;

        float rotationX = 0;
        float rotationY = 0;
        float addY;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            if (!hasAuthority)
            {
                cam.enabled = false;
                cam.GetComponent<AudioListener>().enabled = false;
                enabled = false;
            }
        }

        private void Update()
        {
            if (MenuOpen.IsOpen)
                return;

            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            rotationY = Input.GetAxis("Mouse X") * lookSpeed + addY;
            addY = 0f;
            UpdateCamera();
        }

        public void MoveCamera(float x, float y)
        {
            rotationX += -x;
            addY = y;

            UpdateCamera();
        }

        public void MoveCamera(float x)
        {
            rotationX += -x;

            UpdateCamera();
        }

        public void MoveCamera(Vector2 move)
        {
            rotationX += -move.x;
            addY = move.y;

            UpdateCamera();
        }

        public void MoveCameraVisual(float x, float y)
        {
            cam.transform.Rotate(Vector3.right * -x + Vector3.up * y, Space.Self);
        }

        public void MoveCameraVisual(float x)
        {
            cam.transform.Rotate(Vector3.right * -x, Space.Self);
        }

        public void MoveCameraVisual(Vector2 move)
        {
            cam.transform.Rotate(Vector3.right * -move.x + Vector3.up * move.y, Space.Self);
        }

        public void SetCameraVisual(float x, float y)
        {
            cam.transform.localRotation = Quaternion.Euler(Vector3.right * -x + Vector3.up * y);
        }

        public void SetCameraVisual(float x)
        {
            cam.transform.localRotation = Quaternion.Euler(Vector3.right * -x + Vector3.up * cam.transform.localRotation.y);
        }

        public void SetCameraVisual(Vector2 move)
        {
            cam.transform.localRotation = Quaternion.Euler(Vector3.right * -move.x + Vector3.up * move.y);
        }

        public void SetCameraVisual(Quaternion move)
        {
            cam.transform.localRotation = move;
        }

        public void UpdateCamera()
        {
            cameraHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotationY, 0);
        }

        public Vector2 GetCameraFacing()
        {
            return new Vector2(rotationX, rotationY);
        }

        public Quaternion GetCameraVisualRotation()
        {
            return cam.transform.localRotation;
        }
    }
}
