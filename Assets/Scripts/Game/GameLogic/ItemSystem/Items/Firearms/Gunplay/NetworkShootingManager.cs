using System;
using System.Collections;
using Game.GameLogic.ItemSystem.Items.Knife;
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
        public KnifeComponent Melee;

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
            /*
            if (isServer && curGun != null)
            {
                reloadTimer = Mathf.Clamp(reloadTimer - Time.deltaTime, 0f, curGun.ReloadTime);
            }

            if (reloadTimer == 0f && isServer && curGun != null)
            {
                FillMagazine();
            }
            */
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

        [Server]
        private void ServerShoot(Vector3 start, Vector3 forward, int id, Vector3 visualFiringPoint)
        {
            RaycastHit[] _hits = Physics.RaycastAll(start, forward, curGun.Range, curGun.HitLayers, QueryTriggerInteraction.Ignore);

            if (_hits.Length != 0)
            {
                Array.Sort(_hits, (x, y) => x.distance.CompareTo(y.distance));

                int curDmg = curGun.Damage;
                float curPene = curGun.Penetration;

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
                        part.ServerDamage(curDmg, multiplier);

                        ServerTracer(visualFiringPoint, __hit.point);
                    }
                    else
                    {
                        Vector3 temp = __hit.point + forward * curPene;

                        ServerHit(__hit, visualFiringPoint);

                        RaycastHit hit;
                        if (Physics.Raycast(temp, -forward, out hit, curGun.Penetration, curGun.HitLayers))
                        {
                            float modifier = 1f;

                            if (hit.transform.TryGetComponent(out MaterialModifier material))
                                modifier = material.mat.WallbangMultiplier;

                            float tempDist = Vector3.Distance(hit.point, __hit.point);
                            curPene = Mathf.Clamp(curPene * modifier - tempDist, 0f, curPene);
                            curDmg = curDmg - (int)((1f - curPene / curGun.Penetration) * curDmg / modifier);
                            ServerHit(hit);
                        }
                        else
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
        private void ServerHit(RaycastHit _hit)
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
        public void CmdReloadGun()
        {
            if (currentAmmo == curGun.MaxAmmo || reserveAmmo <= 0 || isReloading) return;
            isReloading = true;
            StartCoroutine(HandleReload());
        }

        public IEnumerator HandleReload()
        {
            yield return new WaitForSeconds(curGun.ReloadTime);
            FillMagazine();
        }
        
        
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
