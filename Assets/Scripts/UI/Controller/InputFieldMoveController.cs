using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputFieldMoveController : MonoBehaviour
{
    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        InputFieldMoveHandler.Action_OnMove += OnMoveInput;
    }

    private void OnDisable()
    {
        InputFieldMoveHandler.Action_OnMove -= OnMoveInput;
    }

    private void OnMoveInput(InputValue inputValue)
    {
        Vector2 moveInput = inputValue.Get<Vector2>();
        
        if (_inputField.isFocused)
        {
            EventSystem.current.currentInputModule.enabled = false;
                
            if (moveInput == Vector2.up)
            {
                if (_inputField.navigation.selectOnUp != null)
                    _inputField.navigation.selectOnUp.Select();
                EventSystem.current.currentInputModule.enabled = true;
            }
            else if (moveInput == Vector2.down)
            {
                if (_inputField.navigation.selectOnDown != null)
                    _inputField.navigation.selectOnDown.Select();
                EventSystem.current.currentInputModule.enabled = true;
            }
        }
    }
}
