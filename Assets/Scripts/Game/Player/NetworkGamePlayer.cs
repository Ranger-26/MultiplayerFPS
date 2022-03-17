using System;
using Game.GameLogic;
using Game.GameLogic.PlayerManagment;
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
        [SyncVar] public bool isSpectating;
        
        public static NetworkGamePlayer localPlayer;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            Camera camera = GetComponentInChildren<Camera>();
            camera.transform.tag = "MainCamera";
            localPlayer = this;
        }

        public override string ToString()
        {
            return $"Name {playerName}, Id: {playerId}";
        }
    }
}