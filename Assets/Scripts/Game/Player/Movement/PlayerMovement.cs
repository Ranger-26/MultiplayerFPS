using Mirror;
using UnityEngine;

// CS: GO Movement by Alpharoah: https://www.youtube.com/watch?v=AVjbCn5i_rk (Slightly modified by SwiftKraft)

namespace Game.Player.Movement
{
	public class PlayerMovement : NetworkBehaviour
	{
        public static PlayerMovement Instance;

        public CharacterController controller;

        public float speed = 12f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;
        Vector3 velocity;

        //ground stuff
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;

        bool isGrounded;

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
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
