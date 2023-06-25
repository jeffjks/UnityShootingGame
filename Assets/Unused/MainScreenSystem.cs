using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreenSystem : MonoBehaviour
{
    private static MainScreenSystem Instance;

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
}
