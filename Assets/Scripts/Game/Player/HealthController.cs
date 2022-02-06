using Mirror;

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
        }

        [ClientRpc]
        public void RpcDamagePlayer()
        {
            
        }

        [TargetRpc]
        public void TargetDamagePlayer()
        {
            
        }
    }
}