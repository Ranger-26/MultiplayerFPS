using Koenigz.PerfectCulling;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Movement
{
    public class PlayerLook : NetworkBehaviour
    {
        public float lookSpeed => GameSettings.current.Sensitivity;

        public float lookXLimit = 90.0f;

        public Camera cam;
        public Transform cameraHolder;
        public Transform visualCamera;
        public Transform aimPunchCamera;

        float rotationX = 0;
        float rotationY = 0;
        float addY;

        Vector2 mouse;

        PlayerInput PI;

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
                cam.GetComponent<PerfectCullingCamera>().enabled = false;
                enabled = false;
            }

            PI = GamePlayerInput.Instance.playerInput;

            PI.actions.FindAction("Look").performed += UpdateLook;
            PI.actions.FindAction("Look").canceled += UpdateLook;
        }

        public void UpdateLook(InputAction.CallbackContext callbackContext)
        {
            if (MenuOpen.IsOpen)
                return;

            mouse = callbackContext.ReadValue<Vector2>();

            rotationX += -mouse.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            rotationY = mouse.x * lookSpeed + addY;
            addY = 0f;

            UpdateCamera();
        }

        #region Main Camera Movement Overloads

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

        #endregion

        #region Visual Camera Movement Overloads

        public void MoveCameraVisual(float x, float y)
        {
            visualCamera.Rotate(Vector3.right * -x + Vector3.up * y, Space.Self);
        }

        public void MoveCameraVisual(float x)
        {
            visualCamera.Rotate(Vector3.right * -x, Space.Self);
        }

        public void MoveCameraVisual(Vector2 move)
        {
            visualCamera.Rotate(Vector3.right * -move.x + Vector3.up * move.y, Space.Self);
        }

        public void SetCameraVisual(float x, float y)
        {
            visualCamera.localRotation = Quaternion.Euler(Vector3.right * -x + Vector3.up * y);
        }

        public void SetCameraVisual(float x)
        {
            visualCamera.localRotation = Quaternion.Euler(Vector3.right * -x + Vector3.up * visualCamera.localRotation.y);
        }

        public void SetCameraVisual(Vector2 move)
        {
            visualCamera.localRotation = Quaternion.Euler(Vector3.right * -move.x + Vector3.up * move.y);
        }

        public void SetCameraVisual(Quaternion move)
        {
            visualCamera.localRotation = move;
        }

        #endregion

        #region Aim Punch Camera Movement Overloads

        public void MoveCameraAimPunch(float x, float y)
        {
            aimPunchCamera.Rotate(Vector3.right * -x + Vector3.up * y, Space.Self);
        }

        public void MoveCameraAimPunch(float x)
        {
            aimPunchCamera.Rotate(Vector3.right * -x, Space.Self);
        }

        public void MoveCameraAimPunch(Vector2 move)
        {
            aimPunchCamera.Rotate(Vector3.right * -move.x + Vector3.up * move.y, Space.Self);
        }

        public void SetCameraAimPunch(float x, float y)
        {
            aimPunchCamera.localRotation = Quaternion.Euler(Vector3.right * -x + Vector3.up * y);
        }

        public void SetCameraAimPunch(float x)
        {
            aimPunchCamera.localRotation = Quaternion.Euler(Vector3.right * -x + Vector3.up * visualCamera.localRotation.y);
        }

        public void SetCameraAimPunch(Vector2 move)
        {
            aimPunchCamera.localRotation = Quaternion.Euler(Vector3.right * -move.x + Vector3.up * move.y);
        }

        public void SetCameraAimPunch(Quaternion move)
        {
            aimPunchCamera.localRotation = move;
        }

        #endregion

        #region Misc Camera Functions

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
            return visualCamera.localRotation;
        }

        public Quaternion GetCameraAimPunchRotation()
        {
            return aimPunchCamera.localRotation;
        }

        #endregion
    }
}
