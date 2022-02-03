using System.Collections;
using UnityEngine;
using Game.Player.Movement;

namespace Game.Player.Gunplay
{
    public class GunViewModel : MonoBehaviour
    {
        public Gun gun;

        Transform cam;

        int currentAmmo;
        int reserve;

        float recoilFactor;
        float spread;
        float moveSpread;

        bool delay;
        bool isSpraying;
        bool isSwaying;
        bool horizontalDirection;

        private void Awake()
        {
            cam = Camera.main.transform;
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

            yield return new WaitForSecondsRealtime(1f / gun.RPS);

            delay = false;
        }

        private void Raycast()
        {
            Spread();

            RaycastHit _hit;
            if (Physics.Raycast(cam.position, cam.forward, out _hit, gun.Range))
            {
                Debug.Log(_hit.transform.gameObject.name);
            }
        }

        private void Recoil()
        {
            recoilFactor = Mathf.Clamp(recoilFactor++, 0f, gun.SwayAfterRound + 1);

            if (recoilFactor < gun.SwayAfterRound)
            {
                PlayerLook.Instance.MoveCamera(gun.Recoil);
            }
            else
            {
                PlayerLook.Instance.MoveCamera(0f, (horizontalDirection ? 1 : -1) * gun.HorizonalRecoil);
            }
        }

        private void Spread()
        {
            spread = Mathf.Clamp(spread + gun.Spread, gun.StartingSpread, gun.MaxSpread);
            moveSpread = Mathf.Clamp(moveSpread + gun.MovementSpread, gun.StartingSpread, gun.MaxMovementSpread);
        }
    }
}
