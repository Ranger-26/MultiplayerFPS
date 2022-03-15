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

        public static NetworkGamePlayer localPlayer;
        private void Start()
        {
            healthController = GetComponent<HealthController>();
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            Camera camera = GetComponentInChildren<Camera>();
            camera.transform.tag = "MainCamera";
            localPlayer = this;
        }

        public override void OnStopClient()
        {
            if (!isServer) return;
            GameManager.Instance.TryRemovePlayer(this);
        }

        public override string ToString()
        {
            return $"Name {playerName}, Id: {playerId}";
        }
    }
}