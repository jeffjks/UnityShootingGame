using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2 : EnemyUnit
{
    [SerializeField] private EnemyUnit[] m_Turret = new EnemyUnit[4];
    [SerializeField] private Transform m_FirePosition = null;
    private byte m_Phase = 0;

    void Start() {
        m_MoveVector = new MoveVector(0.8f, 0f);
    }
    
    protected override void Update()
    {
        if (m_Phase == 0) {
            if (m_Position2D.y < - 1f) {
                StartCoroutine(Pattern1());
                m_Phase = 1;
            }
        }

        base.Update();
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float target_angle;
        yield return new WaitForSeconds(1f);

        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                break;
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                for (int i = 0; i < 25; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(4, pos, 10f, target_angle, accel, 2, 115f - i*4f);
                    yield return new WaitForSeconds(0.02f);
                }
            }
            else {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                for (int i = 0; i < 25; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(4, pos, 9.5f, target_angle, accel, 2, 120f - i*4f);
                    CreateBulletsSector(4, pos, 11f, target_angle, accel, 2, 100f - i*3.6f);
                    yield return new WaitForSeconds(0.02f);
                }
            }
            yield return new WaitForSeconds(3f);
        }
        yield return null;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        for (int i = 0; i < m_Turret.Length; i++) {
            if (m_Turret[i] != null)
                m_Turret[i].OnDeath();
        }
        m_SystemManager.EraseBullets(1f);

        ExplosionEffect(0, -1, new Vector2(-1.5f, 2.5f));
        ExplosionEffect(1, -1, new Vector2(0f, 2.5f));
        ExplosionEffect(0, -1, new Vector2(1.5f, 2.5f));
        ExplosionEffect(0, -1, new Vector2(-1.5f, -3.8f));
        ExplosionEffect(1, -1, new Vector2(0f, -3.8f));
        ExplosionEffect(0, -1, new Vector2(1.5f, -3.8f));
        Destroy(gameObject);
        yield break;
    }
}
