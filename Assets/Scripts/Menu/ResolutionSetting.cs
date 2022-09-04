using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Menu
{
    public class ResolutionSetting : MonoBehaviour
    {
        TMP_Dropdown drop;

        Resolution[] reses;

        private void Awake()
        {
            drop = GetComponentInChildren<TMP_Dropdown>();

            UpdateResolutions();
        }

        public void UpdateResolutions()
        {
            reses = Screen.resolutions;

            Array.Reverse(reses);

            List<string> vs = new List<string>();

            foreach (Resolution res in reses)
                vs.Add(res.ToString());

            drop.ClearOptions();
            drop.AddOptions(vs);
        }

        public void ChangeRes(int index)
        {
            Screen.SetResolution(reses[index].width, reses[index].height, Settings.Current.Fullscreen, reses[index].refreshRate);
        }
    }
}