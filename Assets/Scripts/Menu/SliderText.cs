using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Menu
{
    public class SliderText : MonoBehaviour
    {
        TMP_InputField text;

        Slider slider;

        private void Awake()
        {
            text = GetComponent<TMP_InputField>();
            slider = GetComponentInParent<Slider>();
        }

        private void OnEnable()
        {
            text.text = Math.Round(slider.value, 2, MidpointRounding.ToEven).ToString();
        }

        public void UpdateValue(float value)
        {
            text.text = Math.Round(value, 2, MidpointRounding.ToEven).ToString();
            slider.SetValueWithoutNotify(Convert.ToSingle(Math.Round(value, 2, MidpointRounding.ToEven)));
        }

        public void UpdateSlider(string value)
        {
            if (float.TryParse(value, out float res))
                slider.value = res;
            else
                slider.value = slider.minValue;
        }
    }
}