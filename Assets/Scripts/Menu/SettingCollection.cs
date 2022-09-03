using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Menu
{
    public class SettingCollection : MonoBehaviour
    {
        static HDRenderPipelineAsset hdrp;

        private void Awake()
        {
            var rp = QualitySettings.renderPipeline;
            hdrp = (HDRenderPipelineAsset)rp;
        }

        private void Start()
        {
            Fullscreen(Settings.Current.Fullscreen);
            VSync(Settings.Current.VSync);
            SetVolumeEffects(Settings.Current.EffectsVolume);
            SetVolumeMusic(Settings.Current.MusicVolume);
            SetVolumeMaster(Settings.Current.MasterVolume);
            TextureQuality(Settings.Current.TextureQuality);
        }

        public void SetVolumeMaster(float value)
        {
            SettingsMenu.Instance.audioMixer.SetFloat("master_volume", value);
        }

        public void SetVolumeMusic(float value)
        {
            SettingsMenu.Instance.audioMixer.SetFloat("music_volume", value);
        }

        public void SetVolumeEffects(float value)
        {
            SettingsMenu.Instance.audioMixer.SetFloat("effects_volume", value);
        }

        public void Fullscreen(bool value)
        {
            Screen.fullScreen = value;
        }

        public void VSync(bool value)
        {
            QualitySettings.vSyncCount = value ? 2 : 0;
        }

        public void TextureQuality(int value)
        {
            QualitySettings.masterTextureLimit = value;
        }
    }
}