using System.Collections.Generic;
using UnityEngine;

namespace Game.Player.Gunplay
{
    public static class GunDatabase
    {
        public static Dictionary<GunIDs, GunViewModel> idsToModels = new Dictionary<GunIDs, GunViewModel>();

        public static Dictionary<GunIDs, Gun> idsToGuns = new Dictionary<GunIDs, Gun>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init()
        {
            GunViewModel[] gunmodels = Resources.LoadAll<GunViewModel>("Prefabs/ViewModels");
            
            Gun[] guns = Resources.LoadAll<Gun>("Scriptables/Guns");

            foreach (var model in gunmodels)
            {
                idsToModels.Add(model.gunId, model);
            }

            foreach (var gun in guns)
            {
                idsToGuns.Add(gun.UniqueGunID, gun);
            }
            
            Debug.Log($"Found {idsToGuns.Count} guns and {idsToModels.Count} models!");
        }
    }
}