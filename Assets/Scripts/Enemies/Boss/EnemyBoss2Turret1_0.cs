using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret1_0 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    public Transform m_FirePosition;

    [HideInInspector] public bool m_InPattern = false;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
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
        float duration = 0.8f;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(3f, duration);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(7.6f, 0.6f);
        Vector3 pos;
        m_InPattern = true;

        if (m_SystemManager.m_Difficulty == 0) {
            int[] num = { 1, 2, 3, 3};
            for (int k = 0; k < 3; k++) {
                for (int i = 0; i < num[k]; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    for (int j = 0; j < 7; j++) {
                        CreateBullet(0, pos, 13.7f, m_CurrentAngle - 18f*3.5f + j*18f, accel1, BulletType.ERASE_AND_CREATE, duration,
                        3, 3f, BulletDirection.CURRENT, Random.Range(-6f, 6f), accel2);
                    }
                    yield return new WaitForSeconds(0.3f);
                }
                yield return new WaitForSeconds(1.5f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            int[] num = { 3, 5, 12 };
            for (int k = 0; k < 3; k++) {
                for (int i = 0; i < num[k]; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    for (int j = 0; j < 13; j++) {
                        CreateBullet(0, pos, 13.7f, m_CurrentAngle - 12f*6.5f + j*12f, accel1, BulletType.ERASE_AND_CREATE, duration,
                        3, 3f, BulletDirection.CURRENT, Random.Range(-7f, 7f), accel2);
                    }
                    yield return new WaitForSeconds(0.2f);
                }
                yield return new WaitForSeconds(1.2f);
            }
        }
        else {
            int[] num = { 3, 5, 12 };
            for (int k = 0; k < 3; k++) {
                for (int i = 0; i < num[k]; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    for (int j = 0; j < 15; j++) {
                        CreateBullet(0, pos, 13.7f, m_CurrentAngle - 10f*7.5f + j*10f, accel1, BulletType.ERASE_AND_CREATE, 1f,
                        3, 3f, BulletDirection.CURRENT, Random.Range(-8f, 8f), accel2);
                    }
                    yield return new WaitForSeconds(0.2f);
                }
                yield return new WaitForSeconds(0.9f);
            }
        }
        m_InPattern = false;
        yield break;
    }
}