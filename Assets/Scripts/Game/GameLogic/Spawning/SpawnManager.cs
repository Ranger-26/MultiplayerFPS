using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Random = System.Random;

namespace Game.GameLogic.Spawning
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        
        [SerializeField]
        private List<Vector3> _mtfSpawn = new() ;
        
        [SerializeField]
        private List<Vector3> _chaosSpawn = new();

        [SerializeField]
        private List<Vector3> _curChaosSpawn = new();
        
        [SerializeField]
        private List<Vector3> _curMtfSpawn = new();
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Another instance of GameUiManager already exists, destroying...");
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
        
        public void RegisterSpawnPoint(Vector3 point, SpawnType type)
        {
            switch (type)
            {
                case SpawnType.Mtf:
                    _mtfSpawn.Add(point);
                    _curMtfSpawn.Add(point);
                    break;
                case SpawnType.Chaos:
                    _chaosSpawn.Add(point);
                    _curChaosSpawn.Add(point);
                    break;
                default:
                    return;
            }
        }

        public void RemoveSpawnPoint(Vector3 point, SpawnType type)
        {
            switch (type)
            {
                case SpawnType.Mtf:
                    _mtfSpawn.Remove(point);
                    _curMtfSpawn.Remove(point);
                    break;
                case SpawnType.Chaos:
                    _chaosSpawn.Remove(point);
                    _curChaosSpawn.Remove(point);
                    break;
                default:
                    return;
            }
        }
        
        private Vector3 GetRandomSpawn(SpawnType type)
        {
            Vector3 position = Vector3.zero;

            List<Vector3> spawnList;
            switch (type) 
            {
                case SpawnType.Chaos:
                    spawnList = _curChaosSpawn;
                    break;
                case SpawnType.Mtf:
                    spawnList = _curMtfSpawn;
                    break;
                default:
                    return position;
            }

            int index = UnityEngine.Random.Range(0, spawnList.Count - 1);
            position = spawnList[index];

            if (spawnList.Count != 0)
            {
                Debug.Log($"Getting random spawn point of {spawnList[index]}");
                spawnList.RemoveAt(index);
            }

            return position;
        }

        public Vector3 GetRandomSpawn(Role role) =>
            GetRandomSpawn(role == Role.Mtf ? SpawnType.Mtf : SpawnType.Chaos);

        public void ResetSpawnPoints()
        {
            Debug.Log("Reseting spawn points..");
            _curChaosSpawn.Clear();
            _curMtfSpawn.Clear();
            _curMtfSpawn.AddRange(_mtfSpawn);
            _curChaosSpawn.AddRange(_chaosSpawn);
        }
    }
}