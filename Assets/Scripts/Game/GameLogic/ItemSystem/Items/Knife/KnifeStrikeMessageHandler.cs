using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Inventory;
using Game.Player;
using Game.Player.Damage;
using Mirror;
using Networking;
using System;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Knife
{
    public static class KnifeStrikeMessageHandler
    {
        public static GameObject MeleeHitObject;
        public static GameObject MeleeHitDecal;

        public static void OnReceiveMessage(NetworkConnection conn, KnifeStrikeMessage message)
        {
            PlayerInventory plr = conn.identity.GetComponent<PlayerInventory>();
            if (plr.currentItem != ItemIdentifier.Knife) return;
            RaycastHit[] _hits = Physics.SphereCastAll(message.Start, 1f, message.forward, KnifeComponent.Range, KnifeComponent.HitLayers, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(message.Start, message.forward, Color.blue, 1f);
            Debug.Log("Knife message recieved");
            if (_hits.Length != 0)
            {
                Array.Sort(_hits, (x, y) => x.distance.CompareTo(y.distance));
                int userId = conn.identity.GetComponent<NetworkGamePlayer>().playerId;
                foreach (RaycastHit __hit in _hits)
                {
                    DamagePart part = __hit.transform.GetComponentInChildren<DamagePart>();

                    if (part != null)
                    {
                        if (part.Player.playerId == userId)
                            continue;

                        part.ServerTag(KnifeComponent.Tagging);

                        if (Vector3.Dot(conn.identity.transform.forward, part.transform.forward) > 0f)
                        {
                            part.ServerDamage(KnifeComponent.Damage * (message.Heavy ? 1.5f : 1f), 2f);
                        }
                        else
                        {
                            part.ServerDamage(KnifeComponent.Damage * (message.Heavy ? 1.5f : 1f), 1f);
                        }

                        if (MeleeHitObject != null)
                        {
                            GameObject hit = UnityEngine.Object.Instantiate(MeleeHitObject, __hit.point, Quaternion.LookRotation(__hit.normal));
                            NetworkServer.Spawn(hit);
                        }
                    }
                    else
                    {
                        if (MeleeHitObject != null)
                        {
                            GameObject hit = UnityEngine.Object.Instantiate(MeleeHitObject, __hit.point, Quaternion.LookRotation(__hit.normal));
                            NetworkServer.Spawn(hit);
                        }

                        if (MeleeHitDecal != null)
                        {
                            GameObject decal = UnityEngine.Object.Instantiate(MeleeHitDecal, __hit.point, Quaternion.LookRotation(-__hit.normal));
                            NetworkServer.Spawn(decal);
                        }
                        return;
                    }
                }
            }
        }

        public static void RegisterHandlers()
        {
            NetworkServer.ReplaceHandler<KnifeStrikeMessage>(OnReceiveMessage);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init()
        {
            NetworkManagerScp.OnClientJoin += RegisterHandlers;
            MeleeHitObject = Resources.Load<GameObject>("Prefabs/SpawnablePrefabs/BulletHit");
            MeleeHitDecal = Resources.Load<GameObject>("Prefabs/SpawnablePrefabs/BulletHole");
        }
    }
}