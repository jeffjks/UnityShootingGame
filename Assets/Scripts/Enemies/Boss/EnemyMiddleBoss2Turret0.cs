using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2Turret0 : EnemyUnit
{
    public float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        GetCoordinates();
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 50f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    private IEnumerator Pattern1()
    {
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(2.5f);

        while (true) {
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ((EnemyMiddleBoss2) m_ParentEnemy).ToPhase1();
        Destroy(gameObject);
        yield return null;
    }
}
