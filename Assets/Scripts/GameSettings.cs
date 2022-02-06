using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameSettingsLoader
{
    public static GameSettings SavedSettings;

    public static void SaveFile(GameSettings data)
    {
        string destination = Application.persistentDataPath + "/settings.data";
        FileStream file;

        if (File.Exists(destination)) 
            file = File.OpenWrite(destination);
        else 
            file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();

        SavedSettings = data;
    }

    public static GameSettings LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) 
            file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            SavedSettings = new GameSettings();
            return new GameSettings();
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameSettings data = (GameSettings)bf.Deserialize(file);
        file.Close();

        SavedSettings = data;

        return data;
    }
}

[System.Serializable]
public class GameSettings
{
    public float Sensitivity = 1.0f;
}
