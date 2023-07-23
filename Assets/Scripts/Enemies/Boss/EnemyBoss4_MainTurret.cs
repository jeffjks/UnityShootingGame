using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4_MainTurret : EnemyUnit
{
    public EnemyBoss4_MainTurretBarrel m_EnemyBoss4MainTurretBarrel; // TODO. 애니메이션 설정
    
    private IEnumerator _subPattern1, _subPattern2;

    void Start()
    {
        CurrentAngle = AngleToPlayer;
        SetRotatePattern(new RotatePattern_TargetPlayer(120f, 100f));
    }
}