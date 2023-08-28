using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvasManager : MonoBehaviour
{
    private void Awake()
    {
        SystemManager.Action_OnQuitInGame += DestroySelf;
    }
    
    private void OnDestroy()
    {
        SystemManager.Action_OnQuitInGame -= DestroySelf;
    }
    
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
