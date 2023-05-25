using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsButtonController : MonoBehaviour, IMoveHandler
{
    public GraphicsOption m_GraphicsOption;
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
            
        }
        
        
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

        SetText();
    }

    private void SetText()
    {
        try
        {
            if (m_GraphicsOption == GraphicsOption.Resolution)
            {
                Resolution resolution = GameSetting.GetCurrentResolution();
                _textUI.text = $"{resolution.width} x {resolution.height}";
            }
            else
            {
                _textUI.text = _textContainer[GameSetting.m_Language][GameSetting.m_GraphicOptions[m_GraphicsOption]];
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.text = "Unknown";
        }
    }
}
