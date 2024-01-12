using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UNUSED SCRIPT

// ============================================================================================ //

public abstract class EnemyTurret : EnemyUnit // 적 개체, 포탑 (적 총알 제외)
{
    private IEnumerator m_CurrentPattern;

    public abstract void StartPattern(int num);
    public abstract void StopPattern();
}