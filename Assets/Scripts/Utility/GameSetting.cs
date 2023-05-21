using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSetting : MonoBehaviour
{
    public static readonly Dictionary<GraphicsOption, int> m_GraphicOptions = new();
    public static readonly Dictionary<GraphicsOption, int> m_GraphicOptionsCount = new();
    public static readonly Dictionary<SoundOption, int> m_SoundOptions = new();
    
    public static Language m_Language;

    public AudioMixer m_AudioMixer = null;

    public const int MAX_VOLUME = 100;

    private static List<Vector2Int> _resolutionList = new List<Vector2Int>()
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
    
    private static GameSetting Instance { get; set; }
    
    private void Start()
    {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        m_GraphicOptionsCount[GraphicsOption.Resolution] = _resolutionList.Count;
        m_GraphicOptionsCount[GraphicsOption.FullScreen] = 2;
        m_GraphicOptionsCount[GraphicsOption.Quality] = 6;
        m_GraphicOptionsCount[GraphicsOption.AntiAliasing] = 2;
        
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

        m_GraphicOptions[GraphicsOption.Resolution] = GetResolutionIndex(width, height);
        m_GraphicOptions[GraphicsOption.FullScreen] = PlayerPrefs.GetInt("FullScreen", 1);
        m_GraphicOptions[GraphicsOption.Quality] = PlayerPrefs.GetInt("GraphicsQuality", 0);
        m_GraphicOptions[GraphicsOption.AntiAliasing] = PlayerPrefs.GetInt("AntiAliasing", 1);
        
        m_SoundOptions[SoundOption.MusicVolume] = PlayerPrefs.GetInt("MusicVolume", 80);
        m_SoundOptions[SoundOption.SoundEffectVolume] = PlayerPrefs.GetInt("SoundEffectVolume", 80);

        SaveGraphicSettings();
        SaveSoundSettings();

        m_Language = (Language) PlayerPrefs.GetInt("Language", 1);
        
        SaveLanguage();
    }

    private int GetResolutionIndex(int width, int height)
    {
        for (int i = 0; i < _resolutionList.Count; ++i)
        {
            if (_resolutionList[i] == new Vector2Int(width, height))
            {
                return i;
            }
        }

        return -1;
    }

    public static Resolution GetCurrentResolution()
    {
        var index = m_GraphicOptions[GraphicsOption.Resolution];
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

    private static void SaveScreenSettings()
    {
        var index = m_GraphicOptions[GraphicsOption.Resolution];
        
        var width = _resolutionList[index].x;
        var height = _resolutionList[index].y;
        Screen.SetResolution(width, height, m_GraphicOptions[GraphicsOption.FullScreen] == 1);
        
        PlayerPrefs.SetInt("FullScreen", m_GraphicOptions[GraphicsOption.FullScreen]);
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
    }

    private static void SaveQuality()
    {
        var quality = m_GraphicOptions[GraphicsOption.Quality];
        
        QualitySettings.SetQualityLevel(quality, true);
        
        PlayerPrefs.SetInt("GraphicsQuality", quality);
    }

    private static void SaveAntiAliasing() {
        var antiAliasing = m_GraphicOptions[GraphicsOption.AntiAliasing];
        
        if (antiAliasing == 0)
            QualitySettings.antiAliasing = 0;
        else
            QualitySettings.antiAliasing = 2;
        
        PlayerPrefs.SetInt("AntiAliasing", antiAliasing);
    }

    public static void SaveSoundSettings()
    {
        SaveMusicVolume();
        SaveSoundEffectVolume();
    }
    
    public static void SaveMusicVolume()
    {
        var volume = m_SoundOptions[SoundOption.MusicVolume];
        Instance.m_AudioMixer.SetFloat("Music", Instance.GetMixerVolume(volume));
        PlayerPrefs.SetInt("MusicVolume", volume);
    }
    
    public static void SaveSoundEffectVolume() {
        var volume = m_SoundOptions[SoundOption.SoundEffectVolume];
        Instance.m_AudioMixer.SetFloat("SFX", Instance.GetMixerVolume(volume));
        PlayerPrefs.SetInt("SoundEffectVolume", volume);
    }

    private float GetMixerVolume(int volume) { // vol = 0~100 -> 90
        float volume_f = volume;
        float volume_result = Mathf.Log(volume_f/100f + 0.01f)*20 + 5;
        return volume_result;
    }

    public void SaveLanguage()
    {
        var language = m_Language;
        PlayerPrefs.SetInt("Language", (int) language);
    }
}
