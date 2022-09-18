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
            Vector3 transform;
            if (type == SpawnType.Mtf)
            {
                if (_curChaosSpawn.Count == 0)
                {
                    transform = _curMtfSpawn[UnityEngine.Random.Range(0, _curMtfSpawn.Count - 1)];
                    return transform;
                }
                int index = UnityEngine.Random.Range(0, _curMtfSpawn.Count - 1);
                Debug.Log($"Getting random spawn point of {_curMtfSpawn[index]}");
                transform = _curMtfSpawn[index]; 
                _curMtfSpawn.Remove(_curMtfSpawn[index]);
            }
            else
            {
                if (_curChaosSpawn.Count == 0)
                {
                    transform = _curMtfSpawn[UnityEngine.Random.Range(0, _curChaosSpawn.Count - 1)];
                    return transform;
                }
                int indexChaos = UnityEngine.Random.Range(0, _curChaosSpawn.Count - 1);
                Debug.Log($"Getting random spawn point of {_curChaosSpawn[indexChaos]}");
                transform = _curChaosSpawn[indexChaos];
                _curChaosSpawn.Remove(_curChaosSpawn[indexChaos]);
            }

            return transform;
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