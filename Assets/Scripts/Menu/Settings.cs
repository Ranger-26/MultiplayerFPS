using UnityEngine;

namespace Menu
{
    using System;

    public static class Settings
    {
        public static Setting Current;

        public static void SaveSettings()
        {
            ES3.Save("Settings", Current, Application.persistentDataPath + "/settings.scp");
        }

        public static Setting LoadSettings()
        {
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

        public int DisplayNumber = 0;
        public int DisplayRes = 0;

        public bool Fullscreen = true;
        public bool VSync = false;

        // Graphics

        public int TextureQuality = 2;
        public int AntiAliasing = 2;

        #endregion

        #region Audio

        public float MasterVolume = 0f;
        public float MusicVolume = 0f;
        public float EffectsVolume = 0f;

        #endregion

        #region Gameplay

        public float Sensitivity = 1f;

        // Crosshair

        public float cOffset = 2f;
        public float cLength = 4f;
        public float cThickness = 2f;
        public float cDotSize = 2f;
        public float cScale = 1f;

        public int cColor = 0;

        public float cFiringErrorMultiplier = 10f;

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