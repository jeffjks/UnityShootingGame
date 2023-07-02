using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewScreen : MonoBehaviour
{
    public GameObject m_PlayerPreviewCamera;
    
    private ShipAttributes _tempAttributes;

    public event Action<ShipAttributes> Action_OnTempAttributesChanged;
    
    private void OnEnable()
    {
        if (m_PlayerPreviewCamera != null)
            m_PlayerPreviewCamera.SetActive(true);
        _tempAttributes = PlayerManager.CurrentAttributes;
    }

    private void OnDisable()
    {
        if (m_PlayerPreviewCamera != null)
            m_PlayerPreviewCamera.SetActive(false);
    }

    public void UpdateTempAttributes(AttributeType attributeType, int selection)
    {
        _tempAttributes.SetAttributes(attributeType, selection);
        Action_OnTempAttributesChanged?.Invoke(_tempAttributes);
    }
}
