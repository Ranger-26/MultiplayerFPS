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
            TargetDamagePlayer(0);
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

            Camera camera = GetComponentInChildren<Camera>();
            
            for (int i = 0; i < camera.transform.childCount; i++)
            { 
                Destroy(camera.transform.GetChild(i).gameObject);
            }
            camera.transform.rotation = Quaternion.Euler(0,0,0);
            
            camera.transform.parent = null;
            

            GameObject deadPlayer = Instantiate(NetworkManagerScp.Instance.deadPlayerPrefab,
                transform.position, Quaternion.identity);
            Debug.Log($"Player {GetComponent<NetworkGamePlayer>().playerId} died!");
            NetworkServer.Spawn(deadPlayer);
            GameObject rag = Instantiate(NetworkManagerScp.Instance.ragDoll, transform.position, Quaternion.identity);
            NetworkServer.Spawn(rag);
            NetworkServer.ReplacePlayerForConnection(connectionToClient, deadPlayer);
            Destroy(gameObject);
        }
    }
}