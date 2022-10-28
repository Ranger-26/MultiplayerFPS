using Mirror;
using System;
using AudioUtils;
using Game.UI;
using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Movement
{
	public class PlayerMovement : NetworkBehaviour
	{
        public CharacterController controller;

        public float speed = 5f;
        public float acceleration = 1.5f;
        public float deceleration = 1.5f;
        public float speedCap = 2f;
        public float gravity = -9.81f;
        public float jumpHeight = 3f;
        public float StepDistance = 1.2f;

        public AudioClip[] stepClips;

        [HideInInspector]
        public Vector2 movementInput;
        Vector2 vel;

        Vector3 velocity;
        Vector3 previousStepLocation;

        //ground stuff
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public float noTagDistance = 0.6f;
        [HideInInspector]
        public float weight;
        public LayerMask groundMask;

        [HideInInspector]
        public bool canMakeSound;
        [HideInInspector]
        public bool isGrounded;
        [HideInInspector]
        public Vector2 velocityGun;

        Vector3 _prevPosition;

        float airTime;

        bool LandTagged;
        
        private void Start()
        {
            if (!isLocalPlayer) enabled = false;

            previousStepLocation = transform.position;
            _prevPosition = transform.position;

            LandTagged = true;

            canMakeSound = true;
            
            GameInputManager.PlayerActions.Jump.performed += Jump;
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
                        AudioSystem.NetworkPlaySound(Sound: stepClips[UnityEngine.Random.Range(0, stepClips.Length - 1)], Parent: GetComponent<NetworkTransform>(), Position: transform.position, MaxDistance: 20f, Volume: 0.3f, Priority: 128);
                    }
                }
            }

            if (!isGrounded)
            {
                airTime += Time.deltaTime;

                if (airTime >= 0.5f || !Physics.CheckSphere(groundCheck.position, noTagDistance, groundMask))
                    LandTagged = false;
            }
            else
            {
                airTime = 0f;
            }

            movementInput = GameInputManager.PlayerActions.WASD.ReadValue<Vector2>();
            float x = movementInput.x * Convert.ToInt16(!MenuOpen.IsOpen);
            float z = movementInput.y * Convert.ToInt16(!MenuOpen.IsOpen);

            float moddedSpeed = speed - speed * weight;

            Vector2 temp = Vector2.zero;
            temp.x = Mathf.Clamp(vel.x + x * acceleration * Time.deltaTime, -speedCap, speedCap);
            temp.y = Mathf.Clamp(vel.y + z * acceleration * Time.deltaTime, -speedCap, speedCap);
            vel = Vector3.ClampMagnitude(temp, 1f);

            controller.Move((transform.forward * vel.y + transform.right * vel.x) * moddedSpeed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            vel = Vector2.MoveTowards(vel, Vector2.zero, deceleration * Time.deltaTime);

            if (!LandTagged && isGrounded)
            {
                Tag(0.6f);
                previousStepLocation = transform.position;
                AudioSystem.NetworkPlaySound(Sound: stepClips[UnityEngine.Random.Range(0, stepClips.Length - 1)], Position: transform.position, Parent: GetComponent<NetworkTransform>(), MaxDistance: 20f, Volume: 0.4f, SpatialBlend: 1f, Priority: 128);
                LandTagged = true;
            }
        }

        private void FixedUpdate()
        {
            Vector3 tempvel = (transform.position - _prevPosition) / Time.fixedDeltaTime;
            _prevPosition = transform.position;

            velocityGun = new Vector2(tempvel.x, tempvel.z);
        }


        public void Tag(float amount)
        {
            vel = Vector2.MoveTowards(vel, Vector2.zero, amount);
        }

        [TargetRpc]
        public void TargetTag(float amount) => Tag(amount);

        public void Jump(InputAction.CallbackContext callbackContext)
        {
            if (isGrounded)
            {
                if (MenuOpen.IsOpen)
                    return;

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void OnDestroy()
        {
            GameInputManager.PlayerActions.Jump.performed -= Jump;
        }
    }
}
