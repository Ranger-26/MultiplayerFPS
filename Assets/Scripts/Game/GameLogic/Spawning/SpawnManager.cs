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
        private List<Transform> _mtfSpawn = new List<Transform>();
        
        [SerializeField]
        private List<Transform> _chaosSpawn = new List<Transform>();

        [SerializeField]
        private List<Transform> _curChaosSpawn = new List<Transform>();
        
        [SerializeField]
        private List<Transform> _curMtfSpawn = new List<Transform>();
        
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
        
        public void RegisterSpawnPoint(Transform point, SpawnType type)
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

        public void RemoveSpawnPoint(Transform point, SpawnType type)
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
        
        private Transform GetRandomSpawn(SpawnType type)
        {
            Transform transform;
            if (type == SpawnType.Mtf)
            {
                int index = UnityEngine.Random.Range(0, _curMtfSpawn.Count - 1);
                Debug.Log($"Getting random spawn point of {_curMtfSpawn[index].position}");
                transform = _curMtfSpawn[index]; 
                _curMtfSpawn.Remove(_curMtfSpawn[index]);
            }
            else
            {
                int indexChaos = UnityEngine.Random.Range(0, _curChaosSpawn.Count - 1);
                Debug.Log($"Getting random spawn point of {_curChaosSpawn[indexChaos].position}");
                transform = _curChaosSpawn[indexChaos];
                _curChaosSpawn.Remove(_curChaosSpawn[indexChaos]);
            }

            return transform;
        }

        public Transform GetRandomSpawn(Role role) =>
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