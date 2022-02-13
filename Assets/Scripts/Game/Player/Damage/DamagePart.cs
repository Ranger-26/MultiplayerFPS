using System;
using Mirror;
using UnityEngine;

namespace Game.Player.Damage
{
    public class DamagePart : MonoBehaviour, IDamageable
    {
        public BodyPart bodyPart;

        private HealthController hc;
        
        [ServerCallback]
        private void Start()
        {
            hc = GetComponentInParent<HealthController>();
        }

        [Server]
        public void Damage(int amount, float damageMultiplier)
        {
            hc.ServerDamagePlayer((int)(amount * damageMultiplier));
        }
    }

    public enum BodyPart
    {
        Head,
        Torso,
        Limb
    }
}