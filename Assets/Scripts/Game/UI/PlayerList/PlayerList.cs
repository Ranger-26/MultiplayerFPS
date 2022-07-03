using System.Collections.Generic;
using Lobby;
using Networking;
using UnityEngine;

namespace Game.UI.PlayerList
{
    public class PlayerList : MonoBehaviour
    {
        public string[] GetAllPlayerNames()
        {
            string[] players = new string[NetworkManagerScp.Instance.allPlayers.Count];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] =  NetworkManagerScp.Instance.allPlayers[i].playerName; 
            }
            return players;
        }
    }
}