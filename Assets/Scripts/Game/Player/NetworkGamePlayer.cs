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
            Camera cam;
            cam = GetComponentInChildren<Camera>();
            if (cam == null && isSpectating)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                cam = GetComponentInChildren<Camera>();
                GameUiManager.Instance.OnDie();
            }
            else
            {
                GameUiManager.Instance.OnRespawn();
            }
            cam.transform.tag = "MainCamera";
            gameObject.tag = "Player";
            localPlayer = this;
        }

        public override string ToString()
        {
            return $"Name {playerName}, Id: {playerId}";
        }
    }
}