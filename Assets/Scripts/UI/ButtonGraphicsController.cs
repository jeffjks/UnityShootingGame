using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonGraphicsController : MonoBehaviour, IMoveHandler
{
    public GraphicsOption m_GraphicsOption;
    
    private TMP_Text _text;

    private readonly Dictionary<Language, string[]> _fullScreenText = new()
    {
        { Language.English, new[] { "Full", "Windowed" } },
        { Language.Korean, new[] { "전체 화면", "창 화면" } }
    };
    private readonly Dictionary<Language, string[]> _qualityText = new()
    {
        { Language.English, new[] { "Ultra", "Very High", "High", "Medium", "Low", "Very Low" } },
        { Language.Korean, new[] { "울트라", "매우 높음", "높음", "중간", "낮음", "매우 낮음" } }
    };
    private readonly Dictionary<Language, string[]> _antiAliasingText = new()
    {
        { Language.English, new[] { "On", "Off" } },
        { Language.Korean, new[] { "사용", "미사용" } }
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
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            var moveInputX = (int) axisEventData.moveVector.x;
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
    }

    private void SetText()
    {
        try
        {
            switch (m_GraphicsOption)
            {
                case GraphicsOption.Resolution:
                    Resolution resolution = GameSetting.GetCurrentResolution();
                    _text.text = $"{resolution.width} x {resolution.height}";
                    break;
                case GraphicsOption.FullScreen:
                    _text.text = _fullScreenText[GameSetting.m_Language][GameSetting.m_GraphicOptions[m_GraphicsOption]];
                    break;
                case GraphicsOption.Quality:
                    _text.text = _qualityText[GameSetting.m_Language][GameSetting.m_GraphicOptions[m_GraphicsOption]];
                    break;
                case GraphicsOption.AntiAliasing:
                    _text.text = _antiAliasingText[GameSetting.m_Language][GameSetting.m_GraphicOptions[m_GraphicsOption]];
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _text.text = "Unknown";
        }
    }
}
