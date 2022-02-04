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

        Animator anim;

        int currentAmmo;
        int reserve;

        float recoilFactor;
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
            if (!delay)
            {
                recoilFactor = Mathf.Clamp(recoilFactor - Time.fixedDeltaTime * 10f * gun.RecoilDecay, 0f, gun.SwayAfterRound + 1);
                dropFactor = Mathf.Clamp(dropFactor - Time.fixedDeltaTime * 10f * gun.RecoilDecay, 0f, gun.SwayAfterRound + 1);
                spread = Mathf.Clamp(spread - Time.fixedDeltaTime * 10f * gun.RecoilDecay, gun.StartingSpread, gun.MaxSpread);

                if (dropFactor > 0f)
                {
                    PL.MoveCamera(-Time.fixedDeltaTime * 10f * gun.RecoilDecay * gun.Recoil, 0f);
                }
            }
        }

        public IEnumerator Shoot()
        {
            if (!delay)
            {
                delay = true;

                for (int i = 0; i < gun.BulletCount; i++)
                {
                    Raycast();
                }

                Recoil();

                StartCoroutine(AimPunch());

                yield return new WaitForSecondsRealtime(1f / gun.RPS);

                delay = false;
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

                Tracer(_hit);

                Hit(_hit);
            }
            else
            {
                Tracer();
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
        }

        private void Spread()
        {
            spread = Mathf.Clamp(spread + gun.Spread, gun.StartingSpread, gun.MaxSpread);
            moveSpread = Mathf.Clamp(gun.MovementSpread * PM.controller.velocity.magnitude, gun.StartingSpread, gun.MaxMovementSpread);

            float totalSpread = spread + moveSpread;

            spreadPoint.localRotation = Quaternion.Euler(Random.Range(-totalSpread, totalSpread), Random.Range(-totalSpread, totalSpread), 0f);
        }

        private void Tracer(RaycastHit _hit)
        {
            float tracerRate = Random.Range(0f, 1f);

            if (gun.Tracer != null && _hit.distance >= gun.NoTracerRange && tracerRate <= gun.TracerPercentage)
            {
                Vector3[] poses = new Vector3[2];
                poses[0] = _hit.point;
                poses[1] = spreadPoint.position;

                LineRenderer tracer = Instantiate(gun.Tracer, poses[0], cam.transform.rotation).GetComponent<LineRenderer>();
                tracer.SetPositions(poses);
            }
        }

        private void Tracer()
        {
            float tracerRate = Random.Range(0f, 1f);

            if (gun.Tracer != null && tracerRate <= gun.TracerPercentage)
            {
                Vector3[] poses = new Vector3[2];
                poses[0] = spreadPoint.position + spreadPoint.forward * gun.Range;
                poses[1] = spreadPoint.position;

                LineRenderer tracer = Instantiate(gun.Tracer, poses[0], cam.transform.rotation).GetComponent<LineRenderer>();
                tracer.SetPositions(poses);
            }
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
            if (!delay && !isSpraying && currentAmmo < gun.MaxAmmo)
            {
                delay = true;

                if (anim != null)
                {
                    anim.Play(StringKeys.GunReloadAnimation, -1, 0f);
                }

                yield return new WaitForSeconds(gun.ReloadTime);

                currentAmmo = gun.MaxAmmo;

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
