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

    public bool SetCostText(int cost)
    {
        const int maximumCost = SelectAttributesMenuHandler.MAXIMUM_COST;
        
        if (GameSetting.CurrentLanguage == Language.Korean)
        {
            _costWindowText.SetText($"비용\n{cost} / {maximumCost}");
        }
        else
        {
            _costWindowText.SetText($"Cost\n{cost} / {maximumCost}");
        }

        return SetCostColor(cost, maximumCost);
    }

    private bool SetCostColor(int cost, int maximumCost)
    {
        _costWindowText.color = cost > maximumCost ? _exceedTextColor : _defaultTextColor;
        return cost > maximumCost;
    }
}
