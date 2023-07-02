using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret1_1 : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    [HideInInspector] public bool m_InPattern = false;

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
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        m_InPattern = true;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 2; i++) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 3.5f, CurrentAngle, accel, 11, 17f);
                CreateBulletsSector(4, pos, 4.5f, CurrentAngle, accel, 6, 17f);
                yield return new WaitForMillisecondFrames(2400);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 3; i++) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 3.3f, CurrentAngle, accel, 17, 11f);
                CreateBulletsSector(4, pos, 4f, CurrentAngle, accel, 14, 11f);
                CreateBulletsSector(4, pos, 4.7f, CurrentAngle, accel, 11, 11f);
                yield return new WaitForMillisecondFrames(1600);
            }
        }
        else {
            for (int i = 0; i < 3; i++) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 3.3f, CurrentAngle, accel, 24, 8f);
                CreateBulletsSector(4, pos, 4f, CurrentAngle, accel, 19, 8f);
                CreateBulletsSector(4, pos, 4.7f, CurrentAngle, accel, 16, 8f);
                yield return new WaitForMillisecondFrames(1600);
            }
        }
        m_InPattern = false;
        yield break;
    }
}