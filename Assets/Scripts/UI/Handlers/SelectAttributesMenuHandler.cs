
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectAttributesMenuHandler : MenuHandler
{
    public AttributesDetailsWindowController m_AttributesDetailsWindowController;
    public AttributesCostWindowController m_AttributesCostWindowController;
    public TextMeshProUGUI m_AttributesTypeText;

    public const int MAXIMUM_COST = 500;
    
    private int _totalCost;
    private readonly Dictionary<AttributeType, int> _attributeCost = new();
    private Action Action_startStage;

    public SelectAttributesMenuHandler()
    {
        AttributeType attributeType = AttributeType.Color;
        for (int i = 0; i < attributeType.GetEnumCount(); ++i)
        {
            _attributeCost[attributeType] = 0;
            attributeType = attributeType.GetEnumNext();
        }
    }

    void Start()
    {
        Action_startStage += StartStage;
    }
    
    void OnEnable()
    {
        AudioService.PlayMusic("Select");
    }

    public void SetAttributeName(string typeName)
    {
        m_AttributesTypeText.SetText(typeName);
    }

    public void SetAttributesDetailsInfo(DetailsWindowElement data)
    {
        m_AttributesDetailsWindowController.SetWindow(data);
    }

    private void StartStage()
    {
        SceneManager.LoadScene("Stage1");
    }

    public override void Back()
    {
        BackToMainMenu();
    }

    public void Confirm()
    {
        EventSystem.current.currentInputModule.enabled = false;
        ScreenEffectService.ScreenFadeOut(2f, Action_startStage);
        AudioService.FadeOutMusic(2f);
        AudioService.PlaySound("SallyUI");
    }

    public void UpdateTotalCost(AttributeType attributeType, int cost)
    {
        _totalCost -= _attributeCost[attributeType];
        _attributeCost[attributeType] = cost;
        _totalCost += _attributeCost[attributeType];
        m_AttributesCostWindowController.SetText(_totalCost);
    }
}
