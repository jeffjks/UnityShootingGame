using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelOpen : MonoBehaviour
{
    public Animator m_Animator;
    public int m_Timer;

    void Start()
    {
        m_Animator.speed = 0f;
        StartCoroutine(OpenGate());
    }
    
    private IEnumerator OpenGate() {
        yield return new WaitForMillisecondFrames(m_Timer);
        m_Animator.speed = 1f;
    }
}
