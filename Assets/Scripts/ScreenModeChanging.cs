using UnityEngine;
using TMPro;

public class ScreenModeChanging : MonoBehaviour
{
    TMP_Dropdown option;

    private void Awake() => option = GetComponent<TMP_Dropdown>();

    private void Start() => option.SetValueWithoutNotify(GameSettings.current.ScreenMode);

    private void OnEnable() => option.SetValueWithoutNotify(GameSettings.current.ScreenMode);

    public void ChangeScreenMode(int screenMode)
    {
        Screen.fullScreenMode = (FullScreenMode)screenMode;
        GameSettings.current.ScreenMode = screenMode;
        GameSettingsLoader.SaveFile();
    }
}
