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
        Instance = this;
    }

    public static void SetCriticalState(int frame)
    {
        if (frame < _remainingFrame)
        {
            return;
        }
        if (frame == 0)
        {
            return;
        }

        InCriticalState = true;
        _remainingFrame = frame;
        Instance.StartCoroutine(RunCriticalState());
    }

    private static IEnumerator RunCriticalState()
    {
        while (_remainingFrame > 0)
        {
            _remainingFrame--;
            yield return null;
        }

        InCriticalState = false;
    }
}
