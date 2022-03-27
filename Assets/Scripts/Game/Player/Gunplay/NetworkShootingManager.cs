using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Game.Player.Damage;
using Game.Player.Gunplay.IdentifierComponents;
using Game.Player.Movement;
using Lobby;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

namespace Game.Player.Gunplay
{
    public class NetworkShootingManager : NetworkBehaviour
    {
        [Header("Gun storing")]
        public Gun curGun;

        //[SyncVar] public GunIDs curGunId;
        [SyncVar(hook = nameof(OnCurWeaponSlotChanged))] public WeaponSlot heldWeaponSlot = WeaponSlot.Primary;
        
        private readonly SyncDictionary<WeaponSlot, GunIDs> allGuns = new SyncDictionary<WeaponSlot, GunIDs>();

        [SerializeField]
        private List<Gun> allGunScriptables = new List<Gun>();


        [Header("Ammo info")]
        [SyncVar]
        public int currentAmmo;
        [SyncVar]
        public int reserveAmmo;
        [SyncVar] 
        public bool isReloading = false;

        private readonly SyncDictionary<GunIDs, GunAmmo> gunsToAmmo = new SyncDictionary<GunIDs, GunAmmo>(); 

        [Header("Weapon Holder Transforms")] 
        public Transform primarySlot;

        public Transform secondarySlot;

        public Transform meleeSlot;

        
        private int id => GetComponent<NetworkGamePlayer>().playerId;
        
        
        #region UnityCallbacks
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

            if (isServer)
            {
                ServerInitGuns();
            }
        }
        
        private void Update()
        {
            if (hasAuthority && Input.GetKeyDown(KeyCode.Mouse2))
            {
                Debug.Log("Trying to switch gun...");
                CmdSwitchGunSlot(heldWeaponSlot == WeaponSlot.Primary ? WeaponSlot.Secondary : WeaponSlot.Primary);
            }
        }
        #endregion
        
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
                    NetworkGamePlayer playerMain = part.GetComponentInParent<NetworkGamePlayer>();
                    if (playerMain != null && playerMain.playerId == id)
                    {
                        Debug.Log($"Player {id} trying to shoot themselves.");
                        return;
                    }
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
        [Server]
        private void ServerInitGuns()
        {
            allGuns.Add(WeaponSlot.Primary, GunIDs.None);
            allGuns.Add(WeaponSlot.Secondary, GunIDs.None);
            allGuns.Add(WeaponSlot.Melee, GunIDs.None);

            foreach (var gun in GetComponentsInChildren<GunViewModel>(true))
            {
                if (GunDatabase.TryGetGun(gun.gunId, out Gun newGun))
                {
                    allGuns[newGun.GunSlot] = gun.gunId;
                    gunsToAmmo.Add(gun.gunId, new GunAmmo(newGun.MaxAmmo, newGun.ReserveAmmo));
                }
            }
            
            allGuns.Keys.ToList().ForEach(x => Debug.Log($"All gun keys: {x}, player {id}"));
            allGuns.Values.ToList().ForEach(x => Debug.Log($"All gun values: {x}, player {id}"));
        }
        
        
        
        [Command]
        public void CmdSetNewGunSlot(GunIDs newGun, WeaponSlot slot)
        {
            Debug.Log("Calling command...");
            allGuns[slot] = newGun;
            RpcAddGunSlot(newGun, slot);
            ServerSwitchGunSlot(slot);
        }

        [ClientRpc]
        public void RpcAddGunSlot(GunIDs newGun, WeaponSlot slot)
        {
            Debug.Log("Calling rpc...");
            if (!GunDatabase.TryGetGunModel(newGun, out GunViewModel model)) return;
            
            GameObject newGunModel = Instantiate(model.gameObject, transform.position, Quaternion.identity);
    
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

        public void OnCurWeaponSlotChanged(WeaponSlot old, WeaponSlot newSlot)
        {
            switch (old)
            {
                case WeaponSlot.Primary:
                    primarySlot.GetChild(0).gameObject.SetActive(false);
                    break;
                case WeaponSlot.Secondary:
                    secondarySlot.GetChild(0).gameObject.SetActive(false);
                    break;
                case WeaponSlot.Melee:
                    meleeSlot.GetChild(0).gameObject.SetActive(false);
                    break;
            }
            
            switch (newSlot)
            {
                case WeaponSlot.Primary:
                    primarySlot.GetChild(0).gameObject.SetActive(true);
                    break;
                case WeaponSlot.Secondary:
                    secondarySlot.GetChild(0).gameObject.SetActive(true);
                    break;
                case WeaponSlot.Melee:
                    meleeSlot.GetChild(0).gameObject.SetActive(true);
                    break;
            }
        }

        [Command]
        private void CmdSwitchGunSlot(WeaponSlot newSlot)
        {
            ServerSwitchGunSlot(newSlot);
        }

        [Server]
        private void ServerCalcAmmo(GunIDs oldGun, GunIDs newGun)
        {
            if (!gunsToAmmo.ContainsKey(oldGun))
            {
                gunsToAmmo.Add(oldGun, new GunAmmo(currentAmmo, reserveAmmo));
            }
            else
            {
                gunsToAmmo[oldGun] = new GunAmmo(currentAmmo, reserveAmmo);
            }

            if (gunsToAmmo.ContainsKey(newGun))
            {
                currentAmmo = gunsToAmmo[newGun].currentAmmo;
                reserveAmmo = gunsToAmmo[newGun].reserveAmmo;
            }
            else if (GunDatabase.TryGetGun(newGun, out Gun gun))
            {
                currentAmmo = gun.MaxAmmo;
                reserveAmmo = gun.ReserveAmmo;
            }
        }
        
        
        [Server]
        private void ServerSwitchGunSlot(WeaponSlot newSlot)
        {
            if (isReloading)
            {
                StopCoroutine(Reload());
                isReloading = false;
            }
            if (GunDatabase.TryGetGun(allGuns[newSlot], out Gun newGun))
            {
                ServerCalcAmmo(allGuns[heldWeaponSlot], newGun.UniqueGunID);
                curGun = newGun;
            }
            heldWeaponSlot = newSlot;
        }
        #endregion
    }
    
    public struct GunAmmo
    {
        public int currentAmmo;
        public int reserveAmmo;

        public GunAmmo(int curAmmo, int resAmmo)
        {
            currentAmmo = curAmmo;
            reserveAmmo = resAmmo;
        }
    }
}
