using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipSmall1 : EnemyUnit
{
    protected override void FixedUpdate()
    {
        RotateImmediately(m_MoveVector.direction);
        base.FixedUpdate();
    }
}
