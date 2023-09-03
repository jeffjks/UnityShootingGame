using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewGemRenderer : MonoBehaviour
{
    public static OverviewGemRenderer Instance { get; private set; }
    
    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
}
