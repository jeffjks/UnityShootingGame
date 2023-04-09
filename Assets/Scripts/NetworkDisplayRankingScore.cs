using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public struct LocalRankingData {
    public string id;
    public long score;
    public ShipAttributes shipAttributes;
    public int miss;
    public long date;

    public LocalRankingData(string id, long score, ShipAttributes shipAttributes, int miss, long date) {
        this.id = id;
        this.score = score;
        this.shipAttributes = shipAttributes;
        this.miss = miss;
        this.date = date;
    }

    public bool isBetter(LocalRankingData localRankingData) {
        if (id != localRankingData.id) {
            return false;
        }
        if (score > localRankingData.score) {
            return false;
        }
        if (shipAttributes != localRankingData.shipAttributes) {
            return false;
        }
        if (miss < localRankingData.miss) {
            return false;
        }
        if (date < localRankingData.date) {
            return false;
        }
        return true;
    }

    public void Print() {
        int attributesCode = shipAttributes.GetAttributesCode();
        Debug.Log($"{id}, {score}, {attributesCode}, {miss}, {date}");
    }
}

public class NetworkDisplayRankingScore : MonoBehaviour
{
    public TextUI_ErrorMessage m_ErrorMessage;
    public RankingScoreSlot[] m_TopRankingScoreSlots = new RankingScoreSlot[5];
    public RankingScoreSlot m_MyRankingScoreSlot;

    [HideInInspector] public bool m_Active = false;

    private string[] m_ResponseText; // 0 : succeedMessage, 1~6 : myRank, 7~maxLength-1 : topRank, 
    private const int m_MaxPage = 3;
    private int m_Page;
    private List<LocalRankingData> m_LocalRankingDataList = new List<LocalRankingData>();

    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
        m_SystemManager = SystemManager.instance_sm;
    }

    void OnEnable() {
        string id = m_GameManager.GetAccountID();
        m_Active = false;
        m_Page = 0;

        if (m_GameManager.m_NetworkAvailable) {
            if (id == string.Empty) {
                //TryDisplayScoreRanking("OfflineException");
            }
            else {
                StartCoroutine(DisplayScoreRanking(m_SystemManager.GetDifficulty(), m_GameManager.GetAccountID(), SystemInfo.deviceUniqueIdentifier));
            }
        }
        else {
            DisplayLocalRanking(m_SystemManager.GetDifficulty());
        }
    }

    public void DisplayLocalRanking(int difficulty) {
        if (m_LocalRankingDataList.Count == 0) {
            string filePath = $"{m_GameManager.m_RankingDirectory}ranking{difficulty}.bin";

            BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read));
            while (true){
                try {
                    string id = br.ReadString();
                    long score = br.ReadInt64();
                    ShipAttributes shipAttributes = new ShipAttributes(br.ReadInt32());
                    int miss = br.ReadInt32();
                    long date = br.ReadInt64();

                    int attributesCode = shipAttributes.GetAttributesCode();

                    //Debug.Log($"{id}, {score}, {attributesCode}, {miss}, {date}");

                    LocalRankingData record = new LocalRankingData(id, score, shipAttributes, miss, date);
                    m_LocalRankingDataList.Add(record);
                    //Console.WriteLine("{0} {1}", var1, var2);
                }
                catch (EndOfStreamException) { // 파일 끝에 도달한 예외 처리
                    br.Close();
                    break;
                }
            }

            m_LocalRankingDataList.Sort(new Comparison<LocalRankingData>((n1, n2) => CompareListElement(n1, n2)));
        }

        TryDisplayScoreRanking();
    }

    private int CompareListElement(LocalRankingData n1, LocalRankingData n2) {
        if (n1.score == n2.score) {
            if (n1.date < n2.date) {
                return 1;
            }
            else {
                return -1;
            }
        }
        if (n1.score > n2.score) {
            return 1;
        }
        return -1;
    }

    public IEnumerator DisplayScoreRanking(int difficulty, string id, string pcID) {
        string url = "http://jeffjks.cafe24.com/DeadPlanet2php/getRankingScore.php";

        if (difficulty < Difficulty.NORMAL || Difficulty.HELL < difficulty) {
            //TryDisplayScoreRanking("ArgumentException");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("difficulty", difficulty);
        form.AddField("userID", id);
        form.AddField("deviceUniqueIdentifier", pcID);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError) {
            NetworkError(www.error);
        }
        else {
            //TryDisplayScoreRanking(www.downloadHandler.text);
        }
        yield return null;
    }

    private void TryDisplayScoreRanking() {
        /*
        m_ResponseText = response.Split(',');
        string code = m_ResponseText[0];
        
        if (code == "ScoreRankingDisplaySucceed" && m_ResponseText.Length > 6) {
            UpdateMyRankingSlot();
            UpdateTopRankingSlot();
        }
        else {
            m_ErrorMessage.DisplayText(code);
        }
        */
        m_Active = true;
        for (int i = 0; i < m_LocalRankingDataList.Count; ++i) {
            m_TopRankingScoreSlots[i].UpdateScoreInfo(i + 1, m_LocalRankingDataList[i]);
        }
    }

    private void NetworkError(string errorDetails) {
        m_ErrorMessage.DisplayText("NetworkErrorException", errorDetails);
        m_Active = true;
    }

    public bool TurnOverPage(int move) {
        if (m_Active) {
            m_Page += move;
        }
        if (m_Page < 0) {
            m_Page = m_MaxPage;
        }
        if (m_Page > m_MaxPage) {
            m_Page = 0;
        }
        //UpdateTopRankingSlot();
        return m_Active;
    }
}
