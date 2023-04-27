using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss1Turret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private bool m_Shooting = false;
    private int m_Phase;
    private IEnumerator m_CurrentPattern;
    private byte m_RotateState;

    void Start()
    {
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_ParentEnemy.m_Health <= 0) {
            m_EnemyHealth.OnDeath();
        }

        if (m_RotateState == 0) {
            if (!m_Shooting) {
                RotateSlightly(m_PlayerPosition, 90f);
            }
        }
        else if (m_RotateState == 1) {
            RotateImmediately(m_CurrentAngle + 300f*transform.localScale.x / Application.targetFrameRate * Time.timeScale);
        }
    }

    public void StartPattern() {
        m_CurrentPattern = Pattern2();
        StartCoroutine(m_CurrentPattern);
        m_RotateState = 1;
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(1500);

        if (m_SystemManager.GetDifficulty() == 0) {
            while(true) {
                m_Shooting = true;
                CreateBullet(4, m_FirePosition.position, 4f, m_CurrentAngle, accel);
                m_Shooting = false;
                yield return new WaitForMillisecondFrames(2000 + Random.Range(0, 1000));
            }
        }
        else if (m_SystemManager.GetDifficulty() == 1) {
            while(true) {
                m_Shooting = true;
                CreateBullet(4, m_FirePosition.position, 4.2f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(80);
                CreateBullet(4, m_FirePosition.position, 4.2f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(80);
                CreateBullet(4, m_FirePosition.position, 4.2f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(80);
                CreateBullet(4, m_FirePosition.position, 4.2f, m_CurrentAngle, accel);
                m_Shooting = false;
                yield return new WaitForMillisecondFrames(1200 + Random.Range(0, 500));
            }
        }
        else {
            while(true) {
                m_Shooting = true;
                CreateBullet(4, m_FirePosition.position, 5.6f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(60);
                CreateBullet(4, m_FirePosition.position, 5.6f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(60);
                CreateBullet(4, m_FirePosition.position, 5.6f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(60);
                CreateBullet(4, m_FirePosition.position, 5.6f, m_CurrentAngle, accel);
                m_Shooting = false;
                yield return new WaitForMillisecondFrames(1000 + Random.Range(0, 400));
            }
        }
    }

    private IEnumerator Pattern2()
    {
        if (m_SystemManager.GetDifficulty() == 0) {
            EnemyBulletAccel accel = new EnemyBulletAccel(5.5f, 1000);
            while(true) {
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(200);
            }
        }
        else if (m_SystemManager.GetDifficulty() == 1) {
            EnemyBulletAccel accel = new EnemyBulletAccel(5.5f, 1000);
            while(true) {
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(50);
            }
        }
        else {
            while(true) {
                EnemyBulletAccel accel1 = new EnemyBulletAccel(6.1f, 1000);
                EnemyBulletAccel accel2 = new EnemyBulletAccel(5f, 1000);
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle + 3f, accel1);
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle - 3f, accel2);
                yield return new WaitForMillisecondFrames(50);
            }
        }
    }
}
