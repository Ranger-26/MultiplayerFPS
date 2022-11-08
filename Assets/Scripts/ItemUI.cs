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
        image.sprite = itemBase.ItemData.Icon;

        PlayerInventory.Local.onEquip += UpdateItem; 
    }

    public void Disable()
    {
        PlayerInventory.Local.onEquip -= UpdateItem; 
    }
}
