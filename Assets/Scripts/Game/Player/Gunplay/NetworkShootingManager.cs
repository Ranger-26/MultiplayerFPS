using System;
using Game.Player.Damage;
using Game.Player.Movement;
using Mirror;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Player.Gunplay
{
    public class NetworkShootingManager : NetworkBehaviour
    {
        public Gun curGun;
        
        [SerializeField]
        Transform spreadPoint;

        private void Update()
        {
            if(!hasAuthority) return;
            if (Input.GetKeyDown(KeyCode.Mouse0)) CmdShoot();
        }

        [Command]
        private void CmdShoot() => ServerProcessShot();
        
        [Server]
        private void ServerProcessShot()
        {
            RaycastHit _hit;
            if (Physics.Raycast(spreadPoint.position, spreadPoint.forward, out _hit, curGun.Range, curGun.HitLayers))
            {
                Debug.DrawRay(spreadPoint.position, spreadPoint.forward * curGun.Range, Color.green, 0.2f);

                Debug.Log($"Hit something! {_hit.transform.name}");

                DamagePart part = _hit.transform.gameObject.GetComponentInChildren<DamagePart>();
                if (part != null)
                {
                    Debug.Log($"Found body part {part.bodyPart} when raycasting! ");
                    //Damage player
                }
            }
        }

        
    }
}
