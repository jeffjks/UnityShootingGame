using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : PlayerObject
{
    [HideInInspector] public bool m_SlowMode = false;

    private Vector2Int _positionInt2D;
    public Vector2Int m_PositionInt2D
    {
        get => _positionInt2D;
        set {
            _positionInt2D = value;
            transform.position = new Vector3((float) _positionInt2D.x / 256, (float) _positionInt2D.y / 256, Depth.PLAYER);
        }
    }
}