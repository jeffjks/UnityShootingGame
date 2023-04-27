using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge1Turret1 : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[2];

    private sbyte m_State = -1;

    void Start()
    {
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();
        
        if (3 * m_ParentEnemy.m_Health <= m_ParentEnemy.m_MaxHealth) {
            m_EnemyHealth.OnDeath();
        }

        if (((EnemyTankLarge1) m_ParentEnemy).m_Phase == 1) {
            if (m_State == -1) {
                StartCoroutine(Pattern1());
            }
        }

        if (m_State == 0) {
            RotateSlightly(m_PlayerPosition, 100f, -48f);
        }
        else if (m_State == 1) {
            RotateSlightly(m_CurrentAngle + 20f, 90f);
        }
        else if (m_State == 2) {
            RotateSlightly(m_PlayerPosition, 100f, 48f);
        }
        else if (m_State == 3) {
            RotateSlightly(m_CurrentAngle - 20f, 90f);
        }
        else {
            RotateSlightly(m_PlayerPosition, 100f, -48f);
        }
    }

    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0f, 0);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 500);
        EnemyBulletAccel accel3 = new EnemyBulletAccel(7.4f, 800);

        while(true) {
            if (m_SystemManager.GetDifficulty() == 0) {
                m_State = 1;
                for (int i = 0; i < 7; i++) {
                    pos[0] = GetScreenPosition(m_FirePosition[0].position);
                    CreateBullet(5, pos[0], 6.5f, m_CurrentAngle, accel1);
                    yield return new WaitForMillisecondFrames(137);
                }
                m_State = 2;
                yield return new WaitForMillisecondFrames(900);
                for (int j = 0; j < 3; j++) {
                    pos[1] = GetScreenPosition(m_FirePosition[1].position);
                    CreateBullet(0, pos[1], Random.Range(5f, 12f), ((EnemyTankLarge1) m_ParentEnemy).m_MoveVector.direction + Random.Range(-20f, 20f), accel2,
                    2, 500, 0, 0.1f, BulletDirection.PLAYER, Random.Range(-25f, 25f), accel3);
                    yield return new WaitForMillisecondFrames(320);
                }
                yield return new WaitForMillisecondFrames(750);
                m_State = 3;
                for (int i = 0; i < 7; i++) {
                    pos[0] = GetScreenPosition(m_FirePosition[0].position);
                    CreateBullet(5, pos[0], 6.5f, m_CurrentAngle, accel1);
                    yield return new WaitForMillisecondFrames(137);
                }
                m_State = 0;
                yield return new WaitForMillisecondFrames(750);
                for (int j = 0; j < 3; j++) {
                    pos[1] = GetScreenPosition(m_FirePosition[1].position);
                    CreateBullet(0, pos[1], Random.Range(5f, 12f), ((EnemyTankLarge1) m_ParentEnemy).m_MoveVector.direction + Random.Range(-20f, 20f), accel2,
                    2, 500, 0, 0.1f, BulletDirection.PLAYER, Random.Range(-25f, 25f), accel3);
                    yield return new WaitForMillisecondFrames(320);
                }
                yield return new WaitForMillisecondFrames(900);
            }

            else if (m_SystemManager.GetDifficulty() == 1) {
                m_State = 1;
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 5; j++) {
                        pos[0] = GetScreenPosition(m_FirePosition[0].position);
                        CreateBullet(5, pos[0], 4.2f + j*0.8f, m_CurrentAngle - j*3f, accel1);
                    }
                    yield return new WaitForMillisecondFrames(96);
                }
                m_State = 2;
                yield return new WaitForMillisecondFrames(800);
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        pos[1] = GetScreenPosition(m_FirePosition[1].position);
                        CreateBullet(0, pos[1], Random.Range(5f, 12f), ((EnemyTankLarge1) m_ParentEnemy).m_MoveVector.direction + Random.Range(-20f, 20f), accel2,
                        2, 500, 0, 0.1f, BulletDirection.PLAYER, Random.Range(-25f, 25f), accel3);
                    }
                    yield return new WaitForMillisecondFrames(240);
                }
                yield return new WaitForMillisecondFrames(600);
                m_State = 3;
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 5; j++) {
                        pos[0] = GetScreenPosition(m_FirePosition[0].position);
                        CreateBullet(5, pos[0], 4.2f + j*0.8f, m_CurrentAngle + j*3f, accel1);
                    }
                    yield return new WaitForMillisecondFrames(96);
                }
                m_State = 0;
                yield return new WaitForMillisecondFrames(600);
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        pos[1] = GetScreenPosition(m_FirePosition[1].position);
                        CreateBullet(0, pos[1], Random.Range(5f, 12f), ((EnemyTankLarge1) m_ParentEnemy).m_MoveVector.direction + Random.Range(-20f, 20f), accel2,
                        2, 500, 0, 0.1f, BulletDirection.PLAYER, Random.Range(-25f, 25f), accel3);
                    }
                    yield return new WaitForMillisecondFrames(240);
                }
                yield return new WaitForMillisecondFrames(800);
            }

            else {
                m_State = 1;
                for (int i = 0; i < 12; i++) {
                    for (int j = 0; j < 6; j++) {
                        pos[0] = GetScreenPosition(m_FirePosition[0].position);
                        CreateBullet(5, pos[0], 4f + j*0.8f, m_CurrentAngle - j*3f, accel1);
                    }
                    yield return new WaitForMillisecondFrames(80);
                }
                m_State = 2;
                yield return new WaitForMillisecondFrames(800);
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        pos[1] = GetScreenPosition(m_FirePosition[1].position);
                        CreateBullet(0, pos[1], Random.Range(5f, 12f), ((EnemyTankLarge1) m_ParentEnemy).m_MoveVector.direction + Random.Range(-20f, 20f), accel2,
                        2, 500, 0, 0.1f, BulletDirection.PLAYER, Random.Range(-25f, 25f), accel3);
                    }
                    yield return new WaitForMillisecondFrames(240);
                }
                yield return new WaitForMillisecondFrames(600);
                m_State = 3;
                for (int i = 0; i < 12; i++) {
                    for (int j = 0; j < 6; j++) {
                        pos[0] = GetScreenPosition(m_FirePosition[0].position);
                        CreateBullet(5, pos[0], 4f + j*0.8f, m_CurrentAngle + j*3f, accel1);
                    }
                    yield return new WaitForMillisecondFrames(80);
                }
                m_State = 0;
                yield return new WaitForMillisecondFrames(600);
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        pos[1] = GetScreenPosition(m_FirePosition[1].position);
                        CreateBullet(0, pos[1], Random.Range(5f, 12f), ((EnemyTankLarge1) m_ParentEnemy).m_MoveVector.direction + Random.Range(-20f, 20f), accel2,
                        2, 500, 0, 0.1f, BulletDirection.PLAYER, Random.Range(-25f, 25f), accel3);
                    }
                    yield return new WaitForMillisecondFrames(240);
                }
                yield return new WaitForMillisecondFrames(800);
            }
        }
    }
}
