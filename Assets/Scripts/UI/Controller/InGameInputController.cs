using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameInputController : MonoBehaviour
{
    public event Action Action_OnPauseInput;
    public event Action<InputValue> Action_OnFireInput;
    public event Action Action_OnBombInput;
    public event Action Action_OnEscapeInput;
    public event Action<InputValue> Action_OnMove;
    
    public static InGameInputController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SystemManager.Action_OnQuitInGame += DestroySelf;
        SystemManager.Action_OnNextStage += DestroySelf;
        
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

    public void OnMove(InputValue inputValue)
    {
        Action_OnMove?.Invoke(inputValue);
    }

    public void DestroySelf()
    {
        Instance = null;
        Destroy(gameObject);
    }

    private void DestroySelf(bool hasNextStage)
    {
        if (!hasNextStage)
            DestroySelf();
    }
}
