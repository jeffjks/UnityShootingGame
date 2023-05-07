using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRocket : PlayerWeapon
{
    [Space(10)]
    public float m_MaxSpeed;
    public float m_Accel;

    public override void OnStart()
    {
        base.OnStart();
        
        RotateImmediately(m_MoveVector.direction);
        m_MoveVector.speed = m_Speed;
        
        UpdateTransform();
        SetPosition2D();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (m_MoveVector.speed < m_MaxSpeed) {
            m_MoveVector.speed += m_Accel / Application.targetFrameRate * Time.timeScale;
        }
        else {
            m_MoveVector.speed = m_MaxSpeed;
        }
        
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        UpdateTransform();
        SetPosition2D();
    }
}
