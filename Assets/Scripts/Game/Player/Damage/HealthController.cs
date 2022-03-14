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
            Camera.main.transform.parent = null;
            Debug.Log("You died!");
        }

        [Server]
        private void ServerKillPlayer()
        {
            TargetDeathPlayer();
            for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++)
            { 
                Destroy(transform.GetChild(0).GetChild(0).transform.GetChild(i).gameObject);
            }
            transform.GetChild(0).GetChild(0).rotation = Quaternion.Euler(0,0,0);
            transform.GetChild(0).GetChild(0).parent = null;
            

            NetworkManagerScp nm = (NetworkManagerScp) NetworkManager.singleton;
            GameObject deadPlayer = Instantiate(nm.deadPlayerPrefab,
                transform.position, Quaternion.identity);
            Debug.Log($"Player {GetComponent<NetworkGamePlayer>().playerId} died!");
            NetworkServer.Spawn(deadPlayer);
            GameObject rag = Instantiate(nm.ragDoll, transform.position, Quaternion.identity);
            NetworkServer.Spawn(rag);
            NetworkServer.ReplacePlayerForConnection(connectionToClient, deadPlayer);
            Destroy(gameObject);
        }
    }
}