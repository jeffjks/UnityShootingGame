using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanguageButtonController : MonoBehaviour, IMoveHandler
{
    private TextMeshProUGUI _textUI;
    public string[] m_NativeTexts;
    public string[] m_Texts;

    private readonly Dictionary<Language, string[]> _textContainer = new();

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
        if (moveInputX > 0)
        {
            GameSetting.CurrentLanguage = GameSetting.CurrentLanguage.GetEnumNext();
        }
        else if (moveInputX < 0)
        {
            GameSetting.CurrentLanguage = GameSetting.CurrentLanguage.GetEnumPrev();
        }

        SetText();
    }

    private void SetText()
    {
        try
        {
            _textUI.SetText(_textContainer[GameSetting.CurrentLanguage][(int) GameSetting.CurrentLanguage]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.SetText("Unknown");
        }
    }
}
