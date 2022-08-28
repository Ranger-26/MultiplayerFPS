using UnityEngine;

namespace Menu
{
    using System;

    public static class Settings
    {
        public static Setting Current;

        public static void SaveSettings()
        {
            Debug.Log("Saved settings to \"" + Application.persistentDataPath + "/settings.scp\"");
            ES3.Save("Settings", Current, Application.persistentDataPath + "/settings.scp");
        }

        public static Setting LoadSettings()
        {
            Debug.Log("Loaded settings from \"" + Application.persistentDataPath + "/settings.scp\"");
            if (ES3.FileExists(Application.persistentDataPath + "/settings.scp"))
                Current = ES3.Load<Setting>("Settings", Application.persistentDataPath + "/settings.scp");
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



        #endregion

        #region Audio

        public float MasterVolume;
        public float MusicVolume;
        public float EffectsVolume;

        #endregion

        #region Gameplay

        public float Sensitivity;

        // Crosshair

        public int cOffset = 5;
        public int cLength = 10;
        public int cThickness = 3;
        public int cScale = 1;

        public int cColor = 0;

        public float cFiringErrorMultiplier = 10f;

        #endregion

        #region Methods

        public void SetVariable<T>(string name, T value)
        {
            if (value is float single)
            {
                GetType().GetField(name).SetValue(this, Convert.ToInt32(single));
                return;
            }
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