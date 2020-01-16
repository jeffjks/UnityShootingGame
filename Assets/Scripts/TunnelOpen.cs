using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelOpen : MonoBehaviour
{
    public Animator m_Animator;
    public float m_Timer;

    void Start()
    {
        m_Animator.speed = 0f;
        Invoke("OpenGate", m_Timer);
    }
    
    private void OpenGate() {
        m_Animator.speed = 1f;
    }
}
