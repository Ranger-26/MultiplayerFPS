using UnityEngine;
using System.IO;

public static class GameSettingsLoader
{
    public static void SaveFile(Settings sett)
    {
        ES3.Save("settings", sett, Application.persistentDataPath + "/Settings/general.dat");
    }

    public static Settings LoadFile()
    {
        if (File.Exists(Application.persistentDataPath + "/Settings/general.dat"))
            return ES3.Load<Settings>("settings", Application.persistentDataPath + "/Settings/general.dat");
        else
        {
            Settings temp = new Settings();

            SaveFile(temp);
            return temp;
        }
    }
}

public static class GameSettings
{
    public static Settings current;
}

[System.Serializable]
public class Settings
{
    public float Sensitivity;

    public CrosshairSettings ch = new CrosshairSettings();

    public Settings()
    {
        Sensitivity = 1f;
        ch = new CrosshairSettings();
    }
}

[System.Serializable]
public class CrosshairSettings
{
    public int offset = 5;
    public int length = 10;
    public int thickness = 3;

    public Color color = new Color(255f, 255f, 255f, 100f);

    public float firingErrorMultiplier = 10f;

    public bool firingError = true;

    public CrosshairSettings()
    {
        offset = 5;
        length = 10;
        thickness = 3;
    }

    public CrosshairSettings(int _thickness, int _length, int _offset)
    {
        offset = _offset;
        length = _length;
        thickness = _thickness;
    }
}
