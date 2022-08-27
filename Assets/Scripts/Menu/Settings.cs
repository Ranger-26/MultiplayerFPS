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

        public float Sensitivity;

        public CrosshairSettings ch;

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
}