using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Menu
{
    public class SettingCollection : MonoBehaviour
    {
        static HDRenderPipelineAsset hdrp;

        [SerializeField]
        ResolutionSetting resolutionSetting;

        private void Awake()
        {
            var rp = QualitySettings.renderPipeline;
            hdrp = (HDRenderPipelineAsset)rp;
        }

        private void Start()
        {
            resolutionSetting?.ChangeRes(Settings.Current.DisplayRes);
            Fullscreen(Settings.Current.Fullscreen);
            VSync(Settings.Current.VSync);
            SetVolumeEffects(Settings.Current.EffectsVolume);
            SetVolumeMusic(Settings.Current.MusicVolume);
            SetVolumeMaster(Settings.Current.MasterVolume);
            TextureQuality(Settings.Current.TextureQuality);
            AntiAliasing(Settings.Current.AntiAliasing);
            FrameLimit(Settings.Current.FrameLimit);
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

        public void AntiAliasing(int value)
        {
            switch (value)
            {
                case 0:
                    QualitySettings.antiAliasing = 0;
                    break;
                case 1:
                    QualitySettings.antiAliasing = 2;
                    break;
                case 2:
                    QualitySettings.antiAliasing = 4;
                    break;
                case 3:
                    QualitySettings.antiAliasing = 8;
                    break;
            }
        }

        public void FrameLimit(float value)
        {
            Application.targetFrameRate = Convert.ToInt32(value);
        }
    }
}