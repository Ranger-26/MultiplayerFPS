using Game.GameLogic.ItemSystem.Core;
using Game.GameLogic.ItemSystem.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    [HideInInspector]
    public ItemBase itemBase;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

        Select(false);
    }

    private void Start() => Enable();
    private void OnEnable() => Enable();

    private void OnDisable() => Disable();
    private void OnDestroy() => Disable();

    public void Select(bool state)
    {
        image.color = state ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.15f);
    }

    public void UpdateItem(ItemBase id)
    {
        if (itemBase == id)
            Select(true);
        else
            Select(false);
    }

    public void Enable()
    {
        PlayerInventory.Local.onEquip += UpdateItem; // Need to run this after the player spawns in
    }

    public void Disable()
    {
        PlayerInventory.Local.onEquip -= UpdateItem; // Note: This will cause a null ref since this gets destroyed after the player when they leave
    }
}
