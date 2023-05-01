using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : PlayerObject
{
    [HideInInspector] public bool m_SlowMode = false;

    private Vector2Int positionInt2D;
    public Vector2Int m_PositionInt2D {
        get { return positionInt2D; }
        set {
            positionInt2D = value;
            transform.position = new Vector3((float) positionInt2D.x / 256, (float) positionInt2D.y / 256, transform.position.z);
        }
    }
}