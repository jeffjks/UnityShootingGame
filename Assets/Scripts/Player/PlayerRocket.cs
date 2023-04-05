using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRocket : PlayerMissile
{
    [Space(10)]
    public int m_MaxSpeed;
    public float m_Accel;
    private float m_CurrentSpeed;

    protected override void OnStart()
    {
        m_CurrentSpeed = m_Speed;
        m_Vector2 = Vector2Int.up * (int) m_CurrentSpeed;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (m_CurrentSpeed < m_MaxSpeed) {
            m_CurrentSpeed += m_Accel / Application.targetFrameRate * Time.timeScale;
        }
        else {
            m_CurrentSpeed = m_MaxSpeed;
        }
        m_Vector2 = Vector2Int.up * (int) m_CurrentSpeed;
        
        MoveVector();
        SetPosition();
    }
}
