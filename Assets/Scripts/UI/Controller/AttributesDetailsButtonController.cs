using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttributesDetailsButtonController : MonoBehaviour, ISelectHandler, IMoveHandler
{
    public SelectAttributesMenuHandler m_SelectAttributesMenuHandler;
    public PreviewScreen m_PreviewScreen;
    public AttributesDetailsWindowDatas m_AttributesDetailsWindowDatas;

    private readonly Dictionary<Language, string> _textContainer = new();
    
    private int _attributesMaxNumber;
    
    private int _currentSelection; // Detail Selection
    private int CurrentSelection
    {
        get => _currentSelection;
        set
        {
            if (0 <= value && value < _attributesMaxNumber)
            {
                _currentSelection = value;
                OnSelection(false);
                return;
            }
            _currentSelection = Mathf.Clamp(value, 0, _attributesMaxNumber - 1);
        }
    }


    private void Awake()
    {
        _attributesMaxNumber = m_AttributesDetailsWindowDatas.DetailsWindowElements.Length;
        _textContainer[Language.English] = m_AttributesDetailsWindowDatas.AttributeName;
        _textContainer[Language.Korean] = m_AttributesDetailsWindowDatas.NativeAttributeName;
    }

    private void OnEnable()
    {
        _currentSelection = PlayerManager.CurrentAttributes.GetAttributes(m_AttributesDetailsWindowDatas.AttributeType);
        OnSelection(true);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelection(true);
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

    private void OnSelection(bool transition)
    {
        AttributeType attributeType = m_AttributesDetailsWindowDatas.AttributeType;
        DetailsWindowElement data = m_AttributesDetailsWindowDatas.DetailsWindowElements[CurrentSelection];
        string attributeName = _textContainer[GameSetting.m_Language];
        
        m_PreviewScreen.UpdateTempAttributes(attributeType, CurrentSelection);
        m_SelectAttributesMenuHandler.SetAttributesDetailsInfo(data, transition);
        m_SelectAttributesMenuHandler.UpdateTotalCost(attributeType, data.cost);
        m_SelectAttributesMenuHandler.SetAttributeName(attributeName);
    }
}
