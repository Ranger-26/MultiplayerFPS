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
            if (Input.GetKeyDown(KeyCode.Mouse0)) Shoot();
        }

        private void Shoot()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
            CmdShoot(ray);
        }
        
        [Command]
        private void CmdShoot(Ray ray)
        {
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, curGun.Range, curGun.HitLayers))
            {
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
