using Game.Player.Damage;
using Game.Player.Movement;
using Mirror;
using UnityEngine;

namespace Game.Player.Gunplay
{
    public class NetworkShootingManager : NetworkBehaviour
    {
        public Gun curGun;

        private PlayerMovement _playerMovement;
        private PlayerLook _playerLook;

        private Transform spreadPoint;

        [Server]
        public void ServerFireRaycast()
        {
            RaycastHit _hit;
            if (Physics.Raycast(spreadPoint.position, spreadPoint.forward, out _hit, curGun.Range, curGun.HitLayers))
            {
                Debug.DrawRay(spreadPoint.position, spreadPoint.forward * curGun.Range, Color.green, 0.2f);

                if (_hit.transform.TryGetComponent(out DamagePart bodyPart))
                {
                    Debug.Log($"Hit body part {bodyPart.bodyPart}");
                }


            }
        }
    }
}
