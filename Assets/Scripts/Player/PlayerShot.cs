using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : PlayerWeapon
{
    public override void OnStart()
    {
        base.OnStart();
        
        CurrentAngle = m_MoveVector.direction;
        m_MoveVector.speed = m_Speed;
    }

    private void Update()
    {
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
    }
}
