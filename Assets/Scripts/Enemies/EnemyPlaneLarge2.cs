using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge2 : EnemyUnit
{
    public EnemyUnit[] m_Turret = new EnemyUnit[4];
    public Transform m_FirePosition;
    private int m_Phase;

    void Start() {
        m_MoveVector = new MoveVector(0.8f, 0f);
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 0) {
            if (m_Position2D.y < - 1f) {
                StartCoroutine(Pattern1());
                m_Phase = 1;
            }
        }
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        float target_angle;
        yield return new WaitForMillisecondFrames(1000);

        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                break;
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                for (int i = 0; i < 12; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(4, pos, 10f, target_angle, accel, 2, 115f - i*4.8f);
                    yield return new WaitForFrames(3);
                }
            }
            else {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                for (int i = 0; i < 12; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(4, pos, 9.5f, target_angle, accel, 2, 120f - i*5.3f);
                    CreateBulletsSector(4, pos, 11f, target_angle, accel, 2, 100f - i*4.8f);
                    yield return new WaitForFrames(3);
                }
            }
            yield return new WaitForMillisecondFrames(3000);
        }
        yield break;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        for (int i = 0; i < m_Turret.Length; i++) {
            if (m_Turret[i] != null)
                m_Turret[i].m_EnemyDeath.OnDying();
        }
        BulletManager.SetBulletFreeState(1000);
        
        yield break;
    }
}
