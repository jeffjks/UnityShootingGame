using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUploadRankingScore : MonoBehaviour
{
    public TextErrorMessage m_ErrorMessage;

    private GameManager m_GameManager = null;
    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;

    void Start()
    {
        m_GameManager = GameManager.instance_gm;
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;

        string id = m_GameManager.GetAccountID();
        long totalScore = m_SystemManager.GetTotalScore();
        ShipAttributes shipAttributes = m_PlayerManager.m_CurrentAttributes;
        int totalMiss = m_SystemManager.GetTotalMiss();
        string pcID = SystemInfo.deviceUniqueIdentifier;

        if (m_GameManager.m_NetworkAvailable) {
            StartCoroutine(UploadScore(id, totalScore, shipAttributes, totalMiss, pcID));
        }
    }

    public IEnumerator UploadScore(string id, long totalScore, ShipAttributes shipAttributes, int totalMiss, string pcID) {
        string url = "http://jeffjks.cafe24.com/DeadPlanet2php/uploadRankingScore.php";

        if (SystemManager.Difficulty < GameDifficulty.Normal || GameDifficulty.Hell < SystemManager.Difficulty) {
            TryUploadScore("ArgumentException");
            yield break;
        }

        if (id.Length < 4) {
            TryUploadScore("BadUnauthorizedException");
            yield break;
        }

        WWWForm form = new WWWForm();
        /*
        form.AddField("difficulty", difficulty);
        form.AddField("userID", id);
        form.AddField("totalScore", (int) totalScore);
        form.AddField("shipAttributes", shipAttributes);
        form.AddField("totalMiss", (int) totalMiss);
        form.AddField("deviceUniqueIdentifier", pcID);*/

        UnityWebRequest webRequeset = UnityWebRequest.Post(url, form);
        yield return webRequeset.SendWebRequest();
        if(webRequeset.result == UnityWebRequest.Result.ConnectionError || webRequeset.result == UnityWebRequest.Result.ProtocolError) {
            NetworkError(webRequeset.error);
        }
        else {
            TryUploadScore(webRequeset.downloadHandler.text);
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
        Debug.LogAssertion(errorDetails);
        m_ErrorMessage.DisplayText("NetworkErrorException", errorDetails);
    }
}
