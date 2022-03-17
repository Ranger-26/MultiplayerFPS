using System;
using System.Collections.Generic;
using System.Linq;
using Game.Player;
using Mirror;
using UnityEngine;

namespace Game.GameLogic
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
}