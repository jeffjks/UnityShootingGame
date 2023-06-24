using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : PlayerObject
{
    public bool m_IsPreviewObject;
    
    public bool SlowMode { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsShooting { get; set; }

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