using System;
using Game.GameLogic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Player
{
    public class NetworkGamePlayer : NetworkBehaviour
    {
        [SyncVar] public Role role;

        public HealthController healthController;

        private void Start()
        {
            healthController = GetComponent<HealthController>();
        }
    }
}