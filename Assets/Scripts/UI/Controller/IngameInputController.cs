using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IngameInputController : MonoBehaviour
{
    public event Action Action_OnPauseInput;
    public event Action<InputValue> Action_OnFireInput;
    public event Action Action_OnBombInput;
    public event Action Action_OnEscapeInput;
    
    public static IngameInputController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SystemManager.Action_OnQuitInGame += DestroySelf;
        
        DontDestroyOnLoad(gameObject);
    }

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

    private void DestroySelf()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
