using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameInputController : MonoBehaviour
{
    public static event Action<bool> Action_OnPauseInput;
    public static event Action<bool> Action_OnFireInput;
    public static event Action<bool> Action_OnBombInput;
    public static event Action<bool> Action_OnEscapeInput;
    public static event Action<Vector2Int> Action_OnMove;

    public void OnPause()
    {
        Action_OnPauseInput?.Invoke(true);
    }
    
    public void OnFire(InputValue inputValue)
    {
        Action_OnFireInput?.Invoke(inputValue.isPressed);
        //Debug.Log($"{inputValue.isPressed}, {inputValue.Get<float>()}");
    }
    
    public void OnBomb(InputValue inputValue)
    {
        Action_OnBombInput?.Invoke(inputValue.isPressed);
    }
    
    public void OnEscape(InputValue inputValue)
    {
        Action_OnEscapeInput?.Invoke(inputValue.isPressed);
    }

    public void OnMove(InputValue inputValue)
    {
        var inputVector = inputValue.Get<Vector2>();
        var rawInputVector = new Vector2Int(Math.Sign(inputVector.x), Math.Sign(inputVector.y));
        Action_OnMove?.Invoke(rawInputVector);
    }
}
