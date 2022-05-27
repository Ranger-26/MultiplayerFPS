using UnityEngine;

public static class GameSettingsLoader
{
    public static void SaveFile(Settings sett)
    {
        ES3.Save("settings", sett, Application.persistentDataPath + "/Eternity Studios/SCP Intrusion/Settings");
    }

    public static Settings LoadFile()
    {
        return ES3.Load<Settings>("settings", Application.persistentDataPath + "/Eternity Studios/SCP Intrusion/Settings");
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

    public Crosshair ch;
}
