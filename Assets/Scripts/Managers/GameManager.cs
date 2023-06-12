using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;


public class GameManager : MonoBehaviour
{
    public GameSetting m_GameSetting;
    public NetworkAccount m_NetworkAccount;
    
    //[HideInInspector] public ShipAttribute m_CurrentAttributes;
    [HideInInspector] public byte m_ReplayNum;
    [HideInInspector] public string m_ReplayDirectory;
    [HideInInspector] public string m_RankingDirectory;
    [HideInInspector] public bool m_IsOnline = false;

    public bool m_NetworkAvailable;
    public bool m_InvincibleMod;

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
        
        m_ReplayDirectory = "";
        m_RankingDirectory = "";

        for (int i = 0; i < m_AnimationCurve.Length; ++i) {
            AC_Ease.ac_ease[i] = m_AnimationCurve[i];
        }

        DOTween.SetTweensCapacity(512, 64);
    }

    private void Start()
    {
        m_GameSetting.LoadSettings();
        m_NetworkAccount.Init();
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