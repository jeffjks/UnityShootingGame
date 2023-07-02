using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4FrontTurret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    [HideInInspector] public byte m_RotatePattern = 10;
    private int m_KillScore = 0;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
        m_KillScore = m_Score;
        m_Score = 0;
        
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_RotatePattern == 10) {
            if (PlayerManager.IsPlayerAlive)
                RotateSlightly(PlayerManager.GetPlayerPosition(), 180f);
            else
                RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
        }
        else if (m_RotatePattern == 20) {
            RotateSlightly(0f, 150f);
        }
    }

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        else if (num == 3)
            m_CurrentPattern = Pattern3();
        else
            return;
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
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 4f, CurrentAngle, accel);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 4.1f, CurrentAngle, accel, 3, 14f);
        }
        else {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 3.5f, CurrentAngle, accel, 2, 9f);
            CreateBulletsSector(0, pos, 4.1f, CurrentAngle, accel, 3, 14f);
        }
        yield break;
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos0, pos1, pos2;
        float gap = 0.6f;
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(1, pos0, 5.4f, CurrentAngle, accel);
                CreateBullet(1, pos1, 5.4f, CurrentAngle, accel);
                CreateBullet(1, pos2, 5.4f, CurrentAngle, accel);
        }
        else {
            for (int i = 0; i < 5; i++) {
                if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                    pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                    CreateBullet(1, pos0, 5f+i*0.8f, CurrentAngle, accel);
                    CreateBullet(1, pos1, 5f+i*0.8f, CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f+i*0.8f, CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(50);
                }
                else {
                    pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                    pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                    CreateBullet(1, pos0, 5f+i*0.8f, CurrentAngle, accel);
                    CreateBullet(1, pos1, 5f+i*0.8f, CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f+i*0.8f, CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(20);
                }
            }
        }
        yield break;
    }

    private IEnumerator Pattern3()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 6.5f, CurrentAngle, accel);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 6.3f, CurrentAngle, accel);
            CreateBullet(0, pos, 6.9f, CurrentAngle, accel);
        }
        else {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBullet(0, pos, 6f, CurrentAngle, accel);
            CreateBullet(0, pos, 6.6f, CurrentAngle, accel);
            CreateBullet(0, pos, 7.2f, CurrentAngle, accel);
        }
        yield break;
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_Score = m_KillScore;
        }
    }
}