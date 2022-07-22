using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ResolutionChange : MonoBehaviour
{
    TMP_Dropdown option;

    Resolution[] reses;

    int cur;

    private void Awake()
    {
        option = GetComponent<TMP_Dropdown>();

        reses = Screen.resolutions;

        option.ClearOptions();

        List<string> vs = new List<string>();

        cur = 0;
        for (int i = 0; i < reses.Length; i++)
        {
            vs.Add(reses[i].ToString());

            if (reses[i].width == Screen.currentResolution.width && reses[i].height == Screen.currentResolution.height) cur = i;
        }

        option.AddOptions(vs);
    }

    private void Start() => option.SetValueWithoutNotify(cur);

    private void OnEnable() => option.SetValueWithoutNotify(cur);

    public void ChangeResolution(int index)
    {
        Screen.SetResolution(reses[index].width, reses[index].height, Screen.fullScreenMode, reses[index].refreshRate);
    }
}
