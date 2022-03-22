using System;
using Mirror;
using UnityEngine;

namespace Game.Player.Spectating
{
    public class SpectatorController : NetworkBehaviour
    {
        public float movementForwardMultiplier = 4f;
        public float movementSideMultiplier = 4f;

        private string forwardAxis = "Vertical";
        private string horizontalAxis = "Horizontal";

        void Update () 
        {
            if (MenuOpen.IsOpen)
                return;

            float forwardMovement = Input.GetAxis(forwardAxis) * movementForwardMultiplier * Time.deltaTime;
            float horizontalMovement = Input.GetAxis(horizontalAxis) * movementSideMultiplier * Time.deltaTime;
            Vector3 movementDelta = new Vector3(horizontalMovement, 0f, forwardMovement);
            transform.position += transform.TransformDirection(movementDelta);
        }
    }   
}
