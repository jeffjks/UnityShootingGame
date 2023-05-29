using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AttributesCostWindowController : MonoBehaviour
{
    private TextMeshProUGUI _costWindowText;

    private Color _defaultTextColor;
    private readonly Color _exceedTextColor = Color.red;

    private void Awake()
    {
        _costWindowText = GetComponentInChildren<TextMeshProUGUI>();
        _defaultTextColor = _costWindowText.color;
    }

    public void SetText(int costText)
    {
        int maximumCost = SelectAttributesMenuHandler.MAXIMUM_COST;
        
        if (GameSetting.m_Language == Language.Korean)
        {
            _costWindowText.SetText($"비용\n{costText} / {maximumCost}");
        }
        else
        {
            _costWindowText.SetText($"Cost\n{costText} / {maximumCost}");
        }

        if (costText > maximumCost)
        {
            _costWindowText.color = _exceedTextColor;
        }
        else
        {
            _costWindowText.color = _defaultTextColor;
        }
    }
}
