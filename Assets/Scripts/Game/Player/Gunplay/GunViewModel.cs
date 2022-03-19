using Game.Player.Movement;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Mirror;
using Unity.VisualScripting;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Player.Gunplay
{
    public class GunViewModel : MonoBehaviour
    {
        [Header("Settings")]
        public Gun gun;

        [HideInInspector]
        public float Sprd
        {
            get
            {
                return spread;
            }
        }

        [Header("Visuals")]
        [SerializeField]
        VisualEffect muzzleFlash;

        PlayerMovement PM;
        PlayerLook PL;

        Transform cam;
        Transform firingPoint;
        Transform spreadPoint;

        NetworkShootingManager nsm;

        Vector3 _prevPosition;
        Vector3 vel;

        Animator anim;

        NetworkIdentity ni;

        float recoilFactor;
        float displacementFactor;
        float dropFactor;
        float spread;
        float moveSpread;
        float horizontalRecoil;
        float shootTimer;
        float chargeupTimer;

        bool delay;
        bool isSpraying;
        bool isSwaying;
        bool horizontalDirection;
        bool chargedUp;
        bool chargeupSound;
        bool canCharge;
        bool shootQueue;

        private void Start()
        {
            if (!GetComponentInParent<NetworkIdentity>().isLocalPlayer)
            {
                Transform tempcam = GetComponentInParent<Camera>().transform;
                Destroy(tempcam.GetComponentInChildren<FiringPoint>().gameObject);
                tempcam.GetComponent<HDAdditionalCameraData>().enabled = false;
                tempcam.GetComponent<Camera>().enabled = false;
                tempcam.GetComponent<AudioListener>().enabled = false;
                enabled = false;
            } 

            PM = GetComponentInParent<PlayerMovement>();
            PL = GetComponentInParent<PlayerLook>();
            nsm = GetComponentInParent<NetworkShootingManager>();
            cam = GetComponentInParent<Camera>().transform;
            firingPoint = cam.GetComponentInChildren<FiringPoint>().transform;
            spreadPoint = cam.GetComponentInChildren<SpreadPoint>().transform;

            anim = GetComponent<Animator>();

            ni = GetComponentInParent<NetworkIdentity>();

            if (gun.ChargeupTime <= 0f)
            {
                chargedUp = true;
            }

            canCharge = true;

            if (PM == null) { Debug.LogError("Player movement is null!"); }
            if (PL == null) { Debug.LogError("Player look is null!"); }
            PM.weight = gun.Weight;
            StartCoroutine(Draw());

            if (nsm == null)
            {
                Debug.LogError("Network Shooting Manager is null in the start!");
            }
            //nsm.CmdSendDebug($"Spread point pos: {spreadPoint.position}", GetComponentInParent<NetworkGamePlayer>().playerId);
        }

        private void Update()
        {
            if (nsm == null)
            {
                Debug.LogError("Network Shooting Manager is null in the update!");
            }
            if (!nsm.hasAuthority)
                return;

            isSpraying = Input.GetMouseButton(0);

            if ((isSpraying && gun.GunFiringMode == FiringMode.Auto) || (Input.GetMouseButtonDown(0) && gun.GunFiringMode == FiringMode.SemiAuto))
            {
                if (MenuOpen.IsOpen)
                    return;

                Shoot();

                if (delay && gun.GunFiringMode == FiringMode.SemiAuto && shootTimer <= 0.1f && shootTimer > 0f)
                {
                    shootQueue = true;
                }
            }

            if (shootQueue)
            {
                if (MenuOpen.IsOpen)
                    return;

                Shoot();
                shootQueue = false;
            }

            if (gun.ChargeupTime > 0f && isSpraying && nsm.currentAmmo > 0 && canCharge)
            {
                chargeupTimer = Mathf.Clamp(chargeupTimer + Time.deltaTime, 0f, gun.ChargeupTime);

                if (!chargeupSound)
                {
                    chargeupSound = true;

                    if (gun.ChargeupSounds.Length != 0)
                    {
                        AudioSystem.PlaySound(gun.ChargeupSounds[Random.Range(0, gun.ChargeupSounds.Length - 1)], spreadPoint.position + spreadPoint.forward * 0.5f, gun.SoundMaxDistance, gun.SoundVolume, 1f, 1f, gun.SoundPriority);
                    }
                }
            }
            else if (gun.ChargeupTime > 0f && !isSpraying && nsm.currentAmmo > 0)
            {
                chargeupTimer = Mathf.Clamp(chargeupTimer - Time.deltaTime, 0f, gun.ChargeupTime);

                chargeupSound = false;
            }

            if (gun.ChargeupTime > 0f)
            {
                chargedUp = chargeupTimer >= gun.ChargeupTime;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (MenuOpen.IsOpen)
                    return;

                if (anim != null)
                {
                    anim.Play(StringKeys.GunInspectAnimation, -1, 0f);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (MenuOpen.IsOpen)
                    return;

                if (!delay && !isSpraying && nsm.currentAmmo < gun.MaxAmmo && nsm.reserveAmmo > 0 && nsm.hasAuthority)
                {
                    StartCoroutine(Reload());
                }
            }

            Vector3 temp = Vector3.Slerp(Vector3.zero, new Vector3(0f, -gun.MaxBacking / 4f, -gun.MaxBacking), Mathf.InverseLerp(0f, gun.MaxSpread + gun.MaxMovementSpread, spread + moveSpread));

            transform.localPosition = Vector3.Lerp(transform.localPosition, temp, 4f * Time.deltaTime);

            shootTimer -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            isSwaying = recoilFactor > gun.SwayAfterRound;

            moveSpread = Mathf.Clamp(gun.MovementSpread * vel.magnitude, 0f, gun.MaxMovementSpread);

            vel = (PM.transform.position - _prevPosition) / Time.fixedDeltaTime;
            _prevPosition = PM.transform.position;

            if ((!delay && (!isSpraying && gun.GunFiringMode == FiringMode.Auto) || (gun.GunFiringMode == FiringMode.SemiAuto)) || (nsm.currentAmmo == 0))
            {
                recoilFactor = Mathf.Clamp(recoilFactor - Time.fixedDeltaTime * 10f * gun.RecoilDecay, 0f, gun.SwayAfterRound + 1);
                displacementFactor = Mathf.Clamp(displacementFactor - Time.fixedDeltaTime * 10f * gun.RecoilDecay, 0f, gun.SwayAfterRound + 1);
                dropFactor = Mathf.Clamp(dropFactor - Time.fixedDeltaTime * 10f * gun.RecoilDecay, 0f, gun.SwayAfterRound + 1);
                spread = Mathf.Clamp(spread - Time.fixedDeltaTime * 10f * gun.RecoilDecay, gun.StartingSpread, gun.MaxSpread);

                UpdateSpread();

                if (!isSwaying)
                {
                    horizontalRecoil = 0f;
                    horizontalDirection = gun.SwayStartRight;
                }

                if (dropFactor > 0f)
                {
                    PL.MoveCamera(-Time.fixedDeltaTime * 10f * gun.RecoilDecay * gun.Recoil, 0f);
                }
            }

            firingPoint.localRotation = Quaternion.Euler(-displacementFactor, 0f, 0f);
        }

        public void Shoot()
        {
            if (!ni.hasAuthority) return;
            if (!delay && nsm.currentAmmo > 0 && shootTimer <= 0f && (chargedUp && gun.ChargeupTime > 0f || gun.ChargeupTime <= 0f))
            {
                shootTimer = 60f / gun.RPM;

                for (int i = 0; i < nsm.curGun.BulletCount; i++)
                {
                    if (nsm.hasAuthority)
                    {
                        Debug.DrawRay(spreadPoint.position, spreadPoint.forward * gun.Range, Color.green, 1f);
                        // nsm.CmdSendDebug($"Spread point pos: {spreadPoint.position}", GetComponentInParent<NetworkGamePlayer>().playerId);
                        nsm.CmdShoot(spreadPoint.position, spreadPoint.forward);
                    }

                    Spread();

                    Visual();
                }

                nsm.CmdAmmo();

                Recoil();

                StartCoroutine(AimPunch());
            }

            if (nsm.currentAmmo <= 0 && !delay && nsm.reserveAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }

        public void Visual()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            if (anim != null)
            {
                anim.Play(StringKeys.GunShootAnimation, -1, 0f);
            }

            if (gun.ShootSounds.Length != 0)
            {
                AudioSystem.PlaySound(gun.ShootSounds[Random.Range(0, gun.ShootSounds.Length - 1)], cam, gun.SoundMaxDistance, gun.SoundVolume, 1f, 1.1f, gun.SoundPriority);
            }
        }

        private void Recoil()
        {
            recoilFactor = Mathf.Clamp(recoilFactor + 1, 0f, gun.SwayAfterRound + 1);
            dropFactor += gun.Recoil;

            if (recoilFactor < gun.SwayAfterRound)
            {
                PL.MoveCamera(gun.Recoil);
            }
            else
            {
                PL.MoveCamera(0f, (horizontalDirection ? 1 : -1) * gun.HorizontalRecoil);

                horizontalRecoil += gun.HorizontalRecoil;

                if (horizontalRecoil > gun.MaxHorizontal || horizontalRecoil < -gun.MaxHorizontal)
                {
                    horizontalDirection = !horizontalDirection;
                    horizontalRecoil = -gun.MaxHorizontal;
                }
            }

            Displacement();
        }

        private void Displacement()
        {
            if (recoilFactor >= gun.SwayAfterRound / 4)
            {
                displacementFactor = Mathf.Clamp(displacementFactor + gun.Displacement, 0f, gun.MaxDisplacement);
            }
        }

        private void Spread()
        {
            spread = Mathf.Clamp(spread + gun.Spread, gun.StartingSpread, gun.MaxSpread);

            UpdateSpread();
        }

        private void UpdateSpread()
        {
            float totalSpread = spread + moveSpread;

            spreadPoint.localRotation = Quaternion.Euler(Random.Range(-totalSpread, totalSpread), Random.Range(-totalSpread, totalSpread), 0f);
        }
        
        private IEnumerator AimPunch()
        {
            float timer = 0f;

            while (timer <= gun.AimPunchDuration / 3)
            {
                PL.MoveCamera(gun.AimPunch / (gun.AimPunchDuration / 3) * Time.fixedDeltaTime);

                timer += Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }

            timer = 0f;

            while (timer <= gun.AimPunchDuration * 2 / 3)
            {
                PL.MoveCamera(-gun.AimPunch / (gun.AimPunchDuration * 2 / 3) * Time.fixedDeltaTime);

                timer += Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }
        }
        

        private IEnumerator Reload()
        {
            Debug.Log("Trying to reload...");

            delay = true;
            canCharge = false;

            Debug.Log("Reloading... ");

            if (anim != null)
            {
                anim.Play(StringKeys.GunReloadAnimation, -1, 0f);
            }

            nsm.CmdReload();

            //yield return new WaitUntil(()=>!nsm.isReloading);
            yield return new WaitForSeconds(nsm.curGun.ReloadTime);
                
            chargeupTimer = 0f;
                
            if (gun.ChargeupTime > 0f)
            {
                chargedUp = false;
            }

            canCharge = true;

            Debug.Log("Filled Magazine, Chambering... ");

            yield return new WaitForSeconds(gun.DrawTime);

            Debug.Log("Gun Chambered. ");

            delay = false;
        }

        private IEnumerator Draw()
        {
            delay = true;
            canCharge = false;

            if (anim != null)
            {
                anim.Play(StringKeys.GunDrawAnimation, -1, 0f);
            }

            yield return new WaitForSeconds(gun.DrawTime);

            if (anim != null)
            {
                anim.Play(StringKeys.GunIdleAnimation, -1, 0f);
            }

            canCharge = true;
            delay = false;
        }
    }
}
