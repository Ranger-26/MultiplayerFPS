using Menu;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public static Crosshair Instance;

    [HideInInspector]
    public float size = 24f;

    [SerializeField]
    RectTransform Reticles;
    [SerializeField]
    RectTransform Dot;

    float startingSize = 24f;
    float offsetAddition = 1f;

    RectTransform[] CrosshairParts;

    RectTransform rect;

    private void Awake()
    {
        if (Instance != null) gameObject.SetActive(false);
        else Instance = this;
    }

    private void Start() => Init();

    private void OnEnable() => Init();

    private void Update()
    {
        rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, new Vector2(size, size), 0.5f);
    }

    private void Init()
    {
        UpdateCrosshair();
    }

    public void UpdateCrosshair()
    {
        CrosshairVisibility(true);

        rect = transform.GetComponent<RectTransform>();
        CrosshairParts = transform.GetComponentsInChildren<RectTransform>();
        CrosshairParts = CrosshairParts.Skip(1).ToArray();

        #region Offsetting Crosshair Part

        if ((Settings.Current.cThickness % 2f) > 0f)
        {
            Reticles.localPosition = new Vector2(1, 1);
            offsetAddition = 1f;
        }
        else
        {
            Reticles.localPosition = Vector2.zero;
            offsetAddition = 0f;
        }

        if ((Settings.Current.cDotSize % 2f) > 0f)
        {
            Dot.localPosition = new Vector2(1, 1);
            offsetAddition = 1f;
        }
        else
        {
            Dot.localPosition = Vector2.zero;
            offsetAddition = 0f;
        }

        #endregion

        foreach (RectTransform rectTrans in CrosshairParts)
        {
            if (rectTrans.gameObject.GetComponent<Image>() == null) { Debug.Log($"Crosshair part image null..."); continue; }

            if (rectTrans.gameObject.name == "Reticle")
                rectTrans.sizeDelta = new Vector2(Settings.Current.cLength, Settings.Current.cThickness);
            else if (rectTrans.gameObject.name == "Dot")
                rectTrans.sizeDelta = new Vector2(Settings.Current.cDotSize, Settings.Current.cDotSize);

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

        size = Settings.Current.cLength * 2f + Settings.Current.cOffset * 2f + offsetAddition;
        startingSize = size;

        rect.localScale = new Vector3(Settings.Current.cScale, Settings.Current.cScale, 1f);

        rect.sizeDelta = new Vector2(size, size);
    }

    public void UpdateError(float errorPixelsFire)
    {
        size = startingSize + errorPixelsFire * Settings.Current.cFiringErrorMultiplier * 9f;
    }

    public void ActivateCrosshair()
    {
        Instance.gameObject.SetActive(false);
        Instance = this;
        gameObject.SetActive(true);
    }

    public void CrosshairVisibility(bool state)
    {
        if (Reticles != null)
            Reticles.gameObject.SetActive(state);
        if (Dot != null)
            Dot.gameObject.SetActive(state);
    }
}
