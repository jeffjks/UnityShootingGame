using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanguageButtonController : MonoBehaviour, IMoveHandler
{
    private TMP_Text _text;

    private readonly Dictionary<Language, string> _languageText = new()
    {
        { Language.English, "English" },
        { Language.Korean, "한국어" }
    };

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
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
            _text.text = _languageText[GameSetting.m_Language];
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _text.text = "Unknown";
        }
    }
}
