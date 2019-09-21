using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : PlayerMissile
{
    void FixedUpdate()
    {
        MoveVector();
    }

    protected override void OnStart()
    {
        m_Vector2 = transform.up * m_Speed;
    }
}
