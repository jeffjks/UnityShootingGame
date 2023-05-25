using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSetting : MonoBehaviour
{
    public static GraphicsSettings m_GraphicsSettings;
    public static SoundSettings m_SoundSettings;
    
    public static Language m_Language;

    public AudioMixer m_AudioMixer = null;

    private static readonly List<Vector2Int> _graphicsResolutionList = new List<Vector2Int>()
    {
        new(680, 900),
        new(1600, 900),
        new(720, 960),
        new(1280, 960),
        new(768, 1024),
        new(1280, 1024),
        new(810, 1080),
        new(1920, 1080),
        new(3840, 2160)
    };

    public const int MAX_VOLUME = 100;
    public const int RESOLUTION_NUMBER = 9;
    
    private static GameSetting Instance { get; set; }
    
    private void Start()
    {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        LoadSettings();
    }

    private void LoadSettings() {
        if (!PlayerPrefs.HasKey("SoundEffectVolume")) {
            PlayerPrefs.SetInt("ResolutionWidth", 1920);
            PlayerPrefs.SetInt("ResolutionHeight", 1080);
            PlayerPrefs.SetInt("FullScreen", 1);
            PlayerPrefs.SetInt("GraphicsQuality", 0);
            PlayerPrefs.SetInt("AntiAliasing", 1);
            PlayerPrefs.SetInt("Language", 1);
            PlayerPrefs.SetInt("MusicVolume", 80);
            PlayerPrefs.SetInt("SoundEffectVolume", 80);
            PlayerPrefs.Save();
        }
        
        var width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        var height = PlayerPrefs.GetInt("ResolutionHeight", 1080);

        m_GraphicsSettings.GraphicsResolution = GetResolutionIndex(width, height);
        m_GraphicsSettings.GraphicsScreenMode = (GraphicsScreenMode) PlayerPrefs.GetInt("FullScreen", 1);
        m_GraphicsSettings.GraphicsQuality = (GraphicsQuality) PlayerPrefs.GetInt("GraphicsQuality", 0);
        m_GraphicsSettings.GraphicsAntiAliasing = (GraphicsAntiAliasing) PlayerPrefs.GetInt("AntiAliasing", 1);
        
        m_SoundSettings.MusicVolume = PlayerPrefs.GetInt("MusicVolume", 80);
        m_SoundSettings.SoundEffectVolume = PlayerPrefs.GetInt("SoundEffectVolume", 80);

        SaveGraphicSettings();
        SaveSoundSettings();

        m_Language = (Language) PlayerPrefs.GetInt("Language", 1);
        
        SaveLanguageSetting();
    }

    private int GetResolutionIndex(int width, int height)
    {
        for (int i = 0; i < _graphicsResolutionList.Count; ++i)
        {
            if (_graphicsResolutionList[i] == new Vector2Int(width, height))
            {
                return i;
            }
        }

        return -1;
    }

    public static Resolution GetCurrentResolution()
    {
        var index = m_GraphicsSettings.GraphicsResolution;
        
        Resolution resolution = new Resolution();
        resolution.width = _graphicsResolutionList[index].x;
        resolution.height = _graphicsResolutionList[index].y;
        return resolution;
    }

    public static void SaveGraphicSettings()
    {
        SaveScreenSettings();
        SaveQuality();
        SaveAntiAliasing();
    }

    private static void SaveScreenSettings()
    {
        var index = m_GraphicsSettings.GraphicsResolution;
        
        var width = _graphicsResolutionList[index].x;
        var height = _graphicsResolutionList[index].y;
        Screen.SetResolution(width, height, m_GraphicsSettings.GraphicsScreenMode == GraphicsScreenMode.FullScreen);
        
        PlayerPrefs.SetInt("FullScreen", m_GraphicsSettings.GraphicsScreenMode == GraphicsScreenMode.FullScreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
    }

    private static void SaveQuality()
    {
        var quality = m_GraphicsSettings.GraphicsQuality;
        
        QualitySettings.SetQualityLevel((int) quality, true);
        
        PlayerPrefs.SetInt("GraphicsQuality", (int) quality);
    }

    private static void SaveAntiAliasing() {
        var antiAliasing = m_GraphicsSettings.GraphicsAntiAliasing;
        
        if (antiAliasing == GraphicsAntiAliasing.Deactivated)
            QualitySettings.antiAliasing = 0;
        else
            QualitySettings.antiAliasing = 2;
        
        PlayerPrefs.SetInt("AntiAliasing", (int) antiAliasing);
    }

    public static void SaveSoundSettings()
    {
        SaveMusicVolume();
        SaveSoundEffectVolume();
    }
    
    public static void SaveMusicVolume()
    {
        var volume = m_SoundSettings.MusicVolume;
        Instance.m_AudioMixer.SetFloat("Music", Instance.GetMixerVolume(volume));
        PlayerPrefs.SetInt("MusicVolume", volume);
    }
    
    public static void SaveSoundEffectVolume() {
        var volume = m_SoundSettings.SoundEffectVolume;
        Instance.m_AudioMixer.SetFloat("SFX", Instance.GetMixerVolume(volume));
        PlayerPrefs.SetInt("SoundEffectVolume", volume);
    }

    private float GetMixerVolume(int volume) { // vol = 0~100 -> 90
        var mixerVolume = Mathf.Log(volume/100f + 0.01f)*20 + 5;
        return mixerVolume;
    }

    public static void SaveLanguageSetting()
    {
        var language = m_Language;
        PlayerPrefs.SetInt("Language", (int) language);
    }
}
