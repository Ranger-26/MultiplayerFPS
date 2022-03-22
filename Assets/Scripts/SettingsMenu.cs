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

    public void Sensitivity(string sense)
    {
        float sen = float.Parse(sense);
        GameSettings.Sensitivity = sen;
        GameSettingsLoader.SaveFile();
    }
}
