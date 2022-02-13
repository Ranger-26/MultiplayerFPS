using System;
using Mirror;
using UnityEngine;

namespace Game.Player.Damage
{
    public class DamagePart : MonoBehaviour, IDamageable
    {
        public int damageMultiplier;

        private HealthController hc;
        
        [ServerCallback]
        private void Start()
        {
            hc = GetComponentInParent<HealthController>();
        }

        [Server]
        public void ServerDamage(int amount)
        {
            Debug.Log("Callling damage on player part...");
            hc.ServerDamagePlayer(amount * damageMultiplier);
        }
    }
}