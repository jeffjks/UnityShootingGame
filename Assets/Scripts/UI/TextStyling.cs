using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextStyling : MonoBehaviour
{
    [TextArea]
    public string m_NativeText;

    private TextMeshProUGUI _textUI;
    private string _englishText;

    private GameManager m_GameManager = null;

    void Awake()
    {
        _textUI = GetComponentInChildren<TextMeshProUGUI>();
        _englishText = _textUI.text;
    }

    private void Start()
    {
        if (!m_GameManager)
        {
            m_GameManager = GameManager.instance_gm;
        }
        SetText();
    }

    private void OnEnable()
    {
        if (!m_GameManager)
        {
            return;
        }
        SetText();
    }

    private void SetText()
    {
        if (m_NativeText == "..." || m_NativeText == String.Empty)
        {
            return;
        }
        if (GameSetting.m_Language == Language.English)
        {
            _textUI.text = _englishText;
        }
        else if (GameSetting.m_Language == Language.Korean)
        {
            _textUI.text = m_NativeText;
        }
    }
}
