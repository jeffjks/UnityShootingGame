using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AntiAliasingButtonController : MonoBehaviour, IMoveHandler
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
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        var moveInputX = (int) axisEventData.moveVector.x;
        if (moveInputX > 0)
        {
            GameSetting.GraphicsAntiAliasing = GameSetting.GraphicsAntiAliasing.GetEnumNext(true);
        }
        else if (moveInputX < 0)
        {
            GameSetting.GraphicsAntiAliasing = GameSetting.GraphicsAntiAliasing.GetEnumPrev(true);
        }

        SetText();
    }

    private void SetText()
    {
        try
        {
            _textUI.SetText(_textContainer[GameSetting.CurrentLanguage][(int) GameSetting.GraphicsAntiAliasing]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.SetText("Unknown");
        }
    }
}
