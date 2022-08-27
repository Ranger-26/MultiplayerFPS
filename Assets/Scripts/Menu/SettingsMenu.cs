using UnityEngine;
using UnityEngine.Audio;

namespace Menu
{
    public class SettingsMenu : MonoBehaviour
    {
        public static SettingsMenu Instance;

        public AudioMixer audioMixer;

        private void Awake()
        {
            #region Singleton

            if (Instance != null) Destroy(gameObject);
            else Instance = this;

            #endregion

            Settings.LoadSettings();
        }
    }
}