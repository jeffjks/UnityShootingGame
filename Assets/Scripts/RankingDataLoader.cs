using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class RankingDataLoader : MonoBehaviour
{
    public TextErrorMessage m_TextErrorMessage;
    public RankingDataSlotLoader[] m_RankingDataSlotLoaders = new RankingDataSlotLoader[SLOTS_PER_PAGE];
    public RankingDataSlotLoader m_MyRankingDataSlotLoader; // Unused
    public RankingPageText m_RankingPageText;
    public GameDifficulty m_GameDifficulty;

    private int CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            m_RankingPageText.SetText(_currentPage, MAX_PAGE);
        }
    }
    
    private const int MAX_PAGE = 2;
    private const int SLOTS_PER_PAGE = 5;
    private int _currentPage;
    private bool _isLoaded;
    private readonly Dictionary<GameDifficulty, List<LocalRankingData>> _localRankingDataList = new();

    private void OnEnable() {
        string id = GameManager.GetAccountID();
        CurrentPage = 0;

        if (DebugOption.NetworkAvailable) {
            if (id == string.Empty) {
                //DisplayRanking("OfflineException");
            }
            else {
                StartCoroutine(DisplayOnlineRanking(GameManager.GetAccountID(), SystemInfo.deviceUniqueIdentifier));
            }
        }
        else
        {
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
            var rankingData = Utility.LoadDataFile<List<LocalRankingData>>(Application.dataPath, $"ranking{(int)m_GameDifficulty}").jsonData;
            if (rankingData == null)
            {
                m_TextErrorMessage.DisplayText("FileLoadException");
                rankingData = new List<LocalRankingData>();
            }
            else
            {
                rankingData.Sort();
            }
            _localRankingDataList[m_GameDifficulty] = rankingData;
        }

        DisplayRanking();
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
            m_TextErrorMessage.DisplayText("NetworkErrorException", webRequest.error);
        }
        _isLoaded = true;
    }

    private void DisplayRanking()
    {
        for (int i = 0; i < SLOTS_PER_PAGE; ++i)
        {
            var index = CurrentPage * SLOTS_PER_PAGE + i;
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
        CurrentPage += move;
        
        if (CurrentPage < 0) {
            CurrentPage = MAX_PAGE;
        }
        else if (CurrentPage > MAX_PAGE) {
            CurrentPage = 0;
        }

        DisplayRanking();
        //AudioService.PlaySound("ConfirmUI");
    }
}
