using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipSmall2 : EnemyUnit
{
    protected override void Update()
    {
        RotateImmediately(m_MoveVector.direction);
        base.Update();
    }
}
