using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBoss4_Launcher : EnemyUnit
{
    private int _side;
    private int _moveDirection;
    public float CustomDirectionDelta { get; set; }
    public int CustomDirectionSide { get; set; }

    private void Start()
    {
        m_CustomDirection = new CustomDirection();
        
        _side = transform.localPosition.x < 0f ? -1 : 1;
        _moveDirection = _side;
        CustomDirectionSide = Random.Range(0, 2) * 2 - 1;
    }

    protected override void Update() {
        base.Update();
        
        if (Time.timeScale == 0)
            return;

        MoveSide();
    }

    private void MoveSide()
    {
        m_CustomDirection[0] += CustomDirectionDelta * CustomDirectionSide / Application.targetFrameRate * Time.timeScale;
    }
}