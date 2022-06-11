using UnityEngine;
using TMPro;

public class DisplaysList : MonoBehaviour
{
    [SerializeField]
    TMP_Text option;

    [SerializeField]
    Transform instantiateTo;

    [SerializeField]
    GameObject UIObject;

    private void Start()
    {
        SpawnOptions();
    }

    private void OnEnable()
    {
        UpdateOptions();
    }

    public void SpawnOptions()
    {
        foreach (Transform child in instantiateTo)
        {
            Destroy(child.gameObject);
        }

        for (int d = 0; d < Display.displays.Length; d++)
        {
            ChangeDisplay di = Instantiate(UIObject, instantiateTo).GetComponent<ChangeDisplay>();

            di.monitorID = d;
        }
    }

    public void UpdateOptions()
    {
        option.SetText("Display " + (PlayerPrefs.GetInt("UnitySelectMonitor") + 1).ToString());
    }
}
