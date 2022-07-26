using TMPro;
using UnityEngine;

public class QualityChange : MonoBehaviour
{
    TMP_Dropdown option;

    private void Awake() => option = GetComponent<TMP_Dropdown>();

    private void Start() => option.SetValueWithoutNotify(GameSettings.current.QualityLvl);
    private void OnEnable() => option.SetValueWithoutNotify(GameSettings.current.QualityLvl);

    public void ChangeQuality(int ql)
    {
        QualitySettings.SetQualityLevel(ql);
        GameSettings.current.QualityLvl = ql;
        GameSettingsLoader.SaveFile();
    }
}
