using Game.Player.Movement;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Player.Gunplay
{
    public class GunViewModel : MonoBehaviour
    {
        [Header("Settings")]
        public Gun gun;

        [Header("Visuals")]
        [SerializeField]
        VisualEffect muzzleFlash;

        PlayerMovement PM;
        PlayerLook PL;

        Transform cam;
        Transform firingPoint;
        Transform spreadPoint;

        Vector3 _prevPosition;
        Vector3 vel;

        Animator anim;

        int currentAmmo;
        int reserve;

        float recoilFactor;
        float displacementFactor;
        float dropFactor;
        float spread;
        float moveSpread;
        float horizontalRecoil;

        bool delay;
        bool isSpraying;
        bool isSwaying;
        bool horizontalDirection;

        private void Awake()
        {
            PM = PlayerMovement.Instance;
            PL = PlayerLook.Instance;
            cam = Camera.main.transform;
            firingPoint = cam.GetChild(0);
            spreadPoint = firingPoint.GetChild(0);

            anim = GetComponent<Animator>();

            currentAmmo = gun.MaxAmmo;
            reserve = gun.ReserveAmmo;
        }

        private void OnEnable()
        {
            StartCoroutine(Draw());
        }

        private void Update()
        {
            isSpraying = Input.GetMouseButton(0);

            if ((isSpraying && gun.GunFiringMode == FiringMode.Auto) || (Input.GetMouseButtonDown(0) && gun.GunFiringMode == FiringMode.SemiAuto))
            {
                StartCoroutine(Shoot());
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (anim != null)
                {
                    anim.Play(StringKeys.GunInspectAnimation, -1, 0f);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
            }
        }

        private void FixedUpdate()
        {
            isSwaying = recoilFactor > gun.SwayAfterRound;

            vel = (PM.transform.position - _prevPosition) / Time.fixedDeltaTime;
            _prevPosition = PM.transform.position;

            if ((!delay && !isSpraying) || (currentAmmo == 0))
            {
                recoilFactor = Mathf.Clamp(recoilFactor - Time.fixedDeltaTime * 10f * gun.RecoilDecay, 0f, gun.SwayAfterRound + 1);
                displacementFactor = Mathf.Clamp(displacementFactor - Time.fixedDeltaTime * 10f * gun.RecoilDecay, 0f, gun.SwayAfterRound + 1);
                dropFactor = Mathf.Clamp(dropFactor - Time.fixedDeltaTime * 10f * gun.RecoilDecay, 0f, gun.SwayAfterRound + 1);
                spread = Mathf.Clamp(spread - Time.fixedDeltaTime * 10f * gun.RecoilDecay, gun.StartingSpread, gun.MaxSpread);

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

        public IEnumerator Shoot()
        {
            if (!delay && currentAmmo > 0)
            {
                delay = true;

                currentAmmo--;

                for (int i = 0; i < gun.BulletCount; i++)
                {
                    Raycast();
                }

                Recoil();

                StartCoroutine(AimPunch());

                yield return new WaitForSecondsRealtime(1f / gun.RPS);

                delay = false;
            }

            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
        }

        private void Raycast()
        {
            Spread();

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
                AudioSystem.PlaySound(gun.ShootSounds[Random.Range(0, gun.ShootSounds.Length - 1)], spreadPoint.position, gun.SoundMaxDistance, gun.SoundVolume, 1f, 1f, gun.SoundPriority);
            }

            RaycastHit _hit;
            if (Physics.Raycast(spreadPoint.position, spreadPoint.forward, out _hit, gun.Range, gun.HitLayers))
            {
                // Hit

                if (_hit.transform.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody target = _hit.transform.GetComponent<Rigidbody>();

                    target.AddForceAtPosition(cam.transform.forward * gun.Damage * 0.5f, _hit.point, ForceMode.Impulse);
                }

                Hit(_hit);
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
            if (recoilFactor >= gun.SwayAfterRound / 2)
            {
                displacementFactor = Mathf.Clamp(displacementFactor + gun.Displacement, 0f, gun.MaxDisplacement);
            }
        }

        private void Spread()
        {
            spread = Mathf.Clamp(spread + gun.Spread, gun.StartingSpread, gun.MaxSpread);
            moveSpread = Mathf.Clamp(gun.MovementSpread * vel.magnitude, gun.StartingSpread, gun.MaxMovementSpread);

            float totalSpread = spread + moveSpread;

            spreadPoint.localRotation = Quaternion.Euler(Random.Range(-totalSpread, totalSpread), Random.Range(-totalSpread, totalSpread), 0f);
        }

        private void Hit(RaycastHit _hit)
        {
            if (gun.HitObject != null)
            {
                Instantiate(gun.HitObject, _hit.point, Quaternion.LookRotation(_hit.normal));
            }

            if (gun.HitDecal != null)
            {
                GameObject decal = Instantiate(gun.HitDecal, _hit.point, Quaternion.LookRotation(-_hit.normal));
                decal.transform.parent = _hit.transform.GetComponentInChildren<MeshRenderer>().transform;
            }
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
            if (!delay && !isSpraying && currentAmmo < gun.MaxAmmo && reserve > 0)
            {
                delay = true;

                if (anim != null)
                {
                    anim.Play(StringKeys.GunReloadAnimation, -1, 0f);
                }

                yield return new WaitForSeconds(gun.ReloadTime);

                int exchange = gun.MaxAmmo - currentAmmo;

                if (reserve <= exchange)
                {
                    currentAmmo += reserve;
                    reserve = 0;
                }
                else
                {
                    currentAmmo += exchange;
                    reserve -= exchange;
                }

                delay = false;
            }
        }

        private IEnumerator Draw()
        {
            delay = true;

            if (anim != null)
            {
                anim.Play(StringKeys.GunDrawAnimation, -1, 0f);
            }

            yield return new WaitForSeconds(gun.DrawTime);

            if (anim != null)
            {
                anim.Play(StringKeys.GunIdleAnimation, -1, 0f);
            }

            delay = false;
        }
    }
}
