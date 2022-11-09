using Game.Player;
using UnityEngine;

namespace Game.GameLogic.ItemSystem.Inventory
{
    public class ItemUIManager : MonoBehaviour
    {
        public GameObject UIPrefab;

        private void Start() => Enable();
        private void OnEnable() => Enable();

        private void OnDisable() => Disable();
        private void OnDestroy() => Disable();

        public void Enable()
        {
            GameUiManager.onLocalPlayerSpawn += SubscribeToEvents;
            GameUiManager.onLocalPlayerDisconnect += UnSubscribeToEvents;
        }

        public void Disable()
        {
            GameUiManager.onLocalPlayerSpawn -= SubscribeToEvents;
            GameUiManager.onLocalPlayerDisconnect -= UnSubscribeToEvents;
        }

        public void SubscribeToEvents()
        {
            Invoke(nameof(SubEvents), 0.1f);
        }

        public void UnSubscribeToEvents()
        {
            Invoke(nameof(UnSubEvents), 0.1f);
        }

        void SubEvents()
        {
            PlayerInventory.Local.onInventoryUpdate += UpdateInventory;

            UpdateInventory();
        }

        void UnSubEvents()
        {
            PlayerInventory.Local.onInventoryUpdate -= UpdateInventory;
        }

        public void UpdateInventory()
        {
            Debug.Log("Count: " + PlayerInventory.Local.allItemBases.Count);

            if (transform.childCount > 0)
            {
                foreach (Transform tr in transform)
                {
                    if (tr != transform)
                        Destroy(tr.gameObject);

                    Debug.Log("Destroying UI");
                }
            }

            for (int i = 0; i < PlayerInventory.Local.allItemBases.Count; i++)
            {
                GameObject _ui = Instantiate(UIPrefab, transform);
                ItemUI ui = _ui.GetComponent<ItemUI>();
                ui.itemBase = PlayerInventory.Local.allItemBases[i];

                Debug.Log("Spawning UI");
            }
        }
    }
}
