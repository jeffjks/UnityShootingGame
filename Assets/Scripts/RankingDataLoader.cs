using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class RankingDataLoader : MonoBehaviour
{
    public TextErrorMessage m_ErrorMessage;
    public RankingDataSlotLoader[] m_RankingDataSlotLoaders = new RankingDataSlotLoader[SLOTS_PER_PAGE];
    public RankingDataSlotLoader m_MyRankingDataSlotLoader; // Unused
    public GameDifficulty m_GameDifficulty;
    
    private const int MAX_PAGE = 2;
    private const int SLOTS_PER_PAGE = 5;
    private int _currentPage;
    private bool _isLoaded;
    private readonly Dictionary<GameDifficulty, List<LocalRankingData>> _localRankingDataList = new();

    private GameManager m_GameManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
    }

    private void OnEnable() {
        string id = m_GameManager.GetAccountID();
        _currentPage = 0;

        if (m_GameManager.m_NetworkAvailable) {
            if (id == string.Empty) {
                //DisplayRanking("OfflineException");
            }
            else {
                StartCoroutine(DisplayOnlineRanking(m_GameManager.GetAccountID(), SystemInfo.deviceUniqueIdentifier));
            }
        }
        else {
            LoadLocalRanking();
        }
    }

    private void OnDisable()
    {
        _isLoaded = false;
    }

    private void LoadLocalRanking() {
        if (!_localRankingDataList.ContainsKey(m_GameDifficulty))
        {
            _localRankingDataList[m_GameDifficulty] = new List<LocalRankingData>();

            BinaryReader br = OpenBinaryFile(Application.dataPath, $"ranking{(int)m_GameDifficulty}");
            
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
                    _localRankingDataList[m_GameDifficulty].Add(record);
                    //Console.WriteLine("{0} {1}", var1, var2);
                }
                catch (EndOfStreamException) { // 파일 끝에 도달한 예외 처리
                    br.Close();
                    break;
                }
            }

            _localRankingDataList[m_GameDifficulty].Sort(new Comparison<LocalRankingData>((n1, n2) => CompareListElement(n1, n2)));
        }

        DisplayRanking();
    }

    private BinaryReader OpenBinaryFile(string filePath, string fileName)
    {
        return new BinaryReader(File.Open($"{filePath}/{fileName}.dat", FileMode.OpenOrCreate, FileAccess.Read));
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

    private IEnumerator DisplayOnlineRanking(string id, string pcID) {
        string url = "http://jeffjks.cafe24.com/DeadPlanet2php/getRankingScore.php";

        WWWForm form = new WWWForm();
        form.AddField("difficulty", (int) m_GameDifficulty);
        form.AddField("userID", id);
        form.AddField("deviceUniqueIdentifier", pcID);

        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
            m_ErrorMessage.DisplayText("NetworkErrorException", webRequest.error);
        }
        _isLoaded = true;
    }

    private void DisplayRanking()
    {
        for (int i = 0; i < SLOTS_PER_PAGE; ++i)
        {
            var index = _currentPage * SLOTS_PER_PAGE + i;
            if (index < _localRankingDataList[m_GameDifficulty].Count)
            {
                m_RankingDataSlotLoaders[i].SetRankingSlot(index + 1, _localRankingDataList[m_GameDifficulty][index]);
                continue;
            }
            m_RankingDataSlotLoaders[i].InitRankingSlot();
        }

        _isLoaded = true;
    }

    public void MovePage(int move) {
        if (!_isLoaded)
        {
            return;
        }
        _currentPage += move;
        
        if (_currentPage < 0) {
            _currentPage = MAX_PAGE;
        }
        else if (_currentPage > MAX_PAGE) {
            _currentPage = 0;
        }

        DisplayRanking();
        AudioService.PlaySound("ConfirmUI");
    }
}
