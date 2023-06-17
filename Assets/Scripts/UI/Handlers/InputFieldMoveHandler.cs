using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputFieldMoveHandler : MonoBehaviour
{
    public static event Action<InputValue> Action_OnMove;

    public void OnMove(InputValue inputValue)
    {
        Action_OnMove?.Invoke(inputValue);
    }
}
