using Mirror;
using UnityEngine;

namespace Game.Player.Movement
{
	public class PlayerMovement : NetworkBehaviour
	{
        public CharacterController controller;

        public float speed = 12f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;

        Vector3 velocity;

        //ground stuff
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        [HideInInspector]
        public float weight;
        public LayerMask groundMask;

        float tagging;

        bool isGrounded;
        
        private void Start()
        {
            if (!isLocalPlayer) enabled = false;
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

            controller.Move(move * (speed - speed * tagging - speed * weight) * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            tagging = Mathf.Clamp01(tagging - Time.deltaTime);

            // Debug
            if (Input.GetMouseButtonDown(1))
            {
                Tag(1f);
            }
        }

        public void Tag(float amount)
        {
            tagging = Mathf.Clamp01(tagging + amount);
        }
    }
}
