using UnityEngine;
using Mirror;

namespace Game.Player.Movement
{
    public class PlayerCrouch : NetworkBehaviour
    {
        public float CrouchHeight;
        public float StandingHeight;

        public float CrouchSpeed;
        public float NormalSpeed;

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
            controller.height = Input.GetKey(KeyCode.LeftControl) ? CrouchHeight : StandingHeight;
            playerMovement.moveSpeed = Input.GetKey(KeyCode.LeftControl) ? CrouchSpeed : NormalSpeed;
        }
    }
}

