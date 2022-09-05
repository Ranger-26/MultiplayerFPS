using Mirror;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Knife
{
    public struct KnifeStrikeMessage : NetworkMessage
    {
        public Vector3 Start;
        public Vector3 forward;

        public bool Heavy;
    }
}