using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRocket : PlayerMissile
{
    [Space(10)]
    [SerializeField] private float m_MaxSpeed = 25f;
    private float m_CurrentSpeed;

    protected override void OnStart()
    {
        m_CurrentSpeed = m_Speed;
        m_Vector2 = transform.up * m_CurrentSpeed;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        m_CurrentSpeed = Mathf.MoveTowards(m_CurrentSpeed, m_MaxSpeed, 0.2f);
        m_Vector2 = transform.up * m_CurrentSpeed;
        
        MoveVector();
    }
}
