using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUploadRankingScore : MonoBehaviour
{
    public TextUI_ErrorMessage m_ErrorMessage;

    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;

    void Start()
    {
        m_GameManager = GameManager.instance_gm;
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;

        int difficulty = m_SystemManager.m_Difficulty;
        string id = m_GameManager.GetAccountID();
        uint totalScore = m_SystemManager.GetTotalScore();
        int shipAttributes = m_PlayerManager.m_CurrentAttributes.GetAttributesCode();
        uint totalMiss = m_SystemManager.GetTotalMiss();
        string pcID = SystemInfo.deviceUniqueIdentifier;

        StartCoroutine(UploadScore(difficulty, id, totalScore, shipAttributes, totalMiss, pcID));
    }

    public IEnumerator UploadScore(int difficulty, string id, uint totalScore, int shipAttributes, uint totalMiss, string pcID) {
        string url = "http://jeffjks.cafe24.com/DeadPlanet2php/uploadRankingScore.php";

        if (difficulty < Difficulty.NORMAL || Difficulty.HELL < difficulty) {
            TryUploadScore("ArgumentException");
            yield break;
        }

        if (id.Length < 4) {
            TryUploadScore("BadUnauthorizedException");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("difficulty", difficulty);
        form.AddField("userID", id);
        form.AddField("totalScore", (int) totalScore);
        form.AddField("shipAttributes", shipAttributes);
        form.AddField("totalMiss", (int) totalMiss);
        form.AddField("deviceUniqueIdentifier", pcID);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if(www.isNetworkError || www.isHttpError) {
            NetworkError(www.error);
        }
        else {
            TryUploadScore(www.downloadHandler.text);
        }
        yield return null;
    }

    public void TryUploadScore(string code) {
        if (code == "ScoreUploadingSucceed" || code == "ScoreUploadingNotHighestScore") {
            return;
        }
        else {
            m_ErrorMessage.DisplayText(code);
        }
    }

    public void NetworkError(string errorDetails) {
        Debug.Log(errorDetails);
        m_ErrorMessage.DisplayText("NetworkErrorException", errorDetails);
    }
}
