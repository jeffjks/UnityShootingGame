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

    protected override void OnStart()
    {
        m_Vector2 = Vector2Int.up * m_Speed;
    }
}
