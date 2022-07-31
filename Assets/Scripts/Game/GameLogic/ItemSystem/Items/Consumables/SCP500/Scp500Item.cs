using System.Collections;
using Game.GameLogic.ItemSystem.Core;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.GameLogic.ItemSystem.Items.Consumables.SCP500
{
    public class Scp500Item : UsableItemBase
    {
        public override void OnUse(InputAction.CallbackContext ctx)
        {
            NetworkClient.Send(new Scp500Message()
            {
                time = NetworkTime.time
            });
            //play animation
        }
    }
}