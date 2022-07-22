using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ResolutionChange : MonoBehaviour
{
    TMP_Dropdown option;

    Resolution[] reses;

    private void Awake()
    {
        option = GetComponent<TMP_Dropdown>();

        reses = Screen.resolutions;

        option.ClearOptions();

        List<string> vs = new List<string>();

        for (int i = 0; i < reses.Length; i++)
        {
            vs.Add(reses[i].ToString());
        }

        option.AddOptions(vs);
    }

    private void Start() => option.SetValueWithoutNotify(GameSettings.current.ScreenMode); // change this

    private void OnEnable() => option.SetValueWithoutNotify(GameSettings.current.ScreenMode);

    public void ChangeResolution(int index)
    {
        Screen.SetResolution(reses[index].width, reses[index].height, Screen.fullScreenMode, reses[index].refreshRate);
        GameSettings.current.resolution = reses[index];
    }
}
