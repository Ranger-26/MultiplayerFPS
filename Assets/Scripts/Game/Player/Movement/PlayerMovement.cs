using Mirror;
using System;
using AudioUtils;
using UnityEngine;

namespace Game.Player.Movement
{
	public class PlayerMovement : NetworkBehaviour
	{
        public CharacterController controller;

        public float speed = 5f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;
        public float StepDistance = 1.2f;

        public AudioClip[] stepClips;

        Vector3 velocity;
        Vector3 previousStepLocation;

        //ground stuff
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        [HideInInspector]
        public float weight;
        public LayerMask groundMask;

        [HideInInspector]
        public bool canMakeSound;

        float tagging;
        float airTime;

        bool isGrounded;
        bool LandTagged;
        
        private void Start()
        {
            if (!isLocalPlayer) enabled = false;

            previousStepLocation = transform.position;

            LandTagged = true;

            canMakeSound = true;
        }

        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            if (isGrounded)
            {
                if (canMakeSound)
                {
                    if (Vector3.Distance(previousStepLocation, transform.position) >= StepDistance)
                    {
                        previousStepLocation = transform.position;
                        AudioSystem.NetworkPlaySound(stepClips[UnityEngine.Random.Range(0, stepClips.Length - 1)], transform.position, 20f, 0.3f, 1f, 1f, 128);
                    }
                }
            }

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                if (MenuOpen.IsOpen)
                    return;

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            if (!isGrounded)
            {
                airTime += Time.deltaTime;

                if (airTime >= 0.5f)
                    LandTagged = false;
            }
            else
            {
                airTime = 0f;
            }

            if (!LandTagged && isGrounded)
            {
                Tag(0.8f);
                previousStepLocation = transform.position;
                AudioSystem.NetworkPlaySound(stepClips[UnityEngine.Random.Range(0, stepClips.Length - 1)], transform.position, 20f, 0.4f, 1f, 1f, 128);
                LandTagged = true;
            }

            float x = Input.GetAxis(StringKeys.InputHorizontal) * Convert.ToInt32(!MenuOpen.IsOpen);
            float z = Input.GetAxis(StringKeys.InputVertical) * Convert.ToInt32(!MenuOpen.IsOpen);

            Vector3 move = transform.right * x + transform.forward * z;
            move = Vector3.ClampMagnitude(move, 1f);

            float moddedSpeed = speed - speed * weight;

            controller.Move(move * (moddedSpeed - moddedSpeed * tagging) * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            tagging = Mathf.Clamp01(tagging - Time.deltaTime * 0.9f);
        }

        
        public void Tag(float amount)
        {
            tagging = Mathf.Clamp01(tagging + amount);
        }

        [TargetRpc]
        public void TargetTag(float amount) => Tag(amount);
    }
}
