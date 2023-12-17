using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameText_Score : MonoBehaviour
{
    public TextMeshProUGUI m_ScoreText;

    private InGameDataManager _inGameDataManager;

    private void Awake()
    {
        _inGameDataManager = FindObjectOfType<InGameDataManager>();
        _inGameDataManager.Action_OnUpdateScore += UpdateScoreText;
    }

    private void OnDestroy()
    {
        _inGameDataManager.Action_OnUpdateScore -= UpdateScoreText;
    }

    private void UpdateScoreText(long value)
    {
        m_ScoreText.SetText(value.ToString());
    }
}
