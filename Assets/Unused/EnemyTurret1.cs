using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret1 : EnemyUnit
{
    private void Start()
    {SetRotatePattern(new RotatePattern_MoveDirection());
    }
}
