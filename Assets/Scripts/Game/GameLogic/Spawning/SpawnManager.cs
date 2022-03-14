using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Random = System.Random;

namespace Game.GameLogic.Spawning
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        
        private List<Transform> _mtfSpawn = new List<Transform>();
        
        private List<Transform> _chaosSpawn = new List<Transform>();

        private List<Transform> _usedPositions = new List<Transform>();
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
                    break;
                case SpawnType.Chaos:
                    _chaosSpawn.Add(point);
                    break;
                default:
                    return;
            }
        }

        public Transform GetRandomSpawn(SpawnType type)
        {
            if (type == SpawnType.Mtf)
            {
                int index = UnityEngine.Random.Range(0, _mtfSpawn.Count - 1);
                _usedPositions.Add(_mtfSpawn[index]);
                return _mtfSpawn[index];
            }

            return null;
        }
    }
}