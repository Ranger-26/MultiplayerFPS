using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Menu;

public class Crosshair : MonoBehaviour
{
    public static Crosshair Instance;

    [HideInInspector]
    public float size = 24f;

    float startingSize = 24f;

    RectTransform[] CrosshairParts;

    RectTransform rect;

    private void Awake()
    {
        if (Instance != null) gameObject.SetActive(false);
        else Instance = this;
    }

    private void Start() => Init();

    private void OnEnable() => Init();

    private void Init()
    {
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        UpdateCrosshair();
    }

    public void UpdateCrosshair()
    {
        rect = transform.GetComponent<RectTransform>();
        CrosshairParts = transform.GetComponentsInChildren<RectTransform>();
        CrosshairParts = CrosshairParts.Skip(1).ToArray();

        foreach (RectTransform rectTrans in CrosshairParts)
        {
            if (rectTrans.gameObject.GetComponent<Image>() == null) Debug.Log($"Crosshair part image null...");

            rectTrans.sizeDelta = new Vector2(Settings.Current.cLength * 2f, Settings.Current.cThickness * 2f);

            switch (Settings.Current.cColor)
            {
                case 0:
                    rectTrans.gameObject.GetComponent<Image>().color = Color.white;
                    break;
                case 1:
                    rectTrans.gameObject.GetComponent<Image>().color = Color.red;
                    break;
                case 2:
                    rectTrans.gameObject.GetComponent<Image>().color = Color.green;
                    break;
                case 3:
                    rectTrans.gameObject.GetComponent<Image>().color = Color.blue;
                    break;
                case 4:
                    rectTrans.gameObject.GetComponent<Image>().color = Color.yellow;
                    break;
                case 5:
                    rectTrans.gameObject.GetComponent<Image>().color = Color.magenta;
                    break;
                case 6:
                    rectTrans.gameObject.GetComponent<Image>().color = Color.cyan;
                    break;
                case 7:
                    rectTrans.gameObject.GetComponent<Image>().color = Color.black;
                    break;
            }
        }

        size = Settings.Current.cLength * 4f + Settings.Current.cOffset * 2f;
        startingSize = size;

        rect.localScale = new Vector3(Settings.Current.cScale, Settings.Current.cScale, 1f);

        rect.sizeDelta = new Vector2(size, size);
    }

    public void UpdateError(float errorPixelsFire)
    {
        size = startingSize + errorPixelsFire * Settings.Current.cFiringErrorMultiplier * 2f;

        rect.sizeDelta = new Vector2(size, size);
    }

    public void ActivateCrosshair()
    {
        Instance.gameObject.SetActive(false);
        Instance = this;
        gameObject.SetActive(true);
    }
}
