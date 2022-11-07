using AudioUtils;
using Game.GameLogic.ItemSystem.Items.Firearms.Gunplay.IdentifierComponents;
using Game.Player.Movement;
using Game.UI;
using Inputs;
using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.GameLogic.ItemSystem.Items.Firearms.Gunplay
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
        ParticleSystem muzzleFlash;

        PlayerMovement PM;
        PlayerLook PL;
        PlayerCrouch PC;

        Camera cam;

        Transform firingPoint;
        Transform spreadPoint;

        GameObject scopeUI;
        GameObject model;

        Image scopeUIImage;

        NetworkShootingManager nsm;

        [HideInInspector]
        public Animator anim;

        NetworkIdentity ni;

        float recoilFactor;
        float displacementFactor;
        float lerpFactor;

        float spread;
        float moveSpread;
        float horizontalRecoil;

        float shootTimer;
        float reloadTimer;
        float chamberTimer;
        float chargeupTimer;

        bool delay;
        bool isSpraying;
        bool isSwaying;
        bool isScoped;
        bool horizontalDirection;
        bool chargedUp;
        bool chargeupSound;
        bool canCharge;
        bool chambered;
        bool finishedReload;

        public GunIDs gunId;

        private void Awake()
        {
            PM = GetComponentInParent<PlayerMovement>();
            PL = GetComponentInParent<PlayerLook>();
            PC = GetComponentInParent<PlayerCrouch>();
            scopeUI = GameObject.Find("Canvas").transform.Find("Scope").gameObject;
            scopeUIImage = scopeUI.GetComponent<Image>();
            cam = PL.cam;
            model = transform.GetChild(0).gameObject;
        }

        private void Start()
        {
            ni = GetComponentInParent<NetworkIdentity>();

            if (!ni.isLocalPlayer)
            {
                Transform tempcam = GetComponentInParent<Camera>().transform;
                Destroy(tempcam.GetComponentInChildren<FiringPoint>().gameObject);
                tempcam.GetComponent<HDAdditionalCameraData>().enabled = false;
                tempcam.GetComponent<Camera>().enabled = false;
                tempcam.GetComponent<AudioListener>().enabled = false;
                enabled = false;
            }

            nsm = GetComponentInParent<NetworkShootingManager>();
            firingPoint = cam.GetComponentInChildren<FiringPoint>().transform;
            spreadPoint = cam.GetComponentInChildren<SpreadPoint>().transform;
            anim = GetComponentInChildren<Animator>();

            anim.keepAnimatorControllerStateOnDisable = true;

            if (gun.ChargeupTime <= 0f)
                chargedUp = true;

            canCharge = true;
            chambered = true;
            finishedReload = true;

            if (PM == null) { Debug.LogError("Player movement is null!"); }
            if (PL == null) { Debug.LogError("Player look is null!"); }

            if (PM != null) PM.weight = gun.Weight;

            Scope(false);

            StartCoroutine(Draw());

            scopeUIImage.sprite = gun.ScopeImage;

            if (nsm == null) { Debug.LogError("Network Shooting Manager is null in the start!"); }

            GameInputManager.PlayerActions.Fire.performed += UpdateSpray;
            GameInputManager.PlayerActions.Fire.performed += SemiAuto;
            GameInputManager.PlayerActions.Fire.canceled += UpdateSpray;

            GameInputManager.PlayerActions.Reload.performed += Reload;

            GameInputManager.PlayerActions.Inspect.performed += Inspect;

            GameInputManager.PlayerActions.AltFire.performed += UpdateScope;
        }


        private void OnEnable()
        {
            if (PM != null)
                PM.weight = gun.Weight;

            Scope(false);

            StartCoroutine(Draw());

            scopeUIImage.sprite = gun.ScopeImage;
        }

        private void Update()
        {
            if (nsm == null)
            {
                nsm = GetComponentInParent<NetworkShootingManager>();
                return;
            }

            if (!nsm.hasAuthority)
                enabled = false;

            if (isSpraying && gun.GunFiringMode == FiringMode.Auto)
            {
                if (MenuOpen.IsOpen)
                    return;

                Shoot();
            }

            if (gun.ChargeupTime > 0f && isSpraying && nsm.currentAmmo > 0 && canCharge)
            {
                chargeupTimer = Mathf.Clamp(chargeupTimer + Time.deltaTime, 0f, gun.ChargeupTime);

                if (!chargeupSound)
                {
                    chargeupSound = true;

                    if (gun.ChargeupSounds.Length != 0)
                        AudioSystem.NetworkPlaySound(Sound: gun.ChargeupSounds[Random.Range(0, gun.ChargeupSounds.Length - 1)], Parent: PM.transform.GetComponent<NetworkTransform>(), Position: cam.transform.position + cam.transform.forward, MaxDistance: gun.SoundMaxDistance, Volume: gun.SoundVolume, Priority: gun.SoundPriority);
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

            float x = PM.movementInput.x * Convert.ToInt32(!MenuOpen.IsOpen);
            float z = PM.movementInput.y * Convert.ToInt32(!MenuOpen.IsOpen);

            lerpFactor = Mathf.Clamp01(
                lerpFactor + Mathf.Abs(x * Time.deltaTime * gun.BackingMultiplier) + Mathf.Abs(z * Time.deltaTime * gun.BackingMultiplier)
                );

            if ((x == 0) && (z == 0))
                lerpFactor = Mathf.Clamp01(lerpFactor - Time.deltaTime * gun.BackingMultiplier);

            Vector3 temp = Vector3.Slerp(Vector3.zero, new Vector3(0f, -gun.MaxBacking / 2f, -gun.MaxBacking), lerpFactor);

            transform.localPosition = Vector3.Lerp(transform.localPosition, temp, 14f * Time.deltaTime);

            shootTimer = Mathf.Clamp(shootTimer - Time.deltaTime, 0f, 60f / gun.RPM);
            reloadTimer = Mathf.Clamp(reloadTimer - Time.deltaTime, 0f, gun.ReloadTime);
            chamberTimer = Mathf.Clamp(chamberTimer - Time.deltaTime, 0f, gun.DrawTime);

            if (reloadTimer == 0f)
            {
                Chamber();

                if (chamberTimer == 0f)
                    FinishReload();
            }

            Crosshair.Instance.UpdateError(spread);
        }

        private void FixedUpdate()
        {
            if (PM == null) return;

            isSwaying = recoilFactor > gun.SwayAfterRecoil;

            moveSpread = Mathf.Clamp(gun.MovementSpread * (PM.velocityGun.magnitude - gun.MovementSpreadTolerance) + gun.MaxMovementSpread * Convert.ToInt32(!PM.isGrounded), 0f, gun.MaxMovementSpread);

            if (shootTimer <= 0f && (!isSpraying && gun.GunFiringMode == FiringMode.Auto) || nsm.currentAmmo == 0 || shootTimer <= 60f / gun.RPM - 0.1f && gun.GunFiringMode == FiringMode.SemiAuto)
            {
                UpdateSpread();

                if (!isSwaying)
                {
                    horizontalRecoil = 0f;
                    horizontalDirection = gun.SwayStartRight;
                }

                if (PL.GetCameraVisualRotation() != Quaternion.Euler(Vector3.zero))
                {
                    PL.SetCameraVisual(Quaternion.RotateTowards(PL.GetCameraVisualRotation(), Quaternion.Euler(Vector3.zero), gun.RecoilDecay * Time.fixedDeltaTime));
                }

                recoilFactor = Mathf.Clamp(recoilFactor - Time.fixedDeltaTime * gun.RecoilDecay, 0f, gun.SwayAfterRecoil + 1);
                displacementFactor = Mathf.Clamp(displacementFactor - Time.fixedDeltaTime * gun.RecoilDecay, 0f, gun.SwayAfterRecoil + 1);
                spread = Mathf.Clamp(spread - Time.fixedDeltaTime * gun.SpreadDecay, gun.StartingSpread, gun.MaxSpread);
            }

            firingPoint.localRotation = Quaternion.Euler(-displacementFactor, 0f, 0f);
        }

        public void UpdateSpray(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
                isSpraying = true;
            else if (callbackContext.canceled)
                isSpraying = false;
        }

        public void SemiAuto(InputAction.CallbackContext callbackContext)
        {
            if (MenuOpen.IsOpen)
                return;

            if (!delay && gun.GunFiringMode == FiringMode.SemiAuto)
            {
                Shoot();
            }
        }

        public void Inspect(InputAction.CallbackContext callbackContext)
        {
            if (!delay && !MenuOpen.IsOpen)
            {
                if (anim != null)
                    anim.SetTrigger(StringKeys.GunInspectAnimation);
            }
        }

        public void Reload(InputAction.CallbackContext callbackContext)
        {
            if (MenuOpen.IsOpen)
                return;

            if (!delay && !isSpraying && nsm.currentAmmo < gun.MaxAmmo && nsm.reserveAmmo > 0 && nsm.hasAuthority)
                Reload();
        }

        public void UpdateScope(InputAction.CallbackContext callbackContext)
        {
            if (gun.HasScope && !delay)
                Scope(!isScoped);
        }

        public void Scope(bool status)
        {
            if (!gun.HasScope)
            {
                isScoped = false;
                cam.fieldOfView = 87.5f;
                scopeUI.SetActive(false);
                model.SetActive(true);
                return;
            }

            isScoped = status;
            cam.fieldOfView = status ? gun.ScopeFOV : 87.5f;
            scopeUI.SetActive(status);
            model.SetActive(!status);

            if (PM != null) PM.weight = status ? gun.ScopeSpeedReduction : gun.Weight;
        }

        public void Shoot()
        {
            if (!ni.hasAuthority) return;

            if (nsm.currentAmmo <= 0 && !delay && nsm.reserveAmmo > 0 && !isSpraying) return;

            if (!delay && nsm.currentAmmo > 0 && shootTimer == 0f && (chargedUp && gun.ChargeupTime > 0f || gun.ChargeupTime <= 0f) && !MenuOpen.IsOpen)
            {
                shootTimer = 60f / gun.RPM;

                nsm.CmdAmmo();

                for (int i = 0; i < nsm.curGun.BulletCount; i++)
                {
                    if (nsm.hasAuthority)
                    {
                        Debug.DrawRay(spreadPoint.position, spreadPoint.forward * gun.Range, Color.green, 5f);

                        // nsm.CmdSendDebug($"Spread point pos: {spreadPoint.position}", GetComponentInParent<NetworkGamePlayer>().playerId);

                        if (muzzleFlash != null)
                            nsm.CmdShoot(spreadPoint.position, spreadPoint.forward, muzzleFlash.transform.position);
                        else
                            nsm.CmdShoot(spreadPoint.position, spreadPoint.forward, transform.position);
                    }

                    if (gun.ExitScopeOnShoot)
                    {
                        Scope(false);
                    }

                    Spread();

                    Visual();
                }

                Recoil();
            }
        }

        public void Visual()
        {
            if (anim != null)
                anim.ResetTrigger(StringKeys.GunShootAnimation);

            if (!isScoped)
            {
                if (muzzleFlash != null)
                    muzzleFlash.Play();
            }

            if (anim != null)
                anim.SetTrigger(StringKeys.GunShootAnimation);

            if (gun.ShootSounds.Length != 0)
                AudioSystem.NetworkPlaySound(Sound: gun.ShootSounds[Random.Range(0, gun.ShootSounds.Length - 1)], Position: cam.transform.position + cam.transform.forward, Parent: PM.transform.GetComponent<NetworkTransform>(), MaxDistance: gun.SoundMaxDistance, Volume: gun.SoundVolume, Priority: gun.SoundPriority);
        }

        private void Recoil()
        {
            recoilFactor = Mathf.Clamp(recoilFactor + gun.Recoil, 0f, gun.SwayAfterRecoil + gun.Recoil);

            if (recoilFactor < gun.SwayAfterRecoil)
                PL.MoveCameraVisual(gun.Recoil);
            else
            {
                PL.MoveCameraVisual(0f, (horizontalDirection ? 1 : -1) * gun.HorizontalRecoil);

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
            if (recoilFactor >= gun.SwayAfterRecoil / 4)
                displacementFactor = Mathf.Clamp(displacementFactor + gun.Displacement, 0f, gun.MaxDisplacement);
        }

        private void Spread()
        {
            spread = Mathf.Clamp(spread + (isScoped ? gun.ScopedSpread : gun.Spread), isScoped ? gun.ScopedSpread : gun.StartingSpread, gun.MaxSpread);

            UpdateSpread();
        }

        private void UpdateSpread()
        {
            float totalSpread = spread * (PC.isCrouching ? 0.5f : 1f) + moveSpread;

            spreadPoint.localRotation = Quaternion.Euler(Random.Range(-totalSpread, totalSpread), Random.Range(-totalSpread, totalSpread), 0f);
        }

        private void Reload()
        {
            if (nsm.isReloading) return;
            Debug.Log("Trying to reload...");

            delay = true;
            canCharge = false;
            chambered = false;
            finishedReload = false;

            if (anim != null)
                anim.ResetTrigger(StringKeys.GunReloadAnimation);

            Scope(false);

            reloadTimer = nsm.curGun.ReloadTime;

            Debug.Log("Reloading... ");

            if (anim != null)
                anim.SetTrigger(StringKeys.GunReloadAnimation);

            nsm.CmdReloadGun();
        }

        private void Chamber()
        {
            if (!chambered)
            {
                chamberTimer = gun.DrawTime;
                chambered = true;
            }
        }

        private void FinishReload()
        {
            if (!finishedReload)
            {
                chargeupTimer = 0f;

                if (gun.ChargeupTime > 0f)
                    chargedUp = false;

                canCharge = true;
                finishedReload = true;
                delay = false;
            }
        }

        private IEnumerator Draw()
        {
            delay = true;
            canCharge = false;
            chambered = true;

            if (anim != null)
                anim.ResetTrigger(StringKeys.GunDrawAnimation);

            spread = gun.StartingSpread;

            displacementFactor = 0f;
            recoilFactor = 0f;

            if (anim != null)
                anim.SetTrigger(StringKeys.GunDrawAnimation);

            yield return new WaitForSeconds(gun.DrawTime);

            canCharge = true;
            delay = false;
        }

        private void OnDestroy()
        {
            GameInputManager.PlayerActions.Fire.performed -= UpdateSpray;
            GameInputManager.PlayerActions.Fire.performed -= SemiAuto;
            GameInputManager.PlayerActions.Fire.canceled -= UpdateSpray;

            GameInputManager.PlayerActions.Reload.performed -= Reload;

            GameInputManager.PlayerActions.Inspect.performed -= Inspect;

            GameInputManager.PlayerActions.AltFire.performed -= UpdateScope;
        }
    }
}
