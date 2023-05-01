using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddShot : PlayerWeapon
{
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
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        SetPosition2D();
    }
}
