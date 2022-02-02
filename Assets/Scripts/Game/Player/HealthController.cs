using Mirror;

namespace Game.Player
{
    public class HealthController : NetworkBehaviour
    {
        [SyncVar]
        public int curHealth;
    }
}