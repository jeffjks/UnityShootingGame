using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonDisablerRanking : MonoBehaviour
{
    public EventSystem m_EventSystem;
    public Button m_Button;
    
    private GameManager m_GameManager;

    void Awake()
    {
        m_GameManager = GameManager.instance_gm;
    }
    
    void OnEnable()
    {
        if (m_GameManager.m_NetworkAvailable)
        {
            SetButtonInteractable(m_GameManager.m_IsOnline);
        }
    }

    private void SetButtonInteractable(bool state)
    {
        if (m_EventSystem.currentSelectedGameObject == gameObject)
        {
            m_Button.interactable = state;
        }
    }
}
