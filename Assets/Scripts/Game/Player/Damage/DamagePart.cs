using System;
using Mirror;
using UnityEngine;

namespace Game.Player.Damage
{
    public class DamagePart : MonoBehaviour
    {
        public int damageMultiplier;

        private HealthController hc;
        private void Start()
        {
            hc = GetComponentInParent<HealthController>();
        }

        [Server]
        public void Damage(int amount)
        {
            hc.ServerDamagePlayer(amount * damageMultiplier);
        }
    }
}