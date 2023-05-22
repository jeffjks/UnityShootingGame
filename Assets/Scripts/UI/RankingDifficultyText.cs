using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingDifficultyText : MonoBehaviour
{
    public RankingDataLoader m_RankingDataLoader;
    
    private TMP_Text _text;
    private GameDifficulty _gameDifficulty;

    private readonly Dictionary<Language, string[]> _rankingDifficultyText = new()
    {
        { Language.English, new[] { "Normal Ranking", "Expert Ranking", "Hell Ranking" } },
        { Language.Korean, new[] { "노말 랭킹", "익스퍼트 랭킹", "헬 랭킹" } }
    };

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        _gameDifficulty = m_RankingDataLoader.m_GameDifficulty;
        SetText();
    }

    private void SetText()
    {
        try
        {
            _text.text = _rankingDifficultyText[GameSetting.m_Language][(int) _gameDifficulty];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _text.text = "Unknown";
        }
    }
}
