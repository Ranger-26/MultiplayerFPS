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
        
        [Header("Player Info")]
        [SyncVar] public int playerId;
        [SyncVar] public string playerName;
        
        public HealthController healthController;
        
        private void Start()
        {
            healthController = GetComponent<HealthController>();
        }
    }
}