using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameInputController : MonoBehaviour
{
    public static event Action Action_OnPauseInput;
    public static event Action<InputValue> Action_OnFireInput;
    public static event Action Action_OnBombInput;
    public static event Action Action_OnEscapeInput;
    public static event Action<InputValue> Action_OnMove;

    public void OnPause()
    {
        Action_OnPauseInput?.Invoke();
    }
    
    public void OnFire(InputValue inputValue)
    {
        Action_OnFireInput?.Invoke(inputValue);
        //Debug.Log($"{inputValue.isPressed}, {inputValue.Get<float>()}");
    }
    
    public void OnBomb()
    {
        Action_OnBombInput?.Invoke();
    }
    
    public void OnEscape()
    {
        Action_OnEscapeInput?.Invoke();
    }

    public void OnMove(InputValue inputValue)
    {
        Action_OnMove?.Invoke(inputValue);
    }
}
