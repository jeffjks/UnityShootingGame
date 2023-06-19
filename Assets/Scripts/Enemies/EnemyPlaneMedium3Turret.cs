using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium3Turret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }

    public void StartPattern() {
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        float target_angle, random_value;
        pos = m_FirePosition.position;
        target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
        random_value = Random.Range(-2f, 2f);

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 3; i++) {
                pos = m_FirePosition.position;
                CreateBullet(4, pos, 6f, target_angle + random_value, accel);
                yield return new WaitForMillisecondFrames(47);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition.position;
                CreateBullet(4, pos, 7f, target_angle + random_value, accel);
                yield return new WaitForMillisecondFrames(40);
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition.position;
                CreateBulletsSector(4, pos, 8f, target_angle + random_value, accel, 3, 12f);
                yield return new WaitForMillisecondFrames(35);
            }
        }
        yield break;
    }
}
