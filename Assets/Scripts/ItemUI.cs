using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

        Select(false);
    }

    public void Select(bool state)
    {
        image.color = state ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.15f);
    }
}
