using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret1_2 : EnemyUnit
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

    public void StartPattern(byte num, bool side = true) {
        if (num == 0)
            m_CurrentPattern = Pattern0();
        else if (num == 1)
            m_CurrentPattern = Pattern1(side);
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern0() {
        yield break;
        //BulletAccel accel = new BulletAccel(0f, 0);
        //Vector3 pos = m_FirePosition.position;
        /*
        while (true) {
            if (!m_ParentEnemy.m_IsUnattackable) {
                if (PlayerManager.GetPlayerPosition().y >= -7f) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(0, pos, 6.6f, CurrentAngle, accel, 12, 2.5f);
                }
            }
            yield return new WaitForMillisecondFrames(500);
        }*/
    }

    private IEnumerator Pattern1(bool side)
    {
        BulletAccel accel = new BulletAccel(5f, 500);
        Vector3 pos = m_FirePosition.position;
        
        if (side)
            yield return new WaitForMillisecondFrames(1500);
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, CurrentAngle, accel, 3, 32f);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, CurrentAngle, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 20f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 20f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 40f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 40f, accel, 8, 0.8f);
            yield return new WaitForMillisecondFrames(3000);
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 10f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 10f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 30f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 30f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 50f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 50f, accel, 8, 0.8f);
        }
        else {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, CurrentAngle, accel, 10, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 20f, accel, 10, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 20f, accel, 10, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 40f, accel, 10, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 40f, accel, 10, 0.8f);
            yield return new WaitForMillisecondFrames(3000);
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 10f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 10f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 30f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 30f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle - 50f, accel, 8, 0.8f);
            CreateBulletsSector(0, pos, 2f, CurrentAngle + 50f, accel, 8, 0.8f);
        }
        yield break;
    }
}