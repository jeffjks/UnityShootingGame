using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRocket : PlayerMissile
{
    [Space(10)]
    [SerializeField] private int m_MaxSpeed = 6400;
    [SerializeField] private int m_Accel = 51;
    private int m_CurrentSpeed;

    protected override void OnStart()
    {
        m_CurrentSpeed = m_Speed;
        m_Vector2 = Vector2Int.up * m_CurrentSpeed;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        if (m_CurrentSpeed < m_MaxSpeed) {
            m_CurrentSpeed += m_Accel;
        }
        else {
            m_CurrentSpeed = m_MaxSpeed;
        }
        m_Vector2 = Vector2Int.up * m_CurrentSpeed;
        
        MoveVector();
        SetPosition();
    }
}
