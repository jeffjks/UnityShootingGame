using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttributesDetailsButtonController : MonoBehaviour, ISelectHandler, IMoveHandler
{
    public SelectAttributesMenuHandler m_SelectAttributesMenuHandler;
    public AttributesDetailsWindowDatas m_AttributesDetailsWindowDatas;

    private int _currentSelection;
    private readonly Dictionary<Language, string> _textContainer = new();

    private int CurrentSelection
    {
        get => _currentSelection;
        set
        {
            var clampedValue = Mathf.Clamp(value, 0, _attributesMaxNumber - 1);
            if (clampedValue == value)
            {
                _currentSelection = clampedValue;
                OnSelection();
            }
        }
    }

    private int _attributesMaxNumber;

    private void Awake()
    {
        _attributesMaxNumber = m_AttributesDetailsWindowDatas.DetailsWindowElements.Length;
        _textContainer[Language.English] = m_AttributesDetailsWindowDatas.AttributeName;
        _textContainer[Language.Korean] = m_AttributesDetailsWindowDatas.NativeAttributeName;
    }

    private void OnEnable()
    {
        _currentSelection = PlayerManager.CurrentAttributes.GetAttributes(m_AttributesDetailsWindowDatas.AttributeType);
        OnSelection();
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelection();
    }

    public void OnMove(AxisEventData axisEventData)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        var moveInputX = (int) axisEventData.moveVector.x;
        if (moveInputX != 0)
        {
            CurrentSelection += moveInputX;
        }
    }

    private void OnSelection()
    {
        AttributeType attributeType = m_AttributesDetailsWindowDatas.AttributeType;
        DetailsWindowElement data = m_AttributesDetailsWindowDatas.DetailsWindowElements[CurrentSelection];
        string attributeName = _textContainer[GameSetting.m_Language];
        m_SelectAttributesMenuHandler.SetAttributesDetailsInfo(data);
        m_SelectAttributesMenuHandler.UpdateTotalCost(attributeType, data.cost);
        m_SelectAttributesMenuHandler.SetAttributeName(attributeName);
    }
}
