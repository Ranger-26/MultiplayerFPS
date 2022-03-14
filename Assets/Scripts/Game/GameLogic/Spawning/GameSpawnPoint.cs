using System;
using UnityEngine;

namespace Game.GameLogic.Spawning
{
    public class GameSpawnPoint : MonoBehaviour
    {
        private SpawnType type;

        public void Awake()
        {
            SpawnManager.Instance.RegisterSpawnPoint(transform, type);
        }

        public void OnDestroy()
        {
            SpawnManager.Instance.RemoveSpawnPoint(transform, type);
        }
    }
}