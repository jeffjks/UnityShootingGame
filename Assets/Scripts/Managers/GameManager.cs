using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public int[] m_Resolution;
    [HideInInspector] public bool m_FullScreen;
    [HideInInspector] public int m_MusicVolume, m_SoundVolume;
    [HideInInspector] public int m_MaxLanguageOptions, m_Language;
    [HideInInspector] public int m_MaxResolutionNumber, m_GraphicsQuality, m_MaxGraphicsQuality;
    [HideInInspector] public bool m_AntiAliasing;
    [HideInInspector] public int[] m_CurrentAttributes = {0, 0, 0, 0, 0, 0, 0};
    [HideInInspector] public int m_UsedCost = 0;
    [HideInInspector] public byte m_Difficulty = 0;

    private int[,] m_ResolutionList;
    private PlayerManager m_PlayerManager = null;

    [SerializeField] private AudioMixer m_AudioMixer = null;

    public static GameManager instance_gm = null;

    void Awake()
    {
        if (instance_gm != null) {
            Destroy(this.gameObject);
            return;
        }
        instance_gm = this;
        
        DontDestroyOnLoad(gameObject);
        
        Application.targetFrameRate = 60;
        
        m_PlayerManager = PlayerManager.instance_pm;

        m_ResolutionList = new int[,] {{600, 900}, {1600, 900}, {720, 960}, {1280, 960}, {768, 1024}, {1280, 1024}, {810, 1080}, {1920, 1080}};
        m_MaxResolutionNumber = m_ResolutionList.GetLength(0);
        m_MaxGraphicsQuality = 6;
        m_MaxLanguageOptions = 2;

        DOTween.SetTweensCapacity(512, 64);

        SetOptions();
    }

    public void SetOptions() {
        m_Resolution = new int[2];

        if (!PlayerPrefs.HasKey("SoundVolume")) {
            PlayerPrefs.SetInt("ResolutionWidth", 1920);
            PlayerPrefs.SetInt("ResolutionHeight", 1080);
            PlayerPrefs.SetInt("FullScreen", 1);
            PlayerPrefs.SetInt("GraphicsQuality", 0);
            PlayerPrefs.SetInt("AntiAliasing", 1);
            PlayerPrefs.SetInt("Language", 1);
            PlayerPrefs.SetInt("MusicVolume", 80);
            PlayerPrefs.SetInt("SoundVolume", 80);
            PlayerPrefs.Save();
        }
        
        m_Resolution[0] = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        m_Resolution[1] = PlayerPrefs.GetInt("ResolutionHeight", 1080);

        if (PlayerPrefs.GetInt("FullScreen", 1) == 1)
            m_FullScreen = true;
        else
            m_FullScreen = false;

        m_GraphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 0);

        if (PlayerPrefs.GetInt("AntiAliasing", 1) == 1)
            m_AntiAliasing = true;
        else
            m_AntiAliasing = false;

        m_Language = PlayerPrefs.GetInt("Language", 1);
        
        m_MusicVolume = PlayerPrefs.GetInt("MusicVolume", 80);
        m_SoundVolume = PlayerPrefs.GetInt("SoundVolume", 80);

        Screen.SetResolution(m_Resolution[0], m_Resolution[1], m_FullScreen);
        SetMusicVolume(m_MusicVolume);
        SetSoundVolume(m_SoundVolume);
        SetGraphicsQuality(m_GraphicsQuality);
        SetAntiAliasing(m_AntiAliasing);
        SetLanguage(m_Language);
    }

    public int GetResolutionNumber() {
        for (int i=0; i<m_MaxResolutionNumber; i++) {
            if (m_Resolution[0] == m_ResolutionList[i,0] && m_Resolution[1] == m_ResolutionList[i,1])
                return i;
        }
        return 5;
    }

    public Resolution GetResolution(int number) {
        Resolution resolution = new Resolution();
        resolution.width = m_ResolutionList[number,0];
        resolution.height = m_ResolutionList[number,1];
        return resolution;
    }

    public string GetFullScreen(bool set) {
        if (m_Language == 0) {
            if (set) 
                return "FULL";
            else
                return "WINDOWED";
        }
        else if (m_Language == 1) {
            if (set) 
                return "전체 화면";
            else
                return "창 화면";
        }
        return null;
    }

    public void SetScreen(int number, bool set) {
        m_Resolution[0] = m_ResolutionList[number,0];
        m_Resolution[1] = m_ResolutionList[number,1];
        m_FullScreen = set;
        Screen.SetResolution(m_Resolution[0], m_Resolution[1], set);
        if (m_FullScreen)
            PlayerPrefs.SetInt("FullScreen", 1);
        else
            PlayerPrefs.SetInt("FullScreen", 0);
        PlayerPrefs.SetInt("ResolutionWidth", m_Resolution[0]);
        PlayerPrefs.SetInt("ResolutionHeight", m_Resolution[1]);
    }

    public void SetMusicVolume(int vol) {
        m_AudioMixer.SetFloat("Music", SetVolume(vol));
        m_MusicVolume = vol;
        PlayerPrefs.SetInt("MusicVolume", m_MusicVolume);
    }
    
    public void SetSoundVolume(int vol) {
        m_AudioMixer.SetFloat("SFX", SetVolume(vol));
        m_SoundVolume = vol;
        PlayerPrefs.SetInt("SoundVolume", m_SoundVolume);
    }

    private float SetVolume(int vol) { // vol = 0~100 -> 90
        float vol_f = (float) vol;
        float vol_result = Mathf.Log(vol_f/100f + 0.01f)*20 + 5;
        return vol_result;
    }

    public void SetLanguage(int language) {
        m_Language = language;
        PlayerPrefs.SetInt("Language", m_Language);
    }

    public string GetLanguage(int language) {
        if (m_Language == 0) {
            switch(language) {
                case 0:
                    return "ENGLISH";
                case 1:
                    return "한국어";
                default:
                    return "UNKNOWN";
            }
        }
        else if (m_Language == 1) {
            switch(language) {
                case 0:
                    return "ENGLISH";
                case 1:
                    return "한국어";
                default:
                    return "알 수 없음";
            }
        }
        return null;
    }

    public string GetGraphicsQuality(int quality) {
        if (m_Language == 0) {
            switch(quality) {
                case 0:
                    return "ULTRA";
                case 1:
                    return "VERY HIGH";
                case 2:
                    return "HIGH";
                case 3:
                    return "MEDIUM";
                case 4:
                    return "LOW";
                case 5:
                    return "VERY LOW";
                default:
                    return "UNKNOWN";
            }
        }
        else if (m_Language == 1) {
            switch(quality) {
                case 0:
                    return "울트라";
                case 1:
                    return "매우 높음";
                case 2:
                    return "높음";
                case 3:
                    return "중간";
                case 4:
                    return "낮음";
                case 5:
                    return "매우 낮음";
                default:
                    return "알 수 없음";
            }
        }
        return null;
    }

    public int GetGraphicsQualityNumber() {
        for (int i=0; i<6; i++) {
            if (i == QualitySettings.GetQualityLevel())
                return i;
        }
        return 6;
    }

    public void SetGraphicsQuality(int quality) {
        QualitySettings.SetQualityLevel(quality, true);
        m_GraphicsQuality = quality;
        PlayerPrefs.SetInt("GraphicsQuality", m_GraphicsQuality);
    }

    public string GetAntiAliasing(bool anti) {
        if (m_Language == 0) {
            if (anti)
                return "ON";
            else
                return "OFF";
        }
        else if (m_Language == 1) {
            if (anti)
                return "사용";
            else
                return "미사용";
        }
        return null;
    }

    public void SetAntiAliasing(bool state) {
        if (state)
            QualitySettings.antiAliasing = 2;
        else
            QualitySettings.antiAliasing = 0;
        m_AntiAliasing = state;
        PlayerPrefs.SetInt("AntiAliasing", QualitySettings.antiAliasing);
    }
}