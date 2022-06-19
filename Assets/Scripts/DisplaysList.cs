using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DisplaysList : MonoBehaviour
{
    TMP_Dropdown option;

    List<string> displays = new List<string>();

    private void Awake()
    {
        option = GetComponent<TMP_Dropdown>();

        for (int i = 0; i < Display.displays.Length; i++)
            displays.Add("Display " + (i + 1).ToString());
    }

    private void Start()
    {
        option.ClearOptions();
        option.AddOptions(displays);

        option.SetValueWithoutNotify(GameSettings.current.MonitorID);
    }

    private void OnEnable()
    {
        option.SetValueWithoutNotify(GameSettings.current.MonitorID);
    }

    public void ApplyDisplay(int monitorID)
    {
        PlayerPrefs.SetInt("UnitySelectMonitor", monitorID);
        PlayerPrefs.Save();
        GameSettings.current.MonitorID = monitorID;
        GameSettingsLoader.SaveFile();
    }
}
