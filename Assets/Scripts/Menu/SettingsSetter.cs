using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Menu
{
    public class SettingsSetter : MonoBehaviour
    {
        public string VariableName;

        TMP_Dropdown dropdown;
        Toggle toggle;
        TMP_InputField input;
        Slider slider;

        private void Awake()
        {
            dropdown = GetComponentInChildren<TMP_Dropdown>();
            toggle = GetComponentInChildren<Toggle>();
            input = GetComponentInChildren<TMP_InputField>();
            slider = GetComponentInChildren<Slider>();
        }

        private void OnEnable()
        {
            if (dropdown != null)
                dropdown.SetValueWithoutNotify((int)Settings.Current.GetVariableByName<int>(VariableName));
            if (toggle != null)
                toggle.SetIsOnWithoutNotify((bool)Settings.Current.GetVariableByName<bool>(VariableName));
            if (input != null)
                input.SetTextWithoutNotify(Settings.Current.GetVariableByName<string>(VariableName).ToString());
            if (slider != null)
                slider.SetValueWithoutNotify((float)Settings.Current.GetVariableByName<float>(VariableName));
        }

        public void SetDropdown(int value)
        {
            Settings.Current.SetVariable(VariableName, value);

            Settings.SaveSettings();
        }

        public void SetToggle(bool value)
        {
            Settings.Current.SetVariable(VariableName, value);

            Settings.SaveSettings();
        }

        public void SetInput(string value)
        {
            Settings.Current.SetVariable(VariableName, value);

            Settings.SaveSettings();
        }

        public void SetSlider(float value)
        {
            Settings.Current.SetVariable(VariableName, value);

            Settings.SaveSettings();
        }
    }
}