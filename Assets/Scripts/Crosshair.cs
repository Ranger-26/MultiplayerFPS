using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Crosshair : MonoBehaviour
{
    public static Crosshair Instance;

    // Firing Error
    [HideInInspector]
    public float size = 24f;

    // User Config
    public CrosshairSettings ch = new CrosshairSettings();

    float startingSize = 24f;

    RectTransform[] CrosshairParts;

    RectTransform rect;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        rect = GetComponent<RectTransform>();
    }

    private void Start() => Init();

    private void OnEnable() => Init();

    private void Init()
    {
        UpdateSettings();
    }

    public void UpdateSettings() 
    {
        ch = GameSettings.current.ch;

        UpdateCrosshair();
    }

    public void UpdateCrosshair()
    {
        CrosshairParts = transform.GetComponentsInChildren<RectTransform>();
        CrosshairParts = CrosshairParts.Skip(1).ToArray();

        foreach (RectTransform rectTrans in CrosshairParts)
        {
            if (rectTrans.gameObject.GetComponent<Image>() == null) Debug.Log($"Crosshair part image null...");

            rectTrans.sizeDelta = new Vector2(ch.length * 2f, ch.thickness * 2f);
            rectTrans.gameObject.GetComponent<Image>().color = ch.color;
        }

        size = ch.length * 4f + ch.offset * 2f;
        startingSize = size;

        rect.localScale = new Vector3(ch.scale, ch.scale, 1f);

        rect.sizeDelta = new Vector2(size, size);
    }

    public void UpdateError(float errorPixelsFire)
    {
        size = startingSize + errorPixelsFire * ch.firingErrorMultiplier * 2f;

        rect.sizeDelta = new Vector2(size, size);
    }
}
