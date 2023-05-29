using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;

public struct TrainingInfo
{
    public int stage;
    public bool bossOnly;
}

public class WaitForFrames : CustomYieldInstruction
{
    private int _targetFrameCount;

    public WaitForFrames(int numberOfFrames)
    {
        _targetFrameCount = Time.frameCount + numberOfFrames;
    }

    public override bool keepWaiting
    {
        get
        {
            if (Time.timeScale == 0) {
                _targetFrameCount++;
            }
            return Time.frameCount < _targetFrameCount;
        }
    }
}

public class WaitForMillisecondFrames : CustomYieldInstruction
{
    private int _targetFrameCount;

    public WaitForMillisecondFrames(int millisecond)
    {
        int numberOfFrames = millisecond * Application.targetFrameRate / 1000;
        _targetFrameCount = Time.frameCount + numberOfFrames;
    }

    public override bool keepWaiting
    {
        get
        {
            if (Time.timeScale == 0) {
                _targetFrameCount++;
            }
            return Time.frameCount < _targetFrameCount;
        }
    }
}

public class AC_Ease
{
    public static AnimationCurve[] ac_ease = new AnimationCurve[4];
}

[Serializable]
public enum AttributeType
{
    Color,
    Speed,
    ShotLevel,
    LaserLevel,
    Module,
    Bomb
}

[Serializable]
public class ShipAttributes
{
    private Dictionary<AttributeType, int> _attributes = new();

    public ShipAttributes() : this(0, 0, 0, 0, 0, 0, 0)
    {
    }
    
    public ShipAttributes(int color, int speed, int shot_form, int shotLevel, int laserLevel, int module, int bomb)
    {
        _attributes[AttributeType.Color] = color;
        _attributes[AttributeType.Speed] = speed;
        _attributes[AttributeType.ShotLevel] = shotLevel;
        _attributes[AttributeType.LaserLevel] = laserLevel;
        _attributes[AttributeType.Module] = module;
        _attributes[AttributeType.Bomb] = bomb;
    }

    public ShipAttributes(string jsonCode)
    {
        _attributes = JsonConvert.DeserializeObject<Dictionary<AttributeType, int>>(jsonCode);
    }

    public void SetAttributes(AttributeType key, int value)
    {
        _attributes[key] = value;
    }

    public int GetAttributes(AttributeType key)
    {
        return _attributes[key];
    }

    public string GetAttributesCode() {
        return JsonConvert.SerializeObject(_attributes, Formatting.None);
    }
    
    public static bool operator ==(ShipAttributes op1, ShipAttributes op2)
    {
        if (op1 == null || op2 == null)
        {
            Debug.LogWarning("Compared null shipAttributes and returned false.");
            return false;
        }
        var dic1 = op1._attributes;
        var dic2 = op2._attributes;
        return dic1.Equals(dic2);
    }

    public static bool operator !=(ShipAttributes op1, ShipAttributes op2) {
        return !(op1 == op2);
    }

    public override bool Equals(object op)
    {
        return (this == ((ShipAttributes) op));
    }
    
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class GameManager : MonoBehaviour
{
    //[HideInInspector] public ShipAttribute m_CurrentAttributes;
    [HideInInspector] public byte m_ReplayNum;
    [HideInInspector] public string m_ReplayDirectory;
    [HideInInspector] public string m_RankingDirectory;
    [HideInInspector] public bool m_IsOnline = false;

    public bool m_NetworkAvailable;
    public bool m_InvincibleMod;

    public PlayerManager m_PlayerManager;
    public SystemManager m_SystemManager;

    private string m_AccountID = string.Empty, m_EncryptedAccountID;

    public AnimationCurve[] m_AnimationCurve = new AnimationCurve[3];

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
        
        //m_CurrentAttributes = new ShipAttribute(0, 0, 0, 0, 0, 0, 0);
        DontDestroyOnLoad(m_PlayerManager.gameObject);
        
        m_ReplayDirectory = "";
        m_RankingDirectory = "";

        for (int i = 0; i < m_AnimationCurve.Length; ++i) {
            AC_Ease.ac_ease[i] = m_AnimationCurve[i];
        }

        DOTween.SetTweensCapacity(512, 64);
    }

    void Update()
    {
        if (!m_NetworkAvailable) {
            return;
        }
        if (m_IsOnline) {
            if (Md5Sum(m_AccountID) != m_EncryptedAccountID) {
                m_IsOnline = false;
                Debug.LogAssertion("ID Falsification Detected.");
                Application.Quit(); // 에디터에서는 무시됨
            }
        }
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