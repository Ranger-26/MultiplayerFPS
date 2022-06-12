using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeDisplay : MonoBehaviour
{
    public int monitorID;

    [SerializeField]
    TMP_Text txt;

    Toggle tog;

    DisplaysList dl;

    private void Awake()
    {
        dl = GetComponentInParent<DisplaysList>();
        tog = GetComponent<Toggle>();

        tog.isOn = PlayerPrefs.GetInt("UnitySelectMonitor") == monitorID;

        tog.group = GetComponentInParent<ToggleGroup>();
    }

    private void Start()
    {
        txt.SetText("Display " + (monitorID + 1).ToString());
    }

    private void OnEnable()
    {
        tog.isOn = PlayerPrefs.GetInt("UnitySelectMonitor") == monitorID;
    }

    public void SetMonitor()
    {
        PlayerPrefs.SetInt("UnitySelectMonitor", monitorID);
        PlayerPrefs.Save();
        dl.UpdateOptions(monitorID);

        Display.displays[monitorID].Activate();
    }
}
