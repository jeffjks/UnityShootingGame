using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonDisabler : MonoBehaviour
{
    public Button m_Button;
    
    void OnEnable()
    {
        if (DebugOption.NetworkAvailable)
        {
            SetButtonInteractable(GameManager.isOnline);
        }
    }

    private void SetButtonInteractable(bool state)
    {
        if (state)
        {
            return;
        }
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            (m_Button.navigation.selectOnDown)?.Select();
            m_Button.interactable = false;
        }
    }
}
