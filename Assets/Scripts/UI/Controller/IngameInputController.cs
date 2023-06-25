using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IngameInputController : MonoBehaviour
{
    public event Action Action_OnPause;
    public event Action<InputValue> Action_OnFireInput;
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

    public void OnPause()
    {
        Action_OnPause?.Invoke();
    }
    
    public void OnFire(InputValue inputValue)
    {
        if (!PlayerUnit.IsControllable)
            return;
        Action_OnFireInput?.Invoke(inputValue);
        //Debug.Log($"{inputValue.isPressed}, {inputValue.Get<float>()}");
    }
    
    public void OnBomb()
    {
        if (!PlayerUnit.IsControllable)
            return;
        Action_OnBombInput?.Invoke();
    }
}
