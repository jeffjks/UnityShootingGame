using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret2 : EnemyUnit
{
    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);
    }
}
