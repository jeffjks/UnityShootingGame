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
            if (moveInput == Vector2.up)
            {
                _inputField.DeactivateInputField();
            }
            else if (moveInput == Vector2.down)
            {
                _inputField.DeactivateInputField();
            }
        }
    }
}
