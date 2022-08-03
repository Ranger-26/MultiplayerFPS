using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Core.RuntimeData;
using Game.GameLogic.ItemSystem.Items.Firearms.Gunplay;
using Game.Player;
using Inputs;
using Lobby;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Items.Firearms
{
    public class FirearmBase : ItemBase
    {
        public GunViewModel GunViewModel;

        public NetworkShootingManager nsm;
        
        public FirearmRuntimeData FirearmData; 
            
        public override void InitItem(NetworkGamePlayer owner)
        {
            base.InitItem(owner);
            GunViewModel = GetComponent<GunViewModel>();
            nsm = Owner.GetComponent<NetworkShootingManager>();
        }

        public override bool OnEquip()
        {
            if (IsServer)
            {
                if (FirearmData.currentAmmo < 0 || FirearmData.ReserveAmmo < 0)
                {
                    nsm.reserveAmmo = GunViewModel.gun.ReserveAmmo;
                    nsm.currentAmmo = GunViewModel.gun.MaxAmmo;
                }
                else
                {
                    nsm.reserveAmmo = FirearmData.ReserveAmmo;
                    nsm.currentAmmo = FirearmData.currentAmmo;
                }
            }
            if (IsItemOwner)
            {
                GameUiManager.Instance.SetAmmoTextState(true);
                Invoke(nameof(SubscribeToEvents), ItemData.ItemDrawTime);
            }

            nsm.curGun = GunViewModel.gun;
            return true;
        }

        public override bool OnDeEquip()
        {
            if (nsm.isReloading) return false;
            if (NetworkPlayerLobby.localPlayer.isServer)
            {
                FirearmData.currentAmmo = nsm.currentAmmo;
                FirearmData.ReserveAmmo = nsm.reserveAmmo;
            }
            if (Owner.hasAuthority)
            {
                Debug.Log("Unsubscribing from events!");
                GameUiManager.Instance.SetAmmoTextState(false);
                UnSubscribeFromEvents();
            }
            
            return true;
        }

        public override void ServerSetRuntimeData(IRuntimeData data)
        {
            base.ServerSetRuntimeData(data);
            FirearmData = (FirearmRuntimeData) data;
        }

        public override bool ValidateRuntimeData(IRuntimeData runtimeData)
        {
            return runtimeData is FirearmRuntimeData;
        }

        public void SubscribeToEvents()
        {
            GameInputManager.PlayerActions.Fire.performed += GunViewModel.UpdateSpray;
            GameInputManager.PlayerActions.Fire.performed += GunViewModel.SemiAuto;
            GameInputManager.PlayerActions.Fire.canceled += GunViewModel.UpdateSpray;
            GameInputManager.PlayerActions.Reload.performed += GunViewModel.Reload;
            GameInputManager.PlayerActions.Inspect.performed += GunViewModel.Inspect;
            GameInputManager.PlayerActions.AltFire.performed += GunViewModel.UpdateScope;
        }

        public void UnSubscribeFromEvents()
        {
            GameInputManager.PlayerActions.Fire.performed -= GunViewModel.UpdateSpray;
            GameInputManager.PlayerActions.Fire.performed -= GunViewModel.SemiAuto;
            GameInputManager.PlayerActions.Fire.canceled -= GunViewModel.UpdateSpray;
            GameInputManager.PlayerActions.Reload.performed -= GunViewModel.Reload;
            GameInputManager.PlayerActions.Inspect.performed -= GunViewModel.Inspect;
            GameInputManager.PlayerActions.AltFire.performed -= GunViewModel.UpdateScope;
        }
        
        public void OnDestroy()
        {
            if (IsItemOwner)
            {
                UnSubscribeFromEvents();
            }
        }
    }
}