using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TrainingOption
{
    Stage,
    Difficulty,
    StartingPoint
}
public class TrainingButtonController : MonoBehaviour, IMoveHandler
{
    public TrainingOption m_TrainingOption;
    public TrainingMenuHandler m_TrainingMenuHandler;
    
    private TextMeshProUGUI _textUI;

    private static readonly Dictionary<Language, string[]> _stageText = new()
    {
        { Language.English, new[] { "Stage 1", "Stage 2", "Stage 3", "Stage 4", "Stage 5" } },
        { Language.Korean, new[] { "스테이지 1", "스테이지 2", "스테이지 3", "스테이지 4", "스테이지 5" } }
    };
    private static readonly Dictionary<Language, string[]> _difficultyText = new()
    {
        { Language.English, new[] { "Normal", "Expert", "Hell" } },
        { Language.Korean, new[] { "노말", "익스퍼트", "헬" } }
    };
    private static readonly Dictionary<Language, string[]> _startingPointText = new()
    {
        { Language.English, new[] { "Boss", "Field" } },
        { Language.Korean, new[] { "보스전", "필드전" } }
    };

    private void Awake()
    {
        _textUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        SetText();
    }

    public void OnMove(AxisEventData axisEventData)
    {
        /*
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        var moveInputX = (int) axisEventData.moveVector.x;

        switch (m_TrainingOption)
        {
            case TrainingOption.Stage:
                SystemManager.Difficulty
        }
        
        SystemManager.TrainingInfo
        GameSetting.m_GraphicOptions[m_GraphicsOption] += moveInputX;
        
        var maxCount = GameSetting.m_GraphicOptionsCount[m_GraphicsOption];
        if (GameSetting.m_GraphicOptions[m_GraphicsOption] < 0)
        {
            GameSetting.m_GraphicOptions[m_GraphicsOption] = maxCount - 1;
        }
        else if (GameSetting.m_GraphicOptions[m_GraphicsOption] >= maxCount)
        {
            GameSetting.m_GraphicOptions[m_GraphicsOption] = 0;
        }

        SetText();*/
    }

    private void SetText()
    {/*
        try
        {
            switch (m_GraphicsOption)
            {
                case GraphicsOption.Resolution:
                    Resolution resolution = GameSetting.GetCurrentResolution();
                    _textUI.text = $"{resolution.width} x {resolution.height}";
                    break;
                case GraphicsOption.FullScreen:
                    _textUI.text = _fullScreenText[GameSetting.m_Language][GameSetting.m_GraphicOptions[m_GraphicsOption]];
                    break;
                case GraphicsOption.Quality:
                    _textUI.text = _qualityText[GameSetting.m_Language][GameSetting.m_GraphicOptions[m_GraphicsOption]];
                    break;
                case GraphicsOption.AntiAliasing:
                    _textUI.text = _antiAliasingText[GameSetting.m_Language][GameSetting.m_GraphicOptions[m_GraphicsOption]];
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.text = "Unknown";
        }*/
    }
}
