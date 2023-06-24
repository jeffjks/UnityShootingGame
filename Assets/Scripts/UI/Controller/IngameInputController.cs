using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IngameInputController : MonoBehaviour
{
    public event Action Action_OnPause;
    public event Action<bool> Action_OnFireInput;
    public event Action Action_OnBombInput;
    
    public static IngameInputController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }

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
        Action_OnFireInput?.Invoke(inputValue.isPressed);
    }
    
    public void OnBomb()
    {
        Action_OnBombInput?.Invoke();
    }
}
