﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUploadRankingScore : MonoBehaviour
{
    public TextErrorMessage m_ErrorMessage;

    void Start()
    {
        string id = GameManager.GetAccountID();
        long totalScore = InGameDataManager.Instance.TotalScore;
        ShipAttributes shipAttributes = InGameDataManager.Instance.CurrentShipAttributes;
        int totalMiss = InGameDataManager.Instance.TotalMiss;
        string pcID = SystemInfo.deviceUniqueIdentifier;

        if (DebugOption.NetworkAvailable) {
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
