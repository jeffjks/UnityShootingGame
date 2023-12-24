using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewGemRenderer : MonoBehaviour
{
    private bool _destroySingleton;
    private static OverviewGemRenderer Instance { get; set; }
    
    private void Start()
    {
        if (Instance != null)
        {
            _destroySingleton = true;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);

        SystemManager.Action_OnQuitInGame += DestroySelf;
    }
    
    private void OnDestroy()
    {
        if (_destroySingleton)
            return;
        Instance = null;
        SystemManager.Action_OnQuitInGame -= DestroySelf;
    }

    private void DestroySelf()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
