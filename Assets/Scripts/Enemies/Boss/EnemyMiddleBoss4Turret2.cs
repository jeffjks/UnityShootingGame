using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss4Turret2 : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition + new Vector2(0f, 1.5f));
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition + new Vector2(0f, 1.5f));
        else
            RotateSlightly(m_PlayerPosition + new Vector2(0f, 1.5f), 100f);
        
        base.Update();
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(1.5f);

        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                CreateBullet(4, m_FirePosition.position, 4f, m_CurrentAngle, accel);
                yield return new WaitForSeconds(2f + Random.Range(0f, 1f));
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                yield return new WaitForSeconds(1.2f + Random.Range(0f, 0.5f));
            }
        }
        else {
            while(true) {
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
            while(true) {
                yield return new WaitForSeconds(0.05f);
            }
        }
        else {
            while(true) {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
