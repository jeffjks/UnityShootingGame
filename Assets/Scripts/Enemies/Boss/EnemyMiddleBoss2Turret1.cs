using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2Turret1 : EnemyUnit
{
    public Transform m_FirePosition;
    private int[] m_FireDelay = { 2000, 1500, 1250 };
    
    private IEnumerator m_CurrentPattern;
    private int _killScore;

    void Start()
    {
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateUnit(AngleToPlayer);
        _killScore = m_Score;
        m_Score = 0;
        
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateUnit(AngleToPlayer);
        else
            RotateUnit(AngleToPlayer, 180f);
    }

    private IEnumerator Pattern1()
    {
        Vector3 pos1, pos2;
        BulletAccel accel = new BulletAccel(0f, 0);
        float gap = 0.25f;
        yield return new WaitForMillisecondFrames(2500);

        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                CreateBullet(1, pos1, 5.5f, CurrentAngle, accel);
                CreateBullet(1, pos2, 5.5f, CurrentAngle, accel);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 4; i++) {
                    pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                    pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                    CreateBullet(1, pos1, 5f + i*0.9f, CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f + i*0.9f, CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            else {
                for (int i = 0; i < 6; i++) {
                    pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(Vector3.right * gap));
                    pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(Vector3.left * gap));
                    CreateBullet(1, pos1, 5f + i*0.8f, CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f + i*0.8f, CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(50);
                }
            }
            
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_Score = _killScore;
        }
    }
}
