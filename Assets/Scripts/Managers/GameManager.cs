using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public NetworkAccount m_NetworkAccount;
    
    //[HideInInspector] public ShipAttribute m_CurrentAttributes;

    private static string m_AccountID = string.Empty;
    private static string m_EncryptedAccountID;
    
    public List<AnimationCurve> m_AnimationCurves;

    public static bool isOnline;
    public static bool IsDebugScene;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        //m_CurrentAttributes = new ShipAttribute(0, 0, 0, 0, 0, 0, 0);

        foreach (var animationCurve in m_AnimationCurves)
        {
            AC_Ease.ac_ease.Add(animationCurve);
        }
    }

    private void Start()
    {
        #if UNITY_EDITOR
        if (DebugOption.GenerateJsonFile)
        {
            GetComponent<ExplosionJsonWriter>().GenerateJsonFile();
            GetComponent<EndingCreditJsonWriter>().GenerateJsonFile();
            GetComponent<RankingJsonWriter>().GenerateJsonFile();
        }
        #endif

        if (!TestIntegrityAll())
        {
            Utility.QuitGame();
        }
        ValidateAccount();
        m_NetworkAccount.Init();
    }

    private void ValidateAccount()
    {
        if (!DebugOption.NetworkAvailable) {
            return;
        }
        if (isOnline) {
            if (Utility.Md5Sum(m_AccountID) != m_EncryptedAccountID) {
                isOnline = false;
                Debug.LogAssertion("ID Falsification Detected.");
                Utility.QuitGame();
            }
        }
    }

    private void Update()
    {
    }

    public static string GetAccountID() {
        return m_AccountID;
    }

    public static void SetAccountID(string id) {
        m_AccountID = id;
        isOnline = true;
        m_EncryptedAccountID = Utility.Md5Sum(m_AccountID);
    }

    private static bool TestIntegrityAll()
    {
        try
        {
            List<string> fileList = new () { "resources1", "resources2", "ranking0", "ranking1", "ranking2" };
            string filePath = Application.dataPath;

            foreach (var fileName in fileList)
            {
                if (!File.Exists($"{filePath}/{fileName}.dat"))
                {
                    continue;
                }
                var (jsonData, hash) = Utility.LoadDataFileString(filePath, fileName);
                
                if (jsonData != null && Utility.Md5Sum(jsonData) != hash)
                {
                    Debug.LogError("무결성 검사에 실패하였습니다.");
                    return false;
                }
            }
            Debug.Log("무결성 검사가 완료되었습니다.");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"무결성 검사 중 오류가 발생하였습니다:\n{e}");
        }
        return false;
    }
}