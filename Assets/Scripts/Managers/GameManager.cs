using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public struct TrainingInfo
{
    public int m_Stage;
    public bool m_BossOnly;
    public GameDifficulty m_Difficulty;
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

[System.Serializable]
public class ShipAttributes
{
    public int m_Color, m_Speed, m_ShotDamage, m_LaserDamage, m_Module, m_Bomb;
    
    public ShipAttributes(int color, int speed, int shot_form, int shot_damage, int laser_damage, int module, int bomb) {
        m_Color = color;
        m_Speed = speed;
        m_ShotDamage = shot_damage;
        m_LaserDamage = laser_damage;
        m_Module = module;
        m_Bomb = bomb;
    }

    public ShipAttributes(int code) {
        int[] nums = { 0,0,0,0,0,0 };
        int i = 0;
        while (i < 6) {
            int num = code % 10;
            nums[i] = num;
            code /= 10;
            i++;
        }
        m_Bomb = nums[0];
        m_Module = nums[1];
        m_LaserDamage = nums[2];
        m_ShotDamage = nums[3];
        m_Speed = nums[4];
        m_Color = nums[5];
    }

    public void SetAttributes(int num, int value) {
        switch (num) {
            case 0:
                m_Color = value;
                break;
            case 1:
                m_Speed = value;
                break;
            case 2:
                m_ShotDamage = value;
                break;
            case 3:
                m_LaserDamage = value;
                break;
            case 4:
                m_Module = value;
                break;
            case 5:
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
                return m_ShotDamage;
            case 3:
                return m_LaserDamage;
            case 4:
                return m_Module;
            case 5:
                return m_Bomb;
            default:
                return -1;
        }
    }

    public int GetAttributesCode() { // Color Speed, ShotForm, Shot, Laser, Module, Bomb
        int code = 0;
        code += 100000*m_Color;
        code += 10000*m_Speed;
        code += 1000*m_ShotDamage;
        code += 100*m_LaserDamage;
        code += 10*m_Module;
        code += 1*m_Bomb;
        return code;
    }
    
    public static bool operator ==(ShipAttributes op1, ShipAttributes op2) {
        if (op1.m_Speed != op2.m_Speed)
            return false;
        if (op1.m_ShotDamage != op2.m_ShotDamage)
            return false;
        if (op1.m_LaserDamage != op2.m_LaserDamage)
            return false;
        if (op1.m_Module != op2.m_Module)
            return false;
        if (op1.m_Bomb != op2.m_Bomb)
            return false;
        return true;
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