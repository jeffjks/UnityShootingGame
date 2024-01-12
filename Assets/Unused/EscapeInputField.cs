using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// UNUSED SCRIPT

public class EscapeInputField : MonoBehaviour, ISelectHandler
{
    public EventSystem m_EventSystem;
    
    public void OnSelect(BaseEventData eventData)
    {
        if (!m_EventSystem.sendNavigationEvents)
            m_EventSystem.sendNavigationEvents = true;
    }
}
