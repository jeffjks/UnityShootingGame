using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrainingStageButtonController : MonoBehaviour, IMoveHandler
{
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
        if (axisEventData.moveDir is MoveDirection.Up or MoveDirection.Down)
            return;

        var moveInputX = (int) axisEventData.moveVector.x;

        SystemManager.TrainingInfo.stage += moveInputX;
                
        if (SystemManager.TrainingInfo.stage < 0)
            SystemManager.TrainingInfo.stage = 4;
        else if (SystemManager.TrainingInfo.stage > 4)
            SystemManager.TrainingInfo.stage = 0;

        SetText();
    }

    private void SetText()
    {
        try
        {
            int index = SystemManager.TrainingInfo.stage;
            _textUI.SetText(_textContainer[GameSetting.CurrentLanguage][index]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.SetText("Unknown");
        }
    }
}
