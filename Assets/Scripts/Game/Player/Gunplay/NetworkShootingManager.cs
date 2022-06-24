using AudioUtils;
using Game.Player.Damage;
using Game.Player.Gunplay.IdentifierComponents;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player.Gunplay
{
    public class NetworkShootingManager : NetworkBehaviour
    {
        [Header("Gun storing")]
        public Gun curGun;
        public Melee melee;

        //[SyncVar] public GunIDs curGunId;
        [SyncVar(hook = nameof(OnCurWeaponSlotChanged))] public WeaponSlot heldWeaponSlot = WeaponSlot.Primary;

        private readonly SyncDictionary<WeaponSlot, GunIDs> allGuns = new SyncDictionary<WeaponSlot, GunIDs>();

        [SerializeField]
        private List<Gun> allGunScriptables = new List<Gun>();


        [Header("Ammo info")]
        [SyncVar(hook = nameof(UpdateAmmoUI))]
        public int currentAmmo;
        [SyncVar(hook = nameof(UpdateAmmoUI))]
        public int reserveAmmo;
        [SyncVar]
        public bool isReloading = false;

        private readonly SyncDictionary<GunIDs, GunAmmo> gunsToAmmo = new SyncDictionary<GunIDs, GunAmmo>();

        [Header("Weapon Holder Transforms")]
        public Transform primarySlot;

        public Transform secondarySlot;

        public Transform meleeSlot;

        private int id => GetComponent<NetworkGamePlayer>().playerId;

        [SyncVar]
        private float reloadTimer;


        #region UnityCallbacks
        public void Start()
        {
            if (!hasAuthority)
                enabled = false;

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

            GameUiManager.Instance.UpdateAmmoUI(currentAmmo, reserveAmmo);
            
            
            GameInputManager.PlayerActions.Num1.performed += Slot1;
            GameInputManager.PlayerActions.Num2.performed += Slot2;
            GameInputManager.PlayerActions.Num3.performed += Slot3;
        }

        private void OnDestroy()
        {
            GameInputManager.PlayerActions.Num1.performed -= Slot1;
            GameInputManager.PlayerActions.Num2.performed -= Slot2;
            GameInputManager.PlayerActions.Num3.performed -= Slot3;
        }

        private void Update()
        {
            if (isServer)
            {
                reloadTimer = Mathf.Clamp(reloadTimer - Time.deltaTime, 0f, curGun.ReloadTime);
            }

            if (reloadTimer == 0f && isServer)
            {
                FillMagazine();
            }
        }
        #endregion

        #region ServerShootingLogic
        [Command]
        public void CmdShoot(Vector3 start, Vector3 forward, Vector3 visualFiringPoint)
        {
            if (currentAmmo < 0) return;
            currentAmmo--;
            ServerShoot(start, forward, id, visualFiringPoint);
        }

        [Command]
        public void CmdMelee(Vector3 start, Vector3 forward, float multiplier)
        {
            ServerMelee(start, forward, id, multiplier);
        }

        [Server]
        private void ServerShoot(Vector3 start, Vector3 forward, int id, Vector3 visualFiringPoint)
        {
            RaycastHit[] _hits = Physics.RaycastAll(start, forward, curGun.Range, curGun.HitLayers);

            if (_hits.Length != 0)
            {
                Array.Sort(_hits, (x, y) => x.distance.CompareTo(y.distance));

                foreach (RaycastHit __hit in _hits)
                {
                    DamagePart part = __hit.transform.GetComponentInChildren<DamagePart>();

                    if (part != null)
                    {
                        if (part.Player.playerId == id)
                            continue;

                        Debug.DrawRay(start, forward * curGun.Range, Color.green, 1f);
                        Debug.Log($"Hit something! {__hit.transform.name}, position {__hit.point}, shot by from player {id}");
                        
                        Debug.Log($"Found body part {part.bodyPart} on {__hit.transform.name} when raycasting! ");
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

                        ServerTracer(visualFiringPoint, __hit.point);
                    }
                    else
                    {
                        ServerHit(__hit, visualFiringPoint);
                        return;
                    }
                }
            }
        }

        [Server]
        private void ServerMelee(Vector3 start, Vector3 forward, int id, float multiplier)
        {
            RaycastHit[] _hits = Physics.RaycastAll(start, forward, Melee.Range, Melee.HitLayers);

            if (_hits.Length != 0)
            {
                Array.Sort(_hits, (x, y) => x.distance.CompareTo(y.distance));

                foreach (RaycastHit __hit in _hits)
                {
                    DamagePart part = __hit.transform.GetComponentInChildren<DamagePart>();

                    if (part != null)
                    {
                        if (part.Player.playerId == id)
                            continue;

                        Debug.DrawRay(start, forward * Melee.Range, Color.green, 1f);
                        part.ServerTag(Melee.Tagging);

                        if (Vector3.Dot(transform.forward, part.transform.position - transform.position) > 0f)
                        {
                            part.ServerDamage(Melee.Damage, multiplier * 2f);
                        }
                        else
                        {
                            part.ServerDamage(Melee.Damage, multiplier);
                        }

                        if (melee.HitObject != null)
                        {
                            GameObject hit = Instantiate(melee.HitObject, __hit.point, Quaternion.LookRotation(__hit.normal));
                            NetworkServer.Spawn(hit);
                        }
                    }
                    else
                    {
                        ServerMeleeHit(__hit);
                        return;
                    }
                }
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
        }

        [Server]
        private void ServerMeleeHit(RaycastHit _hit)
        {
            if (melee.HitObject != null)
            {
                GameObject hit = Instantiate(melee.HitObject, _hit.point, Quaternion.LookRotation(_hit.normal));
                NetworkServer.Spawn(hit);
            }

            if (melee.HitDecal != null)
            {
                GameObject decal = Instantiate(melee.HitDecal, _hit.point, Quaternion.LookRotation(-_hit.normal));
                // decal.transform.parent = _hit.transform.GetComponentInChildren<MeshRenderer>().transform;
                NetworkServer.Spawn(decal);
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
            if (currentAmmo == curGun.MaxAmmo || reserveAmmo <= 0 || isReloading) return;
            isReloading = true;
            Reload();
        }

        [Server]
        private void Reload()
        {
            reloadTimer = curGun.ReloadTime;
        }

        [Server]
        private void FillMagazine()
        {
            if (!isReloading)
                return;

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

        [Server]
        private void StopReload()
        {
            reloadTimer = 0f;
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
        }



        [Command]
        public void CmdSetNewGunSlot(GunIDs newGun, WeaponSlot slot)
        {
            Debug.Log("Calling command...");
            StopReload();
            allGuns[slot] = newGun;
            RpcAddGunSlot(newGun, slot);
            ServerSwitchGunSlot(slot);
        }

        [ClientRpc]
        public void RpcAddGunSlot(GunIDs newGun, WeaponSlot slot)
        {
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
            StopReload();
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
            isReloading = false;

            if (GunDatabase.TryGetGun(allGuns[newSlot], out Gun newGun))
            {
                ServerCalcAmmo(allGuns[heldWeaponSlot], newGun.UniqueGunID);
                curGun = newGun;
            }
            heldWeaponSlot = newSlot;
        }
        #endregion

        #region AmmoText

        private void UpdateAmmoUI(int oldValue, int newValue)
        {
            if (!hasAuthority) return;
            GameUiManager.Instance.UpdateAmmoUI(currentAmmo, reserveAmmo);
        }

        #endregion

        public void Slot1(InputAction.CallbackContext callbackContext) => SwitchSlot(1);
        public void Slot2(InputAction.CallbackContext callbackContext) => SwitchSlot(2);
        public void Slot3(InputAction.CallbackContext callbackContext) => SwitchSlot(3);

        public void SwitchSlot(int slot)
        {
            if (MenuOpen.IsOpen)
                return;

            if (hasAuthority && slot == 1)
            {
                Debug.Log("Trying to switch to Primary");
                CmdSwitchGunSlot(WeaponSlot.Primary);
            }
            else if (hasAuthority && slot == 2)
            {
                Debug.Log("Trying to switch to Secondary");
                CmdSwitchGunSlot(WeaponSlot.Secondary);
            }
            else if (hasAuthority && slot == 3)
            {
                Debug.Log("Trying to switch to Melee");
                CmdSwitchGunSlot(WeaponSlot.Melee);
            }
        }
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
