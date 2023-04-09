using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddShot : PlayerMissile
{
    public override void OnStart() {
        base.OnStart();
        m_Vector2 = Vector2Int.FloorToInt(transform.up * m_Speed);
    }

    void Update()
    {
        MoveVector();
        SetPosition();
    }
}
