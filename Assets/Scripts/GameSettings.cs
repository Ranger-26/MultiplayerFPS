using UnityEngine;

public static class GameSettingsLoader
{
    public static void SaveFile()
    {
        PlayerPrefs.SetFloat(StringKeys.PlayerPrefsKeySens, GameSettings.Sensitivity);
        PlayerPrefs.Save();
    }

    public static void LoadFile()
    {
        if (PlayerPrefs.HasKey(StringKeys.PlayerPrefsKeySens))
        {
            GameSettings.Sensitivity = PlayerPrefs.GetFloat(StringKeys.PlayerPrefsKeySens);
        }
        else
        {
            SaveFile();
        }
    }
}

public static class GameSettings
{
    public static float Sensitivity = 1.0f;
}
