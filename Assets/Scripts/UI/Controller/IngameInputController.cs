using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IngameInputController : MonoBehaviour
{
    public event Action Action_OnPause;
    public event Action Action_OnFireInput;
    public event Action Action_OnBombInput;
    
    public void OnPause(InputValue inputValue)
    {
        if (!inputValue.isPressed)
        {
            return;
        }
        Action_OnPause?.Invoke();
    }
    
    public void OnFire(InputValue inputValue)
    {
        if (!inputValue.isPressed)
        {
            return;
        }
        Action_OnFireInput?.Invoke();
    }
    
    public void OnBomb(InputValue inputValue)
    {
        if (!inputValue.isPressed)
        {
            return;
        }
        Action_OnBombInput?.Invoke();
    }
}
