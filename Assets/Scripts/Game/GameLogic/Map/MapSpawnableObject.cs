using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameLogic.Map
{
    public class MapSpawnableObject : MonoBehaviour
    {
        public static List<MapSpawnableObject> allObjects = new();

        private void Start()
        {
            allObjects.Add(this);
        }

        public static void DestroyAllMapObjects()
        {
            foreach (var obj in allObjects)
            {
                if (obj != null)
                {
                    Destroy(obj.gameObject);
                }
            }
            allObjects.Clear();
        }
    }
}