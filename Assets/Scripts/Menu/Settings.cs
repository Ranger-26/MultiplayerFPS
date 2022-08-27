using UnityEngine;

namespace Menu
{
    public static class Settings
    {
        public static Setting Current;

        public static void SaveSettings()
        {
            Debug.Log("Saved settings to \"" + Application.persistentDataPath + "/settings.tcrs\"");
            ES3.Save("Settings", Current, Application.persistentDataPath + "/settings.tcrs");
        }

        public static Setting LoadSettings()
        {
            Debug.Log("Loaded settings from \"" + Application.persistentDataPath + "/settings.tcrs\"");
            if (ES3.FileExists(Application.persistentDataPath + "/settings.tcrs"))
                Current = ES3.Load<Setting>("Settings", Application.persistentDataPath + "/settings.tcrs");
            else
            {
                Current = new Setting();
                SaveSettings();
            }
            
            return Current;
        }
    }

    [System.Serializable]
    public class Setting
    {
        #region Video

        // Display

        public int DisplayNumber;
        public int DisplayRes;

        public bool Fullscreen = true;
        public bool VSync = false;

        // Quality

        public float RenderResolution = 1;

        #endregion

        #region Audio

        public float MasterVolume;
        public float MusicVolume;
        public float EffectsVolume;

        #endregion

        #region Gameplay

        #endregion

        #region Methods

        public void SetVariable<T>(string name, T value)
        {
            GetType().GetField(name).SetValue(this, value);
        }

        public object GetVariableByName<T>(string name)
        {
            return GetType().GetField(name).GetValue(this);
        }

        #endregion

        public Setting()
        {

        }
    }
}