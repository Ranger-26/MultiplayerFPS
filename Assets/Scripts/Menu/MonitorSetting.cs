using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Menu
{
    public class MonitorSetting : MonoBehaviour
    {
        TMP_Dropdown drop;

        private void Awake()
        {
            drop = GetComponentInChildren<TMP_Dropdown>();

            UpdateMonitors();
        }

        public void UpdateMonitors()
        {
            Display[] displays = Display.displays;

            List<string> vs = new List<string>();

            for (int i = 0; i < displays.Length; i++)
                vs.Add("Display " + (i + 1).ToString());

            drop.ClearOptions();
            drop.AddOptions(vs);
        }

        public void ChangeMonitor(int index)
        {
            PlayerPrefs.SetInt("UnitySelectMonitor", index);
        }
    }
}