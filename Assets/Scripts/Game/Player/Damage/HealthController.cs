using Mirror;
using UnityEngine;

namespace Game.Player.Damage
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
                TargetDamagePlayer(currentHealth);
                return;
            }

            currentHealth = 0;
            TargetDeathPlayer();
        }

        [ClientRpc]
        public void RpcDamagePlayer()
        {
            //play sound maybe?
        }

        [TargetRpc]
        private void TargetDamagePlayer(int newHealth)
        {
            GameUiManager.Instance.UpdateHealthUI(newHealth);
        }

        [TargetRpc]
        private void TargetDeathPlayer()
        {
            TargetDamagePlayer(0);
            Debug.Log("You died!");
        }
    }
}