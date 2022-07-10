using System;
using Game.Player;
using Game.Player.Damage;
using Mirror;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Firearms.Gunplay
{
    public class NetworkShootingManager : NetworkBehaviour
    {
        [Header("Gun storing")]
        public Gun curGun;
        public Melee melee;

        //[SyncVar] public GunIDs curGunId;


        [Header("Ammo info")]
        [SyncVar(hook = nameof(UpdateAmmoUI))]
        public int currentAmmo;
        [SyncVar(hook = nameof(UpdateAmmoUI))]
        public int reserveAmmo;
        [SyncVar]
        public bool isReloading = false;

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
            
            GameUiManager.Instance.UpdateAmmoUI(currentAmmo, reserveAmmo);
        }
        

        private void Update()
        {
            if (isServer && curGun != null)
            {
                reloadTimer = Mathf.Clamp(reloadTimer - Time.deltaTime, 0f, curGun.ReloadTime);
            }

            if (reloadTimer == 0f && isServer && curGun != null)
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
            Debug.Log($"Server melee: {_hits.Length} hits.");
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

        #region AmmoText

        private void UpdateAmmoUI(int oldValue, int newValue)
        {
            if (!hasAuthority) return;
            GameUiManager.Instance.UpdateAmmoUI(currentAmmo, reserveAmmo);
        }

        #endregion
    }
}
