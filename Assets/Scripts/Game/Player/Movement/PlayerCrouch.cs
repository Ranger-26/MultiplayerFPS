using UnityEngine;

namespace Game.Player.Movement
{
    public class PlayerCrouch : MonoBehaviour
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

        private void Update()
        {
            controller.height = Input.GetKey(KeyCode.LeftControl) ? CrouchHeight : StandingHeight;
            playerMovement.moveSpeed = Input.GetKey(KeyCode.LeftControl) ? CrouchSpeed : NormalSpeed;
        }
    }
}

