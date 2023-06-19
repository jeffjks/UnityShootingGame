using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalStateSystem : MonoBehaviour
{
    public static bool InCriticalState { get; private set; }
    private static int _remainingFrame;
    private static CriticalStateSystem Instance { get; set; }
    
    void Start()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void SetCriticalState(int frame)
    {
        if (frame < _remainingFrame || frame <= 0)
        {
            return;
        }

        InCriticalState = true;
        _remainingFrame = frame;
    }

    private void Update()
    {
        if (_remainingFrame > 0)
        {
            _remainingFrame--;
        }
        else if (InCriticalState)
        {
            InCriticalState = false;
        }
    }
}
