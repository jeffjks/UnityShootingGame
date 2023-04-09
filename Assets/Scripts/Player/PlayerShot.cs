using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : PlayerMissile
{
    void Update()
    {
        MoveVector();
        SetPosition();
    }

    public override void OnStart() {
        base.OnStart();
        m_Vector2 = Vector2Int.FloorToInt(transform.up * m_Speed);
    }
}
