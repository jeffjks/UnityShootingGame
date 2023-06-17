
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectAttributesMenuHandler : MenuHandler
{
    public AttributesDetailsWindowController m_AttributesDetailsWindowController;
    public AttributesCostWindowController m_AttributesCostWindowController;
    public TextMeshProUGUI m_AttributesTypeText;
    public MenuHandler m_AttributeConfirmPanel;
    //public Button m_ConfirmButton;

    public const int MAXIMUM_COST = 500;
    
    private int _totalCost;
    private readonly Dictionary<AttributeType, int> _attributeCost = new();

    private void OnEnable()
    {
        AudioService.PlayMusic("Select");
    }

    public SelectAttributesMenuHandler()
    {
        AttributeType attributeType = AttributeType.Color;
        for (int i = 0; i < attributeType.GetEnumCount(); ++i)
        {
            _attributeCost[attributeType] = 0;
            attributeType = attributeType.GetEnumNext();
        }
    }

    public void SetAttributeName(string typeName)
    {
        m_AttributesTypeText.SetText(typeName);
    }

    public void SetAttributesDetailsInfo(DetailsWindowElement data)
    {
        m_AttributesDetailsWindowController.SetWindow(data);
        if (!CriticalStateSystem.InCriticalState)
            m_AttributesDetailsWindowController.PlayTransitionAnimation();
    }

    public void SelectDetail()
    {
        GoToTargetMenu(m_AttributeConfirmPanel, false);
    }

    public override void Back()
    {
        m_PreserveLastSelection = false;
        BackToMainMenu();
    }

    public void UpdateTotalCost(AttributeType attributeType, int cost)
    {
        _totalCost -= _attributeCost[attributeType];
        _attributeCost[attributeType] = cost;
        _totalCost += _attributeCost[attributeType];
        m_AttributesCostWindowController.SetText(_totalCost);
    }
}
