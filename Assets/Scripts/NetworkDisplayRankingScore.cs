using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkDisplayRankingScore : MonoBehaviour
{
    public TextUI_ErrorMessage m_ErrorMessage;
    public RankingScoreSlot[] m_TopRankingScoreSlots = new RankingScoreSlot[5];
    public RankingScoreSlot m_MyRankingScoreSlot;

    [HideInInspector] public bool m_Active = false;

    private string[] m_ResponseText; // 0 : succeedMessage, 1~6 : myRank, 7~maxLength-1 : topRank, 
    private const int m_MaxPage = 3;
    private int m_Page;

    private GameManager m_GameManager = null;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
    }

    void OnEnable() {
        string id = m_GameManager.GetAccountID();
        m_Active = false;
        m_Page = 0;

        if (id == string.Empty) {
            TryDisplayScoreRanking("OfflineException");
        }
        else {
            StartCoroutine(DisplayScoreRanking(m_GameManager.m_Difficulty, m_GameManager.GetAccountID(), SystemInfo.deviceUniqueIdentifier));
        }
    }

    public IEnumerator DisplayScoreRanking(int difficulty, string id, string pcID) {
        string url = "http://jeffjks.cafe24.com/DeadPlanet2php/getRankingScore.php";

        if (difficulty < Difficulty.NORMAL || Difficulty.HELL < difficulty) {
            TryDisplayScoreRanking("ArgumentException");
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
            TryDisplayScoreRanking(www.downloadHandler.text);
        }
        yield return null;
    }

    private void TryDisplayScoreRanking(string response) {
        m_ResponseText = response.Split(',');
        string code = m_ResponseText[0];
        
        if (code == "ScoreRankingDisplaySucceed" && m_ResponseText.Length > 6) {
            UpdateMyRankingSlot();
            UpdateTopRankingSlot();
        }
        else {
            m_ErrorMessage.DisplayText(code);
        }
        m_Active = true;
    }

    private void NetworkError(string errorDetails) {
        m_ErrorMessage.DisplayText("NetworkErrorException", errorDetails);
        m_Active = true;
    }

    private void UpdateMyRankingSlot() {
        for (int i = 0; i < 6; i++) {
            m_MyRankingScoreSlot.m_RankingScoreDisplays[i].UpdateScoreInfo(m_ResponseText[i+1]);
        }
    }

    private void UpdateTopRankingSlot() {
        for (int j = 0; j < 5; j++) {
            for (int i = 0; i < 6; i++) {
                try {
                    m_TopRankingScoreSlots[j].m_RankingScoreDisplays[i].UpdateScoreInfo(m_ResponseText[m_Page*30 + (i+1) + (j+1)*6]);
                }
                catch (System.IndexOutOfRangeException) {
                    m_TopRankingScoreSlots[j].m_RankingScoreDisplays[i].UpdateScoreInfo(string.Empty);
                }
            }
        }
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
        UpdateTopRankingSlot();
        return m_Active;
    }
}
