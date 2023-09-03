using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameInputService : MonoBehaviour
{
    public EventSystem m_EventSystem;
    
    private bool _destroySingleton;
    
    private static InGameInputService Instance { get; set; }

    private void Start()
    {
        if (Instance != null)
        {
            _destroySingleton = true;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        m_EventSystem.gameObject.SetActive(true);
        var pauseMenuHandler = FindObjectOfType<PauseMenuHandler>(true);
        var selectables = pauseMenuHandler.GetComponentsInChildren<Selectable>();
        m_EventSystem.firstSelectedGameObject = selectables[pauseMenuHandler.m_InitialSelection].gameObject;

        SystemManager.Action_OnQuitInGame += DestroySelf;
        SystemManager.Action_OnNextStage += DestroySelf;
        SystemManager.Action_OnFinishEndingCredit += DestroySelf;
        
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnDestroy()
    {
        if (_destroySingleton)
            return;
        SystemManager.Action_OnQuitInGame -= DestroySelf;
        SystemManager.Action_OnNextStage -= DestroySelf;
        SystemManager.Action_OnFinishEndingCredit -= DestroySelf;
    }

    private void DestroySelf()
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
