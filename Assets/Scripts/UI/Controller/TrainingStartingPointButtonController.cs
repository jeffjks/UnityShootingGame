using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrainingStartingPointButtonController : MonoBehaviour, IMoveHandler
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

        SystemManager.TrainingInfo.bossOnly = !SystemManager.TrainingInfo.bossOnly;

        SetText();
    }

    private void SetText()
    {
        try
        {
            int index = -1;
            index = SystemManager.TrainingInfo.bossOnly ? 0 : 1;
            _textUI.SetText(_textContainer[GameSetting.CurrentLanguage][index]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _textUI.SetText("Unknown");
        }
    }
}
