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
            Debug.Log("Damaging player on the server...");
            if (currentHealth - amount > 0)
            {
                currentHealth -= amount;
                TargetDamagePlayer();
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
            Debug.Log($"You were damaged! New health is now {currentHealth}");
        }
    }
}