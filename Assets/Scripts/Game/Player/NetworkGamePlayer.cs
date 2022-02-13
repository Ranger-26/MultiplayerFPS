using System;
using Game.GameLogic;
using Game.Player.Damage;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class NetworkGamePlayer : NetworkBehaviour
    {
        [SyncVar] public Role role;
    }
}