using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddShot : PlayerWeapon
{
    private Vector3 _savedPos;
    private int _currentPosFrame;
    private const int MAX_POS_FRAME = 2;
    
    public override void OnStart()
    {
        base.OnStart();

        CurrentAngle = m_MoveVector.direction;
        m_MoveVector.speed = m_Speed;
        
        _currentPosFrame = 1;
        _savedPos = transform.position;
        SetMissilePosition(_savedPos);
    }

    private void Update()
    {
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        SimplifyMissilePosition();
    }

    private void SimplifyMissilePosition()
    {
        if (_currentPosFrame >= MAX_POS_FRAME)
        {
            _savedPos = transform.position;
            _currentPosFrame = 0;
        }
        SetMissilePosition(_savedPos);
        _currentPosFrame++;
    }
}
