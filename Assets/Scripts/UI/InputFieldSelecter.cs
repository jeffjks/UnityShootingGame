using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class InputFieldSelecter : MonoBehaviour
{
    public TMP_InputField[] m_InputFields;
    public InputSystemUIInputModule m_InputSystemUI;

    public void OnMove(InputValue inputValue)
    {
        Vector2 moveInput = inputValue.Get<Vector2>();

        foreach (var inputField in m_InputFields)
        {
            if (inputField.isFocused)
            {
                m_InputSystemUI.enabled = false;
                
                if (moveInput == Vector2.up)
                {
                    (inputField.navigation.selectOnUp)?.Select();
                    m_InputSystemUI.enabled = true;
                }
                else if (moveInput == Vector2.down)
                {
                    (inputField.navigation.selectOnDown)?.Select();
                    m_InputSystemUI.enabled = true;
                }
                return;
            }
        }
    }
}
