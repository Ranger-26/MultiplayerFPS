using UnityEngine;
using System.IO;

public static class GameSettingsLoader
{
    public static void SaveFile()
    {
        ES3.Save("settings", GameSettings.current, Application.persistentDataPath + "/Settings/general.dat");
    }

    public static Settings LoadFile()
    {
        if (File.Exists(Application.persistentDataPath + "/Settings/general.dat"))
            return ES3.Load<Settings>("settings", Application.persistentDataPath + "/Settings/general.dat");
        else
        {
            Settings temp = new Settings();

            SaveFile();
            return temp;
        }
    }
}

public static class GameSettings
{
    public static Settings current
    {
        get
        {
            if (cur == null)
                cur = new Settings();
            return cur;
        }
        set => cur = value;
    }

    static Settings cur;
}

[System.Serializable]
public class Settings
{
    public float Sensitivity;

    public int MonitorID;
    public int ScreenMode;

    public CrosshairSettings ch;

    public Settings()
    {
        Sensitivity = 1f;
        MonitorID = 0;
        ch = new CrosshairSettings();
    }
}

[System.Serializable]
public class CrosshairSettings
{
    public int offset = 5;
    public int length = 10;
    public int thickness = 3;

    public int scale = 1;

    public Color color = new Color(255f, 255f, 255f, 100f);

    public float firingErrorMultiplier = 10f;

    public CrosshairSettings()
    {
        offset = 5;
        length = 10;
        thickness = 3;

        scale = 1;

        color = new Color(255f, 255f, 255f, 100f);

        firingErrorMultiplier = 10f;
    }

    public CrosshairSettings(int _thickness, int _length, int _offset)
    {
        offset = _offset;
        length = _length;
        thickness = _thickness;
    }
}
