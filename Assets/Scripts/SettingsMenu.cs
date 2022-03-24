using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    InputField sens;

    private void OnEnable()
    {
        sens.text = GameSettings.Sensitivity.ToString();
    }

    public void Sensitivity()
    {
        float sen = float.Parse(sens.text);
        GameSettings.Sensitivity = sen;
        GameSettingsLoader.SaveFile();
    }
}
