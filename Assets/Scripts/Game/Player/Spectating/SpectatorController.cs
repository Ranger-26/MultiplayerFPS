using Mirror;
using UnityEngine;

namespace Game.Player.Spectating
{
    public class SpectatorController : NetworkBehaviour
    {
        public override void OnStartLocalPlayer()
        {
            for(int i = 0; i < Camera.main.transform.childCount; i++)
            {
                Destroy(Camera.main.transform.GetChild(i).gameObject);
            }
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 0, 0);
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
