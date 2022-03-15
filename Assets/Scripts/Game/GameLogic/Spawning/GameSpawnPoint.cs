using System;
using UnityEngine;

namespace Game.GameLogic.Spawning
{
    public class GameSpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private SpawnType type;

        public void Start()
        {
            SpawnManager.Instance.RegisterSpawnPoint(transform, type);
        }

        public void OnDestroy()
        {
            SpawnManager.Instance.RemoveSpawnPoint(transform, type);
        }
    }
}