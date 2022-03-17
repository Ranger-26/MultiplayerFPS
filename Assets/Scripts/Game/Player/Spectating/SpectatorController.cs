using System;
using Mirror;
using UnityEngine;

namespace Game.Player.Spectating
{
    public class SpectatorController : NetworkBehaviour
    {
        private void Start()
        {
            if (hasAuthority)
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        
        public float movementForwardMultiplier = 4f;
        public float movementSideMultiplier = 4f;

        private string forwardAxis = "Vertical";
        private string horizontalAxis = "Horizontal";

        void Update () {
            float forwardMovement = Input.GetAxis(forwardAxis) * movementForwardMultiplier * Time.deltaTime;
            float horizontalMovement = Input.GetAxis(horizontalAxis) * movementSideMultiplier * Time.deltaTime;
            Vector3 movementDelta = new Vector3(horizontalMovement, 0, forwardMovement);
            transform.position += transform.TransformDirection(movementDelta);
        }
    }   
}
