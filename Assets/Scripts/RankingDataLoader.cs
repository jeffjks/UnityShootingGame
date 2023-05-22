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

public class RankingDataLoader : MonoBehaviour
{
    public TextErrorMessage m_ErrorMessage;
    public RankingDataSlotLoader[] m_RankingDataSlotLoaders = new RankingDataSlotLoader[5];
    public RankingDataSlotLoader m_MyRankingDataSlotLoader;
    
    private string[] m_ResponseText; // 0 : succeedMessage, 1~6 : myRank, 7~maxLength-1 : topRank, 
    private const int MAX_PAGE = 3;
    private int _currentPage;
    private bool _isLoaded;
    private List<LocalRankingData> m_LocalRankingDataList = new();

    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
        m_SystemManager = SystemManager.instance_sm;
    }

    void OnEnable() {
        string id = m_GameManager.GetAccountID();
        _isLoaded = false;
        _currentPage = 0;

        if (m_GameManager.m_NetworkAvailable) {
            if (id == string.Empty) {
                //DisplayRanking("OfflineException");
            }
            else {
                StartCoroutine(DisplayOnlineRanking(m_SystemManager.GetDifficulty(), m_GameManager.GetAccountID(), SystemInfo.deviceUniqueIdentifier));
            }
        }
        else {
            DisplayLocalRanking(m_SystemManager.GetDifficulty());
        }
    }

    private void DisplayLocalRanking(GameDifficulty difficulty) {
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

                    //int attributesCode = shipAttributes.GetAttributesCode();
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

        DisplayRanking();
    }

    private int CompareListElement(LocalRankingData n1, LocalRankingData n2) {
        if (n1.score == n2.score) {
            if (n1.date < n2.date) {
                return 1;
            }
            return -1;
        }
        if (n1.score > n2.score) {
            return 1;
        }
        return -1;
    }

    private IEnumerator DisplayOnlineRanking(GameDifficulty difficulty, string id, string pcID) {
        string url = "http://jeffjks.cafe24.com/DeadPlanet2php/getRankingScore.php";

        WWWForm form = new WWWForm();
        form.AddField("difficulty", (int) difficulty);
        form.AddField("userID", id);
        form.AddField("deviceUniqueIdentifier", pcID);

        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
            m_ErrorMessage.DisplayText("NetworkErrorException", webRequest.error);
        }
        _isLoaded = true;
    }

    private void DisplayRanking() {
        for (int i = 0; i < m_LocalRankingDataList.Count; ++i) {
            m_RankingDataSlotLoaders[i].LoadRankingData(i + 1, m_LocalRankingDataList[i]);
        }

        _isLoaded = true;
    }

    public bool TurnOverPage(int move) {
        if (_isLoaded) {
            _currentPage += move;
        }
        if (_currentPage < 0) {
            _currentPage = MAX_PAGE;
        }
        if (_currentPage > MAX_PAGE) {
            _currentPage = 0;
        }
        //UpdateTopRankingSlot();
        return _isLoaded;
    }
}
