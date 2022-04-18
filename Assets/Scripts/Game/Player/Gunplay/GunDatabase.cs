using System.Collections.Generic;
using System.Linq;
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

        public static bool TryGetGun(GunIDs gunId, out Gun newGun)
        {
            if (!idsToGuns.ContainsKey(gunId))
            {
                if (gunId != GunIDs.None)
                {
                    Debug.LogError($"Gun id {gunId} was not found in the dictionary of guns.");
                }
                newGun = null;
                return false;
            }

            newGun = idsToGuns[gunId];
            return true;
        }

        public static bool TryGetGunModel(GunIDs id, out GunViewModel newGunModel)
        { 
            if (!idsToModels.ContainsKey(id))
            {
                Debug.LogError($"Gun with ID {id} was not found in the dictionary of gun models.");
                newGunModel = null;
                return false;
            }

            newGunModel = idsToModels[id];
            return true;
        }

    }
}