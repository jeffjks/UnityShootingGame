using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddShot : PlayerMissile
{
    protected override void OnStart()
    {
        m_Vector2 = Vector2Int.FloorToInt(transform.up * m_Speed);
    }

    void Update()
    {
        MoveVector();
        SetPosition();
    }
}
