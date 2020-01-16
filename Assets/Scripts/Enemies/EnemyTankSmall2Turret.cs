using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankSmall2Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    private bool m_Shooting = false;

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1());
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (2 * m_ParentEnemy.m_Health <= m_ParentEnemy.m_MaxHealth) {
            OnDeath();
        }

        if (m_PlayerManager.m_PlayerIsAlive) {
            if (!m_Shooting)
                RotateSlightly(m_PlayerPosition, 72f);
        }
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(Random.Range(0f, 0.5f));
        while(true) {
            float target_angle = Mathf.Floor((m_CurrentAngle + 5f)/10f) * 10f;
            m_Shooting = true;
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 6f, target_angle, accel);
            yield return new WaitForSeconds(0.13f);
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 6f, target_angle, accel);
            yield return new WaitForSeconds(0.13f);
            pos = GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 6f, target_angle, accel);
            m_Shooting = false;
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}
