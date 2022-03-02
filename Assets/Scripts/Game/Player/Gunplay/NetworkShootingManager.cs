using System;
using System.Collections;
using Game.Player.Damage;
using Game.Player.Movement;
using Mirror;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace Game.Player.Gunplay
{
    public class NetworkShootingManager : NetworkBehaviour
    {
        public Gun curGun;
        
        [SerializeField]
        Transform spreadPoint;

        [Header("Ammo")] 
        [SyncVar] 
        public int curReserveAmmo;
        
        private void Update()
        {
            if(!hasAuthority) return;
            if (Input.GetKey(KeyCode.Mouse0)) CmdShoot(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)));
        }

        [Command]
        private void CmdShoot(Ray ray)
        { 
            if (curReserveAmmo < 0) return;
            ServerShoot(ray);
        }

        #region ServerShootingLogic
        [Server]
        private void ServerShoot(Ray ray)
        {
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, curGun.Range, curGun.HitLayers))
            {
                Debug.DrawRay(spreadPoint.position, spreadPoint.forward * curGun.Range, Color.green, 0.2f);

                Debug.Log($"Hit something! {_hit.transform.name}, position {_hit.point}");

                Hit(_hit);

                DamagePart part = _hit.transform.gameObject.GetComponentInChildren<DamagePart>();
                if (part != null)
                {
                    Debug.Log($"Found body part {part.bodyPart} when raycasting! ");
                    // Damage player
                }
            }
        }

        [Server]
        private void Hit(RaycastHit _hit)
        {
            if (curGun.HitObject != null)
            {
                GameObject hit = Instantiate(curGun.HitObject, _hit.point, Quaternion.LookRotation(_hit.normal));
                NetworkServer.Spawn(hit);
            }

            if (curGun.HitDecal != null)
            {
                GameObject decal = Instantiate(curGun.HitDecal, _hit.point, Quaternion.LookRotation(-_hit.normal));
                decal.transform.parent = _hit.transform.GetComponentInChildren<MeshRenderer>().transform;
                NetworkServer.Spawn(decal);
            }
/*
            if (curGun.HitSounds.Length != 0)
            {
                AudioSystem.PlaySound(curGun.HitSounds[UnityEngine.Random.Range(0, curGun.HitSounds.Length - 1)], _hit.point, curGun.SoundMaxDistance, curGun.SoundVolume, 1f, 1f, curGun.SoundPriority);
            }*/
        }
        
        #endregion

        [Server]
        private IEnumerator Reload()
        {
            yield break;
        }
        
        
    }
}
