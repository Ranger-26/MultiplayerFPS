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
            playerMovement.speed = Input.GetKey(KeyCode.LeftControl) ? CrouchSpeed : NormalSpeed;

            if (!Input.GetKey(KeyCode.LeftControl))
            {
                playerMovement.speed = Input.GetKey(KeyCode.LeftShift) ? WalkingSpeed : NormalSpeed;
            }

            float h = Mathf.Lerp(StandingHeight, CrouchHeight, crouchFactor);
            controller.height = h;

            isCrouching = crouchFactor >= 0.8f;
            isWalking = Input.GetKey(KeyCode.LeftShift);

            playerMovement.canMakeSound = !isCrouching && !isWalking;
        }
    }
}

