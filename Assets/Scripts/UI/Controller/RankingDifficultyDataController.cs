using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RankingDifficultyDataController : MonoBehaviour, IMoveHandler
{
    [HideInInspector] public GameDifficulty m_GameDifficulty;
    public TMP_Text m_Text;

    private readonly Dictionary<Language, string[]> _rankingDifficultyText = new()
    {
        { Language.English, new[] { "Normal Ranking", "Expert Ranking", "Hell Ranking" } },
        { Language.Korean, new[] { "노말 랭킹", "익스퍼트 랭킹", "헬 랭킹" } }
    };

    private void Awake()
    {
        m_Text = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        SetText();
    }

    private void SetText()
    {
        try
        {
            m_Text.text = _rankingDifficultyText[GameSetting.m_Language][(int) m_GameDifficulty];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            m_Text.text = "Unknown";
        }
    }

    public void OnMove(AxisEventData axisEventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        var moveInputX = (int) axisEventData.moveVector.x;
        GameSetting.m_Language += moveInputX;
        
        var maxCount = GameSetting.m_LanguageCount;
        if (GameSetting.m_Language < 0)
        {
            GameSetting.m_Language = (Language) maxCount - 1;
        }
        else if (GameSetting.m_Language >= (Language) maxCount)
        {
            GameSetting.m_Language = 0;
        }

        SetText();
    }
}
