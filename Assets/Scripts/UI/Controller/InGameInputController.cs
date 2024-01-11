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
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        Action_OnFireInput?.Invoke(inputValue.isPressed);
    }
    
    public void OnBomb(InputValue inputValue)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        Action_OnBombInput?.Invoke(inputValue.isPressed);
    }
    
    public void OnEscape(InputValue inputValue)
    {
        Action_OnEscapeInput?.Invoke(inputValue.isPressed);
    }

    public void OnMove(InputValue inputValue)
    {
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        var inputValueVector = inputValue.Get<Vector2>();
        var inputVector = new Vector2Int(Math.Sign(inputValueVector.x), Math.Sign(inputValueVector.y));
        Action_OnMove?.Invoke(inputVector);
    }
}
