using System;
using Mirror;
using UnityEngine;

namespace Game.Player.Damage
{
    public class DamagePart : MonoBehaviour, IDamageable
    {
        public BodyPart bodyPart;

        private HealthController hc;
        
        private void Start()
        {
            hc = GetComponentInParent<HealthController>();
        }
        
        [Server]
        public void ServerDamage(int amount, int multiplier)
        {
            hc.ServerDamagePlayer(amount * multiplier);
        }
    }

    public enum BodyPart
    {
        Head,
        Torso,
        Limb
    }
}