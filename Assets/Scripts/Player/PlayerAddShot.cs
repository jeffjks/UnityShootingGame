using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAddShot : PlayerMissile
{
    void Update()
    {
        MoveVector();
    }
    
    protected override void OnStart()
    {
        m_Vector2 = Vector2Int.up * m_Speed;
    }
}
