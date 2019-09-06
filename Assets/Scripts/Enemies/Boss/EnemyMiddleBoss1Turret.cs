using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss1Turret : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    
    private bool m_Shooting = false;
    private byte m_Phase = 0;
    private IEnumerator m_CurrentPattern = null;

    void Start()
    {
        GetCoordinates();
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_ParentEnemy.m_Health <= 0) {
            OnDeath();
        }

        if (((EnemyMiddleBoss1) m_ParentEnemy).m_Phase == 0) {
            if (!m_Shooting) {
                RotateSlightly(m_PlayerPosition, 90f);
            }
        }
        else {
            if (m_Phase == 0) {
                m_Phase = 1;
                if (m_CurrentPattern != null)
                    StopCoroutine(m_CurrentPattern);
                m_CurrentPattern = Pattern2();
                StartCoroutine(m_CurrentPattern);
            }
        }

        if (m_Phase == 1) {
            RotateImmediately(m_CurrentAngle + 300f*transform.localScale.x * Time.deltaTime);
        }
        
        base.Update();
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(1.5f);

        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                m_Shooting = true;
                CreateBullet(4, m_FirePosition.position, 4f, m_CurrentAngle, accel);
                m_Shooting = false;
                yield return new WaitForSeconds(2f + Random.Range(0f, 1f));
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                m_Shooting = true;
                CreateBullet(4, m_FirePosition.position, 4.2f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.08f);
                CreateBullet(4, m_FirePosition.position, 4.2f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.08f);
                CreateBullet(4, m_FirePosition.position, 4.2f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.08f);
                CreateBullet(4, m_FirePosition.position, 4.2f, m_CurrentAngle, accel);
                m_Shooting = false;
                yield return new WaitForSeconds(1.2f + Random.Range(0f, 0.5f));
            }
        }
        else {
            while(true) {
                m_Shooting = true;
                CreateBullet(4, m_FirePosition.position, 5.6f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.06f);
                CreateBullet(4, m_FirePosition.position, 5.6f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.06f);
                CreateBullet(4, m_FirePosition.position, 5.6f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.06f);
                CreateBullet(4, m_FirePosition.position, 5.6f, m_CurrentAngle, accel);
                m_Shooting = false;
                yield return new WaitForSeconds(1f + Random.Range(0f, 0.4f));
            }
        }
    }

    private IEnumerator Pattern2()
    {
        if (m_SystemManager.m_Difficulty == 0) {
            EnemyBulletAccel accel = new EnemyBulletAccel(5.5f, 1f);
            while(true) {
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.2f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            EnemyBulletAccel accel = new EnemyBulletAccel(5.5f, 1f);
            while(true) {
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else {
            while(true) {
                EnemyBulletAccel accel1 = new EnemyBulletAccel(6.1f, 1f);
                EnemyBulletAccel accel2 = new EnemyBulletAccel(5f, 1f);
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle + 3f, accel1);
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle - 3f, accel2);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
