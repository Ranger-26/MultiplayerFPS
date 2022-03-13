using Mirror;
using Networking;
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
            TargetDamagePlayer(currentHealth);
            ServerKillPlayer();
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
            Debug.Log("You died!");
        }

        [Server]
        private void ServerKillPlayer()
        {
            NetworkManagerScp nm = (NetworkManagerScp) NetworkManager.singleton;
            GameObject deadPlayer = Instantiate(nm.deadPlayerPrefab,
                gameObject.transform.position, Quaternion.identity);
            NetworkServer.Spawn(deadPlayer);
            NetworkServer.ReplacePlayerForConnection(connectionToClient, deadPlayer);
            GameObject rag = Instantiate(nm.ragDoll, transform.position, Quaternion.identity);
            Destroy(gameObject);
            NetworkServer.Spawn(rag);
            TargetDeathPlayer();
        }
    }
}