using System;
using Inputs;
using Mirror;
using UnityEngine;

namespace Game.Player.Spectating
{
    public class SpectatorController : NetworkBehaviour
    {
        public float movementForwardMultiplier = 4f;
        public float movementSideMultiplier = 4f;

        private void Start()
        {
            //if (!hasAuthority) enabled = false;
        }

        void Update () 
        {
            if (MenuOpen.IsOpen)
                return;
            

            var input = GameInputManager.PlayerActions.WASD.ReadValue<Vector2>();
            
            Debug.Log(input);
            
            Vector3 movementDelta = new Vector3(input.x * movementForwardMultiplier * Time.deltaTime, 0f, input.y* movementSideMultiplier * Time.deltaTime);
            transform.position += transform.TransformDirection(movementDelta);
        }
    }   
}
