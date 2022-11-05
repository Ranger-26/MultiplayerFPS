using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Inventory;
using UnityEngine;

public class ItemUIManager : MonoBehaviour
{
    public GameObject UIPrefab;

    private void Start() => Enable();
    private void OnEnable() => Enable();

    private void OnDisable() => Disable();
    private void OnDestroy() => Disable();

    public void Enable()
    {
        PlayerInventory.Local.onInventoryUpdate += UpdateInventory; // Need to run this after the player spawns in

        UpdateInventory();
    }

    public void Disable()
    {
        PlayerInventory.Local.onInventoryUpdate -= UpdateInventory; // Note: This will cause a null ref since this gets destroyed after the player when they leave
    }

    public void UpdateInventory()
    {
        foreach (Transform tr in transform)
        {
            if (tr != transform)
                Destroy(tr.gameObject);

            Debug.Log("Destroying UI");
        }

        for (int i = 0; i < PlayerInventory.Local.allItemBases.Count; i++)
        {
            ItemUI ui = Instantiate(UIPrefab, transform).GetComponent<ItemUI>();
            ui.itemBase = PlayerInventory.Local.allItemBases[i];

            Debug.Log("Spawning UI");
        }
    }
}
