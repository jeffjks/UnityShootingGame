using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSetting : MonoBehaviour
{
    public static int GraphicsResolution;
    public static ScreenModeSetting GraphicsScreenMode;
    public static QualitySetting GraphicsQuality;
    public static AntiAliasingSetting GraphicsAntiAliasing;
    
    public static int MusicVolume;
    public static int SoundEffectVolume;
    
    public static Language CurrentLanguage;

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
    public const int RESOLUTION_SETTING_COUNT = 9;
    
    private static GameSetting Instance { get; set; }
    
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSettings() {
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

        GraphicsResolution = GetResolutionIndex(width, height);
        GraphicsScreenMode = (ScreenModeSetting) PlayerPrefs.GetInt("FullScreen", 1);
        GraphicsQuality = (QualitySetting) PlayerPrefs.GetInt("GraphicsQuality", 0);
        GraphicsAntiAliasing = (AntiAliasingSetting) PlayerPrefs.GetInt("AntiAliasing", 1);
        
        MusicVolume = PlayerPrefs.GetInt("MusicVolume", 80);
        SoundEffectVolume = PlayerPrefs.GetInt("SoundEffectVolume", 80);

        CurrentLanguage = (Language) PlayerPrefs.GetInt("Language", 1);

        SetGraphicSettings();
        SetSoundSettings();
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
        var index = GraphicsResolution;
        
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

    public static void SetGraphicSettings()
    {
        SetScreenSettings();
        SetQuality();
        SetAntiAliasing();
    }

    public static void SaveSoundSettings()
    {
        SaveMusicVolume();
        SaveSoundEffectVolume();
    }

    public static void SetSoundSettings()
    {
        SetMusicVolume();
        SetSoundEffectVolume();
    }

    private static void SetScreenSettings()
    {
        var index = GraphicsResolution;
        var width = _graphicsResolutionList[index].x;
        var height = _graphicsResolutionList[index].y;
        Screen.SetResolution(width, height, GraphicsScreenMode == ScreenModeSetting.FullScreen);
    }

    private static void SaveScreenSettings()
    {
        var index = GraphicsResolution;
        var width = _graphicsResolutionList[index].x;
        var height = _graphicsResolutionList[index].y;
        
        PlayerPrefs.SetInt("FullScreen", GraphicsScreenMode == ScreenModeSetting.FullScreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
    }

    private static void SetQuality()
    {
        var quality = GraphicsQuality;
        QualitySettings.SetQualityLevel((int) quality, true);
    }

    private static void SaveQuality()
    {
        var quality = GraphicsQuality;
        PlayerPrefs.SetInt("GraphicsQuality", (int) quality);
    }

    private static void SetAntiAliasing() {
        var antiAliasing = GraphicsAntiAliasing;
        
        if (antiAliasing == AntiAliasingSetting.Deactivated)
            QualitySettings.antiAliasing = 0;
        else
            QualitySettings.antiAliasing = 2;
    }

    private static void SaveAntiAliasing() {
        var antiAliasing = GraphicsAntiAliasing;
        PlayerPrefs.SetInt("AntiAliasing", (int) antiAliasing);
    }
    
    public static void SetMusicVolume()
    {
        var volume = MusicVolume;
        Instance.m_AudioMixer.SetFloat("Music", Instance.GetMixerVolume(volume));
    }
    
    public static void SaveMusicVolume()
    {
        var volume = MusicVolume;
        PlayerPrefs.SetInt("MusicVolume", volume);
    }
    
    public static void SetSoundEffectVolume() {
        var volume = SoundEffectVolume;
        Instance.m_AudioMixer.SetFloat("SFX", Instance.GetMixerVolume(volume));
    }
    
    public static void SaveSoundEffectVolume() {
        var volume = SoundEffectVolume;
        PlayerPrefs.SetInt("SoundEffectVolume", volume);
    }

    private float GetMixerVolume(int volume) { // vol = 0~100 -> 90
        var mixerVolume = Mathf.Log(volume/100f + 0.01f)*20 + 5;
        return mixerVolume;
    }

    public static void SaveLanguageSetting()
    {
        var language = CurrentLanguage;
        PlayerPrefs.SetInt("Language", (int) language);
    }
}
