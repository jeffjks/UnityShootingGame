using System;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameSetting m_GameSetting;
    public NetworkAccount m_NetworkAccount;
    
    //[HideInInspector] public ShipAttribute m_CurrentAttributes;
    [HideInInspector] public byte m_ReplayNum;
    [HideInInspector] public string m_ReplayDirectory;
    [HideInInspector] public string m_RankingDirectory;

    private static string m_AccountID = string.Empty;
    private static string m_EncryptedAccountID;

    public AnimationCurve[] m_AnimationCurve = new AnimationCurve[3];

    public static bool isOnline;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
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
        if (DebugOption.GenerateJsonFile)
        {
            GetComponent<ExplosionJsonWriter>().GenerateJsonFile();
            GetComponent<EndingCreditJsonWriter>().GenerateJsonFile();
            Debug.Log("A");
        }

        if (!TestIntegrity())
        {
            Utility.QuitGame();
        }
        ValidateAccount();
        m_GameSetting.LoadSettings();
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

    private bool TestIntegrity()
    {
        try
        {
            var (jsonData, hash) = Utility.LoadDataFileString(Application.dataPath, "resources1");
            if (Utility.Md5Sum(jsonData) == hash)
            {
                Debug.Log("무결성 검사가 완료되었습니다.");
                return true;
            }
            Debug.LogError("무결성 검사에 실패하였습니다.");
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"무결성 검사 중 오류가 발생하였습니다:\n{e}");
        }
        return false;
    }
}