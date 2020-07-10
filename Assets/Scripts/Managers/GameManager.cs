using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using DG.Tweening;

public struct TrainingInfo
{
    public int m_Stage;
    public bool m_BossOnly;
    public byte m_Difficulty;
}

[System.Serializable]
public class Attributes {
    public int m_Color, m_Speed, m_ShotForm, m_ShotDamage, m_LaserDamage, m_Module, m_Bomb;
    
    public Attributes(int color, int speed, int shot_form, int shot_damage, int laser_damage, int module, int bomb) {
        m_Color = color;
        m_Speed = speed;
        m_ShotForm = shot_form;
        m_ShotDamage = shot_damage;
        m_LaserDamage = laser_damage;
        m_Module = module;
        m_Bomb = bomb;
    }

    public void SetAttributes(byte num, int value) {
        switch (num) {
            case 0:
                m_Color = value;
                break;
            case 1:
                m_Speed = value;
                break;
            case 2:
                m_ShotForm = value;
                break;
            case 3:
                m_ShotDamage = value;
                break;
            case 4:
                m_LaserDamage = value;
                break;
            case 5:
                m_Module = value;
                break;
            case 6:
                m_Bomb = value;
                break;
            default:
                break;
        }
    }

    public int GetAttributes(byte num) {
        switch (num) {
            case 0:
                return m_Color;
            case 1:
                return m_Speed;
            case 2:
                return m_ShotForm;
            case 3:
                return m_ShotDamage;
            case 4:
                return m_LaserDamage;
            case 5:
                return m_Module;
            case 6:
                return m_Bomb;
            default:
                return -1;
        }
    }

    public int GetAttributesCode() { // Color Speed, ShotForm, Shot, Laser, Module, Bomb
        int code = 0;
        code += 1000000*m_Color;
        code += 100000*m_Speed;
        code += 10000*m_ShotForm;
        code += 1000*m_ShotDamage;
        code += 100*m_LaserDamage;
        code += 10*m_Module;
        code += 1*m_Bomb;
        return code;
    }
}

public class GameManager : MonoBehaviour
{
    [HideInInspector] public int[] m_Resolution;
    [HideInInspector] public bool m_FullScreen;
    [HideInInspector] public int m_MusicVolume, m_SoundVolume;
    [HideInInspector] public int m_MaxLanguageOptions, m_Language;
    [HideInInspector] public int m_MaxResolutionNumber, m_GraphicsQuality, m_MaxGraphicsQuality;
    [HideInInspector] public bool m_AntiAliasing;
    [HideInInspector] public Attributes m_CurrentAttributes;
    [HideInInspector] public int m_UsedCost;
    [HideInInspector] public byte m_Difficulty;
    [HideInInspector] public bool m_ReplayState;
    [HideInInspector] public byte m_ReplayNum;
    [HideInInspector] public string m_ReplayDirectory;
    [HideInInspector] public bool m_TrainingState;
    [HideInInspector] public TrainingInfo m_TrainingInfo;
    [HideInInspector] public bool m_IsOnline = false;

    private string m_AccountID = string.Empty, m_EncryptedAccountID;
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
        Cursor.visible = false;
        
        m_PlayerManager = PlayerManager.instance_pm;
        m_CurrentAttributes = new Attributes(0, 0, 0, 0, 0, 0, 0);

        m_ResolutionList = new int[,] {{600, 900}, {1600, 900}, {720, 960}, {1280, 960}, {768, 1024}, {1280, 1024}, {810, 1080}, {1920, 1080}};
        m_MaxResolutionNumber = m_ResolutionList.GetLength(0);
        m_MaxGraphicsQuality = 6;
        m_MaxLanguageOptions = 2;
        m_ReplayDirectory = "";

        DOTween.SetTweensCapacity(512, 64);

        SetOptions();
    }

    void Update()
    {
        if (Md5Sum(m_AccountID) != m_EncryptedAccountID) {
            m_IsOnline = false;
            Application.Quit(); // 에디터에서는 무시됨
        }
    }

    public void SetOptions() {
        m_Resolution = new int[2];

        if (!PlayerPrefs.HasKey("SoundVolume")) {
            PlayerPrefs.SetInt("ResolutionWidth", 1920);
            PlayerPrefs.SetInt("ResolutionHeight", 1080);
            PlayerPrefs.SetInt("FullScreen", 1);
            PlayerPrefs.SetInt("GraphicsQuality", 0);
            PlayerPrefs.SetInt("AntiAliasing", 2);
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

        if (PlayerPrefs.GetInt("AntiAliasing", 2) == 2)
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
        for (int i = 0; i < m_MaxResolutionNumber; i++) {
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
        for (int i = 0; i < 6; i++) {
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

    public string GetAccountID() {
        return m_AccountID;
    }

    public void SetAccountID(string id) {
        m_AccountID = id;
        m_IsOnline = true;
        m_EncryptedAccountID = Md5Sum(m_AccountID);
    }

    private string Md5Sum(string strToEncrypt) {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
    
        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
    
        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";
    
        for (int i = 0; i < hashBytes.Length; i++) {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
    
        return hashString.PadLeft(32, '0');
    }
}