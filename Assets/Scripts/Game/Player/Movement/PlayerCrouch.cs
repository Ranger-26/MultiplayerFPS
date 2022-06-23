using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

namespace Game.Player.Movement
{
    public class PlayerCrouch : NetworkBehaviour
    {
        public float CrouchRate;
        public float CrouchHeight;
        public float StandingHeight;

        public float CrouchSpeed;
        public float WalkingSpeed;
        public float NormalSpeed;

        [HideInInspector]
        public float crouchFactor;

        [HideInInspector]
        public bool isCrouching;
        [HideInInspector]
        public bool isWalking;

        bool crouching;

        CharacterController controller;
        PlayerMovement playerMovement;

        PlayerInput PI;

        private InputAction moveAction;
        
        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            controller = GetComponent<CharacterController>();
            controller.height = StandingHeight;
        }

        private void Start()
        {
            if (!isLocalPlayer) enabled = false;

            PI = GamePlayerInput.Instance.playerInput;

            PI.actions.FindAction("Crouch").performed += UpdateCrouch;
            PI.actions.FindAction("Crouch").canceled += UpdateCrouch;

            PI.actions.FindAction("Walk").performed += UpdateWalk;
            PI.actions.FindAction("Walk").canceled += UpdateWalk;
        }

        private void Update()
        {
            crouchFactor = Mathf.Clamp01(crouchFactor + Time.deltaTime * CrouchRate * (crouching ? 1 : -1));

            playerMovement.speed = Mathf.Lerp(NormalSpeed, CrouchSpeed, crouchFactor);

            float previousHeight = controller.height;

            if (!isCrouching)
            {
                playerMovement.speed = isWalking ? WalkingSpeed : NormalSpeed;
            }

            float h = Mathf.Lerp(StandingHeight, CrouchHeight, crouchFactor);
            controller.height = h;

            if (playerMovement.isGrounded)
                controller.Move(new Vector3(0f, h - previousHeight, 0f));

            isCrouching = crouchFactor >= 0.5f;

            playerMovement.canMakeSound = !isCrouching && !isWalking;
        }

        public void UpdateCrouch(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed) crouching = true;
            else if (callbackContext.canceled) crouching = false;
        }

        public void UpdateWalk(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed) isWalking = true;
            else if (callbackContext.canceled) isWalking = false;
        }

        private void OnDestroy()
        {
            PI.actions.FindAction("Crouch").performed -= UpdateCrouch;
            PI.actions.FindAction("Crouch").canceled -= UpdateCrouch;

            PI.actions.FindAction("Walk").performed -= UpdateWalk;
            PI.actions.FindAction("Walk").canceled -= UpdateWalk;
        }
    }
}

