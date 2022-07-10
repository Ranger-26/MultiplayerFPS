using System;
using Game.GameLogic.ItemSystem.Items.Firearms.Gunplay;
using Game.Player;
using Game.Player.Damage;
using Mirror;
using Networking;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Knife
{
    public static class KnifeStrikeMessageHandler
    {
        public static float Range = 2f;
        public static float Tagging = 1f;
        public static float Damage = 40f;
        public static float DrawTime = 0.5f;
        public static LayerMask HitLayers = 65;
        
        public static GameObject MeleeHitObject;
        public static GameObject MeleeHitDecal;
        
        public static void OnReceiveMessage(NetworkConnection conn, KnifeStrikeMessage message)
        {
            RaycastHit[] _hits = Physics.RaycastAll(message.Start, message.forward, Range, HitLayers);
            Debug.DrawRay(message.Start, message.forward, Color.blue);
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

                        part.ServerTag(Melee.Tagging);

                        if (Vector3.Dot(conn.identity.transform.forward, part.transform.forward) > 0f)
                        {
                            part.ServerDamage(Damage, 2f);
                        }
                        else
                        {
                            part.ServerDamage(Damage, 1f);
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
                            GameObject decal = UnityEngine.Object.Instantiate(MeleeHitDecal, __hit.point, Quaternion.LookRotation(-__hit.normal)) as GameObject;
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