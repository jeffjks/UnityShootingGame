
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
    public Button m_ConfirmButton;

    public const int MAXIMUM_COST = 500;

    private bool _onConfirmSelected = false;
    private int _totalCost;
    private readonly Dictionary<AttributeType, int> _attributeCost = new();
    
    private Action Action_startStage;

    private void OnEnable()
    {
        AudioService.PlayMusic("Select");
    }

    private void Start()
    {
        Action_startStage += StartStage;
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
    }

    public void SelectDetail()
    {
        _onConfirmSelected = true;
        _lastSelected[gameObject] = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        m_ConfirmButton.Select();
        AudioService.PlaySound("ConfirmUI");
    }

    public override void Back()
    {
        if (!_onConfirmSelected)
        {
            BackToMainMenu();
            return;
        }
        _onConfirmSelected = false;
        _lastSelected[gameObject].Select();
        AudioService.PlaySound("CancelUI");
    }

    public void UpdateTotalCost(AttributeType attributeType, int cost)
    {
        _totalCost -= _attributeCost[attributeType];
        _attributeCost[attributeType] = cost;
        _totalCost += _attributeCost[attributeType];
        m_AttributesCostWindowController.SetText(_totalCost);
    }

    public void Confirm()
    {
        EventSystem.current.currentInputModule.enabled = false;
        m_ConfirmButton.interactable = false;
        FadeScreenService.ScreenFadeOut(2f, Action_startStage);
        AudioService.FadeOutMusic(2f);
        AudioService.PlaySound("SallyUI");
    }

    private void StartStage()
    {
        _previousMenuStack.Clear();
        if (SystemManager.GameMode == GameMode.Normal)
        {
            SceneManager.LoadScene("Stage1");
        }
        else if (SystemManager.GameMode == GameMode.Training)
        {
            int stageNum = SystemManager.TrainingInfo.stage + 1;
            SceneManager.LoadScene($"Stage{stageNum}");
        }
    }
}
