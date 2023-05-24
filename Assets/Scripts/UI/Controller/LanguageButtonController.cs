using System;
using System.Collections;
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

    private void SetText()
    {
        try
        {
            _textUI.text = _textContainer[GameSetting.m_Language][(int) GameSetting.m_Language];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.text = "Unknown";
        }
    }
}
