using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class Crosshair : MonoBehaviour
{
    public static Crosshair Instance;

    // Firing Error
    [HideInInspector]
    public float size = 25f;
    // User Config 
    public int offset = 5;
    public int length = 10;
    public int thickness = 3;

    public Color color = new Color(255f, 255f, 255f, 100f);

    public float firingErrorMultiplier = 10f;

    public bool firingError = true;

    float startingSize = 25f;

    RectTransform[] CrosshairParts;

    RectTransform rect;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        rect = GetComponent<RectTransform>();

        CrosshairParts = transform.GetComponentsInChildren<RectTransform>();
        CrosshairParts = CrosshairParts.Skip(1).ToArray();
        Debug.Log($"Found {CrosshairParts.Length} cross parts.");
        foreach (RectTransform rectTrans in CrosshairParts)
        {
            if (rectTrans.gameObject.GetComponent<Image>() == null) Debug.Log($"Crosshair part image null...");
            rectTrans.sizeDelta = new Vector2(length, thickness);
            rectTrans.gameObject.GetComponent<Image>().color = color;
        }

        size = length * 2 + offset;
        startingSize = size;

        rect.sizeDelta = new Vector2(size, size);
    }

    public void UpdateError(float errorPixelsFire)
    {
        if (!firingError)
            return;

        size = startingSize + errorPixelsFire * firingErrorMultiplier * Convert.ToInt32(firingError);

        //Debug.Log(size);

        rect.sizeDelta = new Vector2(size, size);
    }
}
