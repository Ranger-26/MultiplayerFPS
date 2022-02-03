using System.Collections;
using UnityEngine;
using Game.Player.Movement;

namespace Game.Player.Gunplay
{
    public class GunViewModel : MonoBehaviour
    {
        public Gun gun;

        PlayerMovement PM;
        PlayerLook PL;

        Transform cam;
        Transform firingPoint;
        Transform spreadPoint;

        int currentAmmo;
        int reserve;

        float recoilFactor;
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
        }

        private void Update()
        {
            isSpraying = Input.GetMouseButton(0);

            if ((isSpraying && gun.GunFiringMode == FiringMode.Auto) || (Input.GetMouseButtonDown(0) && gun.GunFiringMode == FiringMode.SemiAuto))
            {
                StartCoroutine(Shoot());
            }
        }

        private void FixedUpdate()
        {
            if (!delay)
            {
                recoilFactor = Mathf.Clamp(recoilFactor - gun.RecoilDecay * Time.fixedDeltaTime, 0f, gun.SwayAfterRound + 1);
                spread = Mathf.Clamp(spread - gun.RecoilDecay * Time.fixedDeltaTime, gun.StartingSpread, gun.MaxSpread);
            }
        }

        public IEnumerator Shoot()
        {
            if (delay)
                yield return null;

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

        private void Raycast()
        {
            Spread();

            RaycastHit _hit;
            if (Physics.Raycast(spreadPoint.position, spreadPoint.forward, out _hit, gun.Range, gun.HitLayers))
            {
                Debug.Log(_hit.transform.gameObject.name);
            }
        }

        private void Recoil()
        {
            recoilFactor = Mathf.Clamp(recoilFactor++, 0f, gun.SwayAfterRound + 1);

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

        public IEnumerator AimPunch()
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
    }
}
