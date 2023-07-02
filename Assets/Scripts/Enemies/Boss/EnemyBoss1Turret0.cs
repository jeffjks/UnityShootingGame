using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss1Turret0 : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateImmediately(PlayerManager.GetPlayerPosition());
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
    }

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos = m_FirePosition.position;
        CreateBulletsSector(0, pos, 7f, CurrentAngle + Random.Range(-2f, 2f), accel, 7, 14f);
        yield break;
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        float gap = 0.03f;
        pos = m_FirePosition.position;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 6; i++) {
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle - 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle + 3f, accel);
                yield return new WaitForMillisecondFrames(60);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 6; i++) {
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle - 14f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle - 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle + 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle + 14f, accel);
                yield return new WaitForMillisecondFrames(60);
            }
        }
        else {
            for (int i = 0; i < 6; i++) {
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle - 18f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle - 10f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle - 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle + 3f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle + 10f, accel);
                CreateBullet(3, m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)), 5f + 0.7f*i, CurrentAngle + 18f, accel);
                if (i == 5) {
                    for (int j = -1; j < 2; j += 2) {
                        pos = m_FirePosition.position;
                        CreateBullet(3, pos, 9f, CurrentAngle - 26f*j, accel);
                        CreateBullet(3, pos, 8.4f, CurrentAngle - 34f*j, accel);
                        CreateBullet(3, pos, 7.8f, CurrentAngle - 43f*j, accel);
                        CreateBullet(3, pos, 7.1f, CurrentAngle - 47f*j, accel);
                        CreateBullet(3, pos, 6.3f, CurrentAngle - 52f*j, accel);
                    }
                }
            yield return new WaitForMillisecondFrames(60);
            }
        }
        yield break;
    }
}