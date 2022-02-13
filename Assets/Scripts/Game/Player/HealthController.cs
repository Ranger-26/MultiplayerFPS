using Mirror;
using UnityEngine;

namespace Game.Player
{
    public class HealthController : NetworkBehaviour
    {
        [SyncVar]
        public int currentHealth;

        [Server]
        public void ServerDamagePlayer(int amount)
        {
            if (currentHealth - amount > 0)
            {
                currentHealth -= amount;
                return;
            }

            currentHealth = 0;
            TargetDamagePlayer();
        }

        [ClientRpc]
        public void RpcDamagePlayer()
        {
            
        }

        [TargetRpc]
        private void TargetDamagePlayer()
        {
            Debug.Log("You were damaged!");
        }
    }
}