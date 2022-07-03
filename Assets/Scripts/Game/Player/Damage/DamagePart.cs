using System;
using Game.Player.Movement;
using Game.UI;
using Mirror;
using UnityEngine;

namespace Game.Player.Damage
{
    public class DamagePart : MonoBehaviour, IDamageable
    {
        public BodyPart bodyPart;

        private HealthController _hc;

        private PlayerMovement _playerMovement;

        public NetworkGamePlayer Player;
        
        private void Start()
        {
            _hc = GetComponentInParent<HealthController>();
            _playerMovement = GetComponentInParent<PlayerMovement>();
            Player = GetComponentInParent<NetworkGamePlayer>();
        }
        
        [Server]
        public void ServerDamage(float amount, float multiplier)
        {
            _hc.ServerDamagePlayer(amount * multiplier);
        }
        
        [Server]
        public void ServerTag(float tagging)
        {
            _playerMovement.TargetTag(tagging);
        } 
    }

    public enum BodyPart
    {
        Head,
        Torso,
        Limb
    }
}