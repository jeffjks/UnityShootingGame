using System;
using System.Collections.Generic;
using DG.Tweening;
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

    public AudioMixer m_AudioMixer;

    [SerializeField] private GraphicOptionSettingData m_GraphicOptionSettingData;

    private static Vector2Int[] _resolutionList;

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
        
        Application.targetFrameRate = 60;
        Cursor.visible = (DebugOption.SceneMode != 0);
        DOTween.SetTweensCapacity(512, 64);
        _resolutionList = m_GraphicOptionSettingData.GraphicResolutionList;
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadSettings(); // Have to call in Start method!
    }

    private void LoadSettings()
    {
        if (!PlayerPrefs.HasKey("SoundEffectVolume")) {
            PlayerPrefs.SetInt("ResolutionWidth", _resolutionList[2].x);
            PlayerPrefs.SetInt("ResolutionHeight", _resolutionList[2].y);
            PlayerPrefs.SetInt("FullScreen", 1);
            PlayerPrefs.SetInt("GraphicsQuality", (int) QualitySetting.Ultra);
            PlayerPrefs.SetInt("AntiAliasing", 1);
            PlayerPrefs.SetInt("Language", 1);
            PlayerPrefs.SetInt("MusicVolume", 80);
            PlayerPrefs.SetInt("SoundEffectVolume", 80);
            PlayerPrefs.Save();
        }
        
        var width = PlayerPrefs.GetInt("ResolutionWidth", _resolutionList[2].x);
        var height = PlayerPrefs.GetInt("ResolutionHeight", _resolutionList[2].y);

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
        for (int i = 0; i < _resolutionList.Length; ++i)
        {
            if (_resolutionList[i] == new Vector2Int(width, height))
            {
                return i;
            }
        }

        return 0;
    }

    public static Resolution GetCurrentResolution()
    {
        var index = GraphicsResolution;
        
        Resolution resolution = new Resolution();
        resolution.width = _resolutionList[index].x;
        resolution.height = _resolutionList[index].y;
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
        SetQualitySettings();
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
        var width = _resolutionList[index].x;
        var height = _resolutionList[index].y;
        Screen.SetResolution(width, height, GraphicsScreenMode == ScreenModeSetting.FullScreen);
        Debug.Log($"[Screen Settings] Resolution: {width}x{height}, FullScreen: {Screen.fullScreen}");
    }

    private static void SaveScreenSettings()
    {
        var index = GraphicsResolution;
        var width = _resolutionList[index].x;
        var height = _resolutionList[index].y;
        
        PlayerPrefs.SetInt("FullScreen", GraphicsScreenMode == ScreenModeSetting.FullScreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
    }

    private static void SetQualitySettings()
    {
        var quality = GraphicsQuality;
        QualitySettings.SetQualityLevel((int) quality, true);
        
        var antiAliasing = GraphicsAntiAliasing;
        QualitySettings.antiAliasing = (antiAliasing == AntiAliasingSetting.Deactivated) ? 0 : 2;
        Debug.Log($"[Quality Settings] Quality: {QualitySettings.names[(int)quality]}, AntiAliasing: {QualitySettings.antiAliasing}");
    }

    private static void SaveQuality()
    {
        var quality = GraphicsQuality;
        PlayerPrefs.SetInt("GraphicsQuality", (int) quality);
    }
    
    private static void SaveAntiAliasing() {
        var antiAliasing = GraphicsAntiAliasing;
        PlayerPrefs.SetInt("AntiAliasing", (int) antiAliasing);
    }
    
    public static void SetMusicVolume()
    {
        var volume = MusicVolume;
        Instance.m_AudioMixer.SetFloat("Music", Instance.GetMixerVolume(volume));
        Debug.Log($"[Music Volume] Volume: {volume}");
    }
    
    public static void SaveMusicVolume()
    {
        var volume = MusicVolume;
        PlayerPrefs.SetInt("MusicVolume", volume);
    }
    
    public static void SetSoundEffectVolume() {
        var volume = SoundEffectVolume;
        Instance.m_AudioMixer.SetFloat("SFX", Instance.GetMixerVolume(volume));
        Debug.Log($"[SFX Volume] Volume: {volume}");
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
        Debug.Log($"[LanguageSetting] Language: {language}");
    }
}