using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public NetworkAccount m_NetworkAccount;
    
    //[HideInInspector] public ShipAttribute m_CurrentAttributes;

    private static string m_AccountID = string.Empty;
    private static string m_EncryptedAccountID;
    
    public List<AnimationCurve> m_AnimationCurves;

    public static bool isOnline;
    public static bool IsDebugScene;

    private class EncryptedFile
    {
        public string filePath;
        public List<string> fileList;

        public EncryptedFile(string filePath, List<string> fileList)
        {
            this.filePath = filePath;
            this.fileList = fileList;
        }
    }

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
        }
        #endif
        GetComponent<RankingJsonWriter>().GenerateJsonFile();

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

#if UNITY_EDITOR
    public static float RandomTest(float r1, float r2)
    {
        var r = Random.Range(r1, r2);
        //Debug.LogWarning($"{ReplayManager.CurrentFrame}: {r}");
        return r;
    }

    public static int RandomTest(int r1, int r2)
    {
        var r = Random.Range(r1, r2);
        //Debug.LogWarning($"{ReplayManager.CurrentFrame}: {r}");
        return r;
    }
#endif

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
            var resourceFile = new EncryptedFile(Application.streamingAssetsPath, new List<string> { "resources1.dat", "resources2.dat" });
            var rankingFile = new EncryptedFile(Application.dataPath, new List<string> { "ranking0.dat", "ranking1.dat", "ranking2.dat" });
            List<EncryptedFile> encryptedFiles = new () { resourceFile, rankingFile };

            foreach (var encryptedFile in encryptedFiles)
            {
                foreach (var fileName in encryptedFile.fileList)
                {
                    if (!File.Exists($"{encryptedFile.filePath}/{fileName}"))
                    {
                        if (resourceFile.filePath == Application.dataPath)
                            continue;
                        Debug.LogError("게임 실행에 필요한 파일을 찾을 수 없습니다.");
                    }
                    var (jsonData, hash) = Utility.LoadDataFileString(encryptedFile.filePath, fileName);
                
                    if (jsonData != null && Utility.Md5Sum(jsonData) != hash)
                    {
                        Debug.LogError("무결성 검사에 실패하였습니다.");
                        return false;
                    }
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