using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret2_0 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    public Transform m_FirePosition;

    [HideInInspector] public bool m_InPattern = false;
    
    private IEnumerator m_CurrentPattern;
    private float m_Direction;

    void Start()
    {
        GetCoordinates();
    }

    protected override void FixedUpdate()
    {
        m_Direction += 80f * Time.fixedDeltaTime;
        if (m_Direction > 360f) {
            m_Direction -= 360f;
        }

        base.FixedUpdate();
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
        float duration = 0.6f;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(3f, duration);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(6.1f, 0.5f); // Normal
        EnemyBulletAccel accel3 = new EnemyBulletAccel(7f, 0.5f); // Expert, Hell
        Vector3 pos;
        int side = Random.Range(0, 2);
        m_InPattern = true;

        if (side == 0)
            side = -1;

        while (true) {
            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 7; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(0, pos, 13f, m_Direction*side, accel1, 10, 36f, BulletType.ERASE_AND_CREATE, duration,
                    1, 3f, BulletDirection.CURRENT, 30f*side, accel2);
                    yield return new WaitForSeconds(0.6f);
                }
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 9; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(0, pos, 13f, m_Direction*side, accel1, 20, 18f, BulletType.ERASE_AND_CREATE, duration,
                    1, 3f, BulletDirection.CURRENT, 30f*side, accel3);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            else {
                for (int i = 0; i < 11; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(0, pos, 13f, m_Direction*side, accel1, 24, 15f, BulletType.ERASE_AND_CREATE, duration,
                    1, 3f, BulletDirection.CURRENT, 30f*side, accel3);
                    yield return new WaitForSeconds(0.22f);
                }
            }
            side *= -1;
        }
    }

    private IEnumerator Pattern2()
    {
        float duration = 0.6f;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(3f, duration);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(6.7f, 0.5f);
        Vector3 pos;
        int side = Random.Range(0, 2);
        m_InPattern = true;

        if (side == 0)
            side = -1;

        while (true) {
            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 7; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 13f, m_Direction*side, accel1, 12, 30f, BulletType.ERASE_AND_CREATE, duration,
                    4, 3f, BulletDirection.CURRENT, 30f*side, accel2);
                    yield return new WaitForSeconds(0.48f);
                }
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 9; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 13f, m_Direction*side, accel1, 24, 15f, BulletType.ERASE_AND_CREATE, duration,
                    4, 3f, BulletDirection.CURRENT, 30f*side, accel2);
                    yield return new WaitForSeconds(0.24f);
                }
            }
            else {
                for (int i = 0; i < 11; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 13f, m_Direction*side, accel1, 30, 12f, BulletType.ERASE_AND_CREATE, duration,
                    4, 3f, BulletDirection.CURRENT, 30f*side, accel2);
                    yield return new WaitForSeconds(0.18f);
                }
            }
            side *= -1;
        }
    }
}