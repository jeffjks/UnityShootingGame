using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttributesDetailsButtonController : MonoBehaviour, ISelectHandler, IMoveHandler
{
    public SelectAttributesMenuHandler m_SelectAttributesMenuHandler;
    public AttributesDetailsWindowDatas m_AttributesDetailsWindowDatas;

    private int _currentSelection;

    private int CurrentSelection
    {
        get => _currentSelection;
        set
        {
            _currentSelection = Mathf.Clamp(value, 0, _attributesMaxNumber - 1);
            OnSelection();
        }
    }

    private int _attributesMaxNumber;
    private PlayerManager m_PlayerManager;

    private void Awake()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        _attributesMaxNumber = m_AttributesDetailsWindowDatas.DetailsWindowElements.Length;
    }

    private void OnEnable()
    {
        _currentSelection = PlayerManager.CurrentAttributes.GetAttributes(m_AttributesDetailsWindowDatas.AttributeType);
        OnSelection();
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelection();
        
        string attributeName;
        if (GameSetting.m_Language == Language.Korean)
        {
            attributeName = m_AttributesDetailsWindowDatas.NativeAttributeName;
        }
        else
        {
            attributeName = m_AttributesDetailsWindowDatas.AttributeName;
        }
        m_SelectAttributesMenuHandler.SetAttributeName(attributeName);
    }

    public void OnMove(AxisEventData axisEventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        var moveInputX = (int) axisEventData.moveVector.x;
        CurrentSelection += moveInputX;
    }

    private void OnSelection()
    {
        AttributeType attributeType = m_AttributesDetailsWindowDatas.AttributeType;
        DetailsWindowElement data = m_AttributesDetailsWindowDatas.DetailsWindowElements[CurrentSelection];
        m_SelectAttributesMenuHandler.SetAttributesDetailsInfo(data);
        m_SelectAttributesMenuHandler.UpdateTotalCost(attributeType, data.cost);
    }
}
