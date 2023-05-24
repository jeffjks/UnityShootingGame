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
    public string[] m_NativeTexts;
    public string[] m_Texts;

    private readonly Dictionary<Language, string[]> _textContainer = new();

    private TextMeshProUGUI _textUI;

    private void Awake()
    {
        _textUI = GetComponentInChildren<TextMeshProUGUI>();
        
        _textContainer[Language.Korean] = m_NativeTexts;
        _textContainer[Language.English] = m_Texts;
    }

    private void OnEnable()
    {
        SetText();
    }

    public void OnMove(AxisEventData axisEventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        var moveInputX = (int) axisEventData.moveVector.x;

        switch (m_TrainingOption)
        {
            case TrainingOption.Stage:
                SystemManager.TrainingInfo.stage += moveInputX;
                
                if (SystemManager.TrainingInfo.stage < 0)
                    SystemManager.TrainingInfo.stage = 4;
                else if (SystemManager.TrainingInfo.stage >= 5)
                    SystemManager.TrainingInfo.stage = 0;
                break;
            
            case TrainingOption.Difficulty:
                if (moveInputX > 0)
                    SystemManager.SetDifficulty(SystemManager.Difficulty.GetEnumNext());
                else if (moveInputX < 0)
                    SystemManager.SetDifficulty(SystemManager.Difficulty.GetEnumPrev());
                break;
            
            case TrainingOption.StartingPoint:
                SystemManager.TrainingInfo.bossOnly = !SystemManager.TrainingInfo.bossOnly;
                break;
        }

        SetText();
    }

    private void SetText()
    {
        try
        {
            int index = -1;
            switch (m_TrainingOption)
            {
                case TrainingOption.Stage:
                    index = SystemManager.TrainingInfo.stage;
                    break;
            
                case TrainingOption.Difficulty:
                    index = (int) SystemManager.Difficulty;
                    break;
            
                case TrainingOption.StartingPoint:
                    index = SystemManager.TrainingInfo.bossOnly ? 0 : 1;
                    break;
            }
            _textUI.text = _textContainer[GameSetting.m_Language][index];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.text = "Unknown";
        }
    }
}
