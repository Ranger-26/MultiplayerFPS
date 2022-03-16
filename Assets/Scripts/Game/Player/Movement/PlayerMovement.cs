using Mirror;
using System;
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
        bool canJump;
        bool LandTagged;
        
        private void Start()
        {
            if (!isLocalPlayer) enabled = false;

            LandTagged = true;
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance / 2, groundMask);
            canJump = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (Input.GetButtonDown("Jump") && canJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            if (!isGrounded)
            {
                LandTagged = false;
            }

            if (!LandTagged && isGrounded)
            {
                Tag(0.5f);
                LandTagged = true;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * (speed - speed * (tagging - tagging * 0.75f * Convert.ToInt32(!isGrounded)) - speed * weight) * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            tagging = Mathf.Clamp01(tagging - Time.deltaTime * 0.7f);
        }

        
        public void Tag(float amount)
        {
            tagging = Mathf.Clamp01(tagging + amount);
        }

        [TargetRpc]
        public void TargetTag(float amount) => Tag(amount);
    }
}
