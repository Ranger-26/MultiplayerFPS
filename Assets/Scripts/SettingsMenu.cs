using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    InputField sens;

    [SerializeField]
    InputField c_width;
    [SerializeField]
    InputField c_height;
    [SerializeField]
    InputField c_offset;

    private void Awake()
    {
        GameSettings.current = GameSettingsLoader.LoadFile();
    }

    private void OnEnable()
    {
        sens.text = GameSettings.current.Sensitivity.ToString();

        Vector3 crs = new Vector3(GameSettings.current.ch.length, GameSettings.current.ch.thickness, GameSettings.current.ch.offset);

        c_width.text = ((int)crs.x).ToString();
        c_height.text = ((int)crs.y).ToString();
        c_offset.text = ((int)crs.z).ToString();
    }

    public void Sensitivity()
    {
        float sen = float.Parse(sens.text);
        GameSettings.current.Sensitivity = sen;
        GameSettingsLoader.SaveFile(GameSettings.current);
    }

    public void SetCrosshair()
    {
        Vector3 crs = new Vector3(int.Parse(c_width.text), int.Parse(c_height.text), int.Parse(c_offset.text));

        GameSettings.current.ch.length = (int)crs.x;
        GameSettings.current.ch.thickness = (int)crs.y;
        GameSettings.current.ch.offset = (int)crs.z;

        GameSettingsLoader.SaveFile(GameSettings.current);
    }
}
