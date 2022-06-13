using UnityEngine;
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

        CharacterController controller;
        PlayerMovement playerMovement;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            controller = GetComponent<CharacterController>();
            controller.height = StandingHeight;
        }

        private void Start()
        {
            if (!isLocalPlayer) enabled = false;
        }

        private void Update()
        {
            crouchFactor = Mathf.Clamp01(crouchFactor + Time.deltaTime * CrouchRate * (Input.GetKey(KeyCode.LeftControl) ? 1 : -1));

            playerMovement.speed = Mathf.Lerp(NormalSpeed, CrouchSpeed, crouchFactor);

            float previousHeight = controller.height;

            if (!isCrouching)
            {
                playerMovement.speed = Input.GetKey(KeyCode.LeftShift) ? WalkingSpeed : NormalSpeed;
            }

            float h = Mathf.Lerp(StandingHeight, CrouchHeight, crouchFactor);
            controller.height = h;

            if (playerMovement.isGrounded)
                controller.Move(new Vector3(0f, h - previousHeight, 0f));

            isCrouching = crouchFactor >= 0.5f;
            isWalking = Input.GetKey(KeyCode.LeftShift);

            playerMovement.canMakeSound = !isCrouching && !isWalking;
        }
    }
}

