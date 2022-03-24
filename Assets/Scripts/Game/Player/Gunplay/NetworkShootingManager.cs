using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game.Player.Damage;
using Game.Player.Gunplay.IdentifierComponents;
using Game.Player.Movement;
using Lobby;
using Mirror;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

namespace Game.Player.Gunplay
{
    public class NetworkShootingManager : NetworkBehaviour
    {
        [Header("Gun storing")]
        public Gun curGun;
        
        [Header("Ammo info")]
        [SyncVar]
        public int currentAmmo;
        [SyncVar]
        public int reserveAmmo;
        [SyncVar] 
        public bool isReloading = false;

        [Header("Weapon Holder Transforms")] 
        public Transform primarySlot;

        public Transform secondarySlot;

        public Transform meleeSlot;
        
        
        public void Start()
        {
            currentAmmo = curGun.MaxAmmo;
            reserveAmmo = curGun.ReserveAmmo;
            if (primarySlot == null)
            {
                primarySlot = GetComponentInChildren<PrimarySlot>().transform;
            }
            if (secondarySlot == null)
            {
                secondarySlot = GetComponentInChildren<SecondarySlot>().transform;
            }
            if (meleeSlot == null)
            {
                meleeSlot = GetComponentInChildren<MeleeSlot>().transform;
            }
        }

        private void Update()
        {
            if (hasAuthority && Input.GetKeyDown(KeyCode.Mouse2))
            {
                Debug.Log("Trying to switch gun...");
                CmdSwitchGunSlot(GunIDs.Makarov, WeaponSlot.Melee);
            }
        }

        private int id => GetComponent<NetworkGamePlayer>().playerId;
        
        #region ServerShootingLogic
        [Command]
        public void CmdShoot(Vector3 start, Vector3 forward, Vector3 visualFiringPoint)
        {
            ServerShoot(start, forward, id, visualFiringPoint);
        }

        [Command]
        public void CmdAmmo()
        {
            if (currentAmmo <= 0) return;
            currentAmmo--;
        }
        
        [Server]
        private void ServerShoot(Vector3 start, Vector3 forward, int id, Vector3 visualFiringPoint)
        {
            RaycastHit _hit;
            if (Physics.Raycast(start, forward, out _hit, curGun.Range, curGun.HitLayers))
            {
                Debug.DrawRay(start, forward * curGun.Range, Color.green, 1f);
                // Debug.Log($"Server: {start}, {forward}, player {id}");
                Debug.Log($"Hit something! {_hit.transform.name}, position {_hit.point}, shot by from player {id}");
                                           
                ServerHit(_hit, visualFiringPoint);
                
                DamagePart part = _hit.transform.gameObject.GetComponentInChildren<DamagePart>();
                if (part != null)
                {
                    Debug.Log($"Found body part {part.bodyPart} on {_hit.transform.name} when raycasting! ");
                    float multiplier;
                    switch (part.bodyPart)
                    {
                        case BodyPart.Head:
                            multiplier = curGun.HeadMultiplier;
                            break;
                        default:
                            multiplier = 1;
                            break;
                    }
                    part.ServerTag(curGun.Tagging);
                    part.ServerDamage(curGun.Damage, multiplier);
                }
            }
            else
            {
                ServerTracer(visualFiringPoint, start + forward * curGun.Range);
            }
        }
        
        [Server]
        private void ServerHit(RaycastHit _hit, Vector3 visualFiringPoint)
        {
            if (curGun.HitObject != null)
            {
                GameObject hit = Instantiate(curGun.HitObject, _hit.point, Quaternion.LookRotation(_hit.normal));
                NetworkServer.Spawn(hit);
            }

            if (curGun.HitDecal != null)
            {
                GameObject decal = Instantiate(curGun.HitDecal, _hit.point, Quaternion.LookRotation(-_hit.normal));
                // decal.transform.parent = _hit.transform.GetComponentInChildren<MeshRenderer>().transform;
                NetworkServer.Spawn(decal);
            }

            ServerTracer(visualFiringPoint, _hit.point);

            if (curGun.HitSounds.Length != 0)
            {
                AudioSystem.PlaySound(curGun.HitSounds[UnityEngine.Random.Range(0, curGun.HitSounds.Length - 1)], _hit.point, curGun.SoundMaxDistance, curGun.SoundVolume, 1f, 1f, curGun.SoundPriority);
            }
        }

        [Server]
        private void ServerTracer(Vector3 start, Vector3 end)
        {
            if (curGun.Tracer != null)
            {
                NetworkTracer ln = Instantiate(curGun.Tracer, transform.position, transform.rotation).GetComponent<NetworkTracer>();
                ln.start = start;
                ln.end = end;
                NetworkServer.Spawn(ln.gameObject);
            }
        }
        
        #endregion

        #region ReloadingLogic
        [Command]
        public void CmdReload()
        {
            if (currentAmmo == curGun.MaxAmmo || reserveAmmo <= 0) return; 
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
        #endregion

        #region GunSwitchLogic

        [Command]
        public void CmdSwitchGunSlot(GunIDs newGun, WeaponSlot slot)
        {
            Debug.Log("Calling command...");
            RpcSwitchGunSlot(newGun, slot);
        }

        [ClientRpc]
        public void RpcSwitchGunSlot(GunIDs newGun, WeaponSlot slot)
        {
            Debug.Log("Calling rpc...");
            GameObject newGunModel = Instantiate(GunDatabase.idsToModels[newGun].gameObject, transform.position, Quaternion.identity);

            switch (slot)
            {
                case WeaponSlot.Primary:
                    Destroy(primarySlot.GetChild(0).gameObject);
                    newGunModel.transform.parent = primarySlot;
                    break;
                case WeaponSlot.Secondary:
                    Destroy(secondarySlot.GetChild(0).gameObject);
                    newGunModel.transform.parent = secondarySlot;
                    break;
                case WeaponSlot.Melee:
                    Destroy(meleeSlot.GetChild(0).gameObject);
                    newGunModel.transform.parent = meleeSlot;
                    break;
            }
            
            newGunModel.transform.localPosition = Vector3.zero;
            newGunModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        
        #endregion
    }
}
