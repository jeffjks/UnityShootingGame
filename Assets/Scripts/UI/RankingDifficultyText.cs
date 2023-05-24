using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingDifficultyText : MonoBehaviour
{
    public RankingDataLoader m_RankingDataLoader;
    public string[] m_NativeTexts;
    public string[] m_Texts;

    private readonly Dictionary<Language, string[]> _textContainer = new();
    private TextMeshProUGUI _textUI;
    private GameDifficulty _gameDifficulty;

    private void Awake()
    {
        _textUI = GetComponentInChildren<TextMeshProUGUI>();
        
        _textContainer[Language.Korean] = m_NativeTexts;
        _textContainer[Language.English] = m_Texts;
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
            _textUI.text = _textContainer[GameSetting.m_Language][(int) _gameDifficulty];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.text = "Unknown";
        }
    }
}
