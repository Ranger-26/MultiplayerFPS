using System;
using System.Collections;
using System.Runtime.InteropServices;
using Game.Player.Damage;
using Game.Player.Movement;
using Lobby;
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

        [SyncVar]
        public int currentAmmo;
        [SyncVar]
        public int reserveAmmo;

        [SyncVar] public bool isReloading = false;
        public void Start()
        {
            currentAmmo = curGun.MaxAmmo;
            reserveAmmo = curGun.ReserveAmmo;
        }

        private int id => GetComponent<NetworkGamePlayer>().playerId;
        
        private void Update()
        {
            
        }
        
        [Command]
        public void CmdShoot(Ray ray)
        {
            currentAmmo--;
            for (int i = 0; i < curGun.BulletCount; i++)
            {
                ServerShoot(ray);
            }
            
        }

        #region ServerShootingLogic
        [Server]
        private void ServerShoot(Ray ray)
        {
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, curGun.Range, curGun.HitLayers))
            {
                Debug.DrawRay(spreadPoint.position, spreadPoint.forward * curGun.Range, Color.green, 0.2f);

                Debug.Log($"Hit something! {_hit.transform.name}, position {_hit.point}, shot by from player {id}");

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
                //decal.transform.parent = _hit.transform.GetComponentInChildren<MeshRenderer>().transform;
                NetworkServer.Spawn(decal);
            }
/*
            if (curGun.HitSounds.Length != 0)
            {
                AudioSystem.PlaySound(curGun.HitSounds[UnityEngine.Random.Range(0, curGun.HitSounds.Length - 1)], _hit.point, curGun.SoundMaxDistance, curGun.SoundVolume, 1f, 1f, curGun.SoundPriority);
            }*/
        }
        
        #endregion

        [Command]
        public void CmdReload()
        {
            if (currentAmmo > 0 || reserveAmmo <= 0) return; 
            isReloading = true;
            StartCoroutine(Reload());
        } 
        
        [Server]
        private IEnumerator Reload()
        {
            yield return new WaitForSeconds(curGun.ReloadTime);
            int exchange = curGun.MaxAmmo - currentAmmo;
            if (reserveAmmo <= exchange)
            {
                currentAmmo += reserveAmmo;
                reserveAmmo = 0;
            }
            else
            {
                currentAmmo += exchange;
                reserveAmmo -= exchange;
            }

            isReloading = false;
        }
    }
}
