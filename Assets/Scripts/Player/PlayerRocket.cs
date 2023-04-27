using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRocket : PlayerWeapon
{
    [Space(10)]
    public int m_MaxSpeed;
    public float m_Accel;

    public override void OnStart()
    {
        base.OnStart();
        
        RotateImmediately(m_MoveVector.direction);
        m_MoveVector.speed = m_Speed;
        
        UpdateTransform();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (m_Speed < m_MaxSpeed) {
            m_Speed += m_Accel / Application.targetFrameRate * Time.timeScale;
        }
        else {
            m_Speed = m_MaxSpeed;
        }
        
        m_MoveVector.speed = m_Speed;
        
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        UpdateTransform();
    }
}
