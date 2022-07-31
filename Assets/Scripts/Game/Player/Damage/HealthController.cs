using Game.GameLogic.PlayerManagment;
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
        public void ServerDamagePlayer(float amount)
        {
            Debug.Log("Damaging player on the server...");
            if (currentHealth - amount > 0)
            {
                currentHealth -= (int)amount;
                TargetDisplayHealth(currentHealth);
                return;
            }
            
            currentHealth = 0;
            TargetDisplayHealth(0);
            ServerKillPlayer();
        }

        [Server]
        public void ServerHealPlayer(float amount)
        {
            currentHealth += (int) amount;
            if (currentHealth > 100)
            {
                currentHealth = 100;
            }
            TargetDisplayHealth(currentHealth);
        }
        
        [ClientRpc]
        public void RpcDamagePlayer()
        {
            // play sound maybe?
        }

        [TargetRpc]
        private void TargetDisplayHealth(int newHealth)
        {
            GameUiManager.Instance.UpdateHealthUI(newHealth);
        }

        [Server]
        private void ServerKillPlayer()
        {
            
            GameObject deadPlayer = Instantiate(NetworkManagerScp.Instance.deadPlayerPrefab,
                transform.position, Quaternion.identity);
            Debug.Log($"Player {GetComponent<NetworkGamePlayer>().playerId} died!");
            deadPlayer.GetComponent<NetworkGamePlayer>().playerName = GetComponent<NetworkGamePlayer>().playerName;
            deadPlayer.GetComponent<NetworkGamePlayer>().playerId = GetComponent<NetworkGamePlayer>().playerId;
            deadPlayer.GetComponent<NetworkGamePlayer>().role = GetComponent<NetworkGamePlayer>().role;
            deadPlayer.GetComponent<NetworkGamePlayer>().isSpectating = true;
            NetworkServer.Spawn(deadPlayer);
            GameObject rag = Instantiate(NetworkManagerScp.Instance.ragDoll, transform.position, Quaternion.identity);
            NetworkServer.Spawn(rag);
            NetworkServer.ReplacePlayerForConnection(connectionToClient, deadPlayer);
            PlayerManager.Instance.TryHandleDeadPlayer(GetComponent<NetworkGamePlayer>(), deadPlayer.GetComponent<NetworkGamePlayer>());
            Destroy(gameObject);
        }
    }
}