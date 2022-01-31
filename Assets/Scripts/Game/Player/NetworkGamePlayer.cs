using Game.GameLogic;
using Mirror;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class NetworkGamePlayer : NetworkBehaviour
    {
        [SyncVar] public Role role;
    }
}