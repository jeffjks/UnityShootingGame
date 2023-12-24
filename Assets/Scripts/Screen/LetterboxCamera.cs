using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBoxCamera : MonoBehaviour
{
    private bool _destroySingleton;
    private static LetterBoxCamera Instance { get; set; }

    private void Awake()
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
