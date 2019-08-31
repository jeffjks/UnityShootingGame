using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret0_1 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    public Transform m_FirePosition = null;
    
    private IEnumerator m_CurrentPattern = null;
    private int m_Side = 1, m_RoateState = 0;
    private bool m_Pattern2Rotate = false;

    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_Pattern2Rotate) {
            if (m_RoateState == -1)
                RotateSlightly(m_PlayerPosition, 180f);
            if (m_RoateState == 0)
                RotateSlightly(90f + 90f*transform.localScale.x*m_Side, 180f); // Prepare 1
            if (m_RoateState == 1)
                RotateSlightly(90f + 90f*m_Side, 180f); // Prepare 2
            if (m_RoateState == 2)
                RotateSlightly(m_CurrentAngle + 60f*transform.localScale.x, 360f); // Rotate
            if (m_RoateState == 3)
                RotateSlightly(m_CurrentAngle + 150f*transform.localScale.x, 850f); // Rotate Fast
        }
        else {
            if (m_PlayerManager.m_PlayerIsAlive)
                RotateImmediately(m_PlayerPosition);
            else
                RotateSlightly(m_PlayerPosition, 100f);
        }
        
        base.Update();
    }

    public void StartPattern(byte num) {
        if (num == 0)
            m_CurrentPattern = Pattern0();
        else if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        else if (num == 3)
            m_CurrentPattern = Pattern3();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern(bool stop_rotate = false) {
        m_RoateState = -1;
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
        if (stop_rotate)
            StartCoroutine(StopRotate());
        else
            m_Pattern2Rotate = false;
    }

    private IEnumerator StopRotate() {
        m_RoateState = -1;
        StopCoroutine(m_CurrentPattern);
        yield return new WaitForSeconds(1f);
        m_Pattern2Rotate = false;
    }

    public void PrepareRotate(int rotate_state, int side) {
        m_Side = side;
        m_RoateState = rotate_state;
        m_Pattern2Rotate = true;
    }

    private IEnumerator Pattern0() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos = m_FirePosition.position;
        while (true) {
            if (m_PlayerManager.m_Player.transform.position.y >= -5.8f) {
                if (!m_Pattern2Rotate) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(0, pos, 6.6f, m_CurrentAngle, accel, 8, 2.5f);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos = m_FirePosition.position;
        if (transform.localScale.x == -1f)
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty] * 0.5f);
        while (true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos = GetScreenPosition(m_FirePosition.position);
                for (int i = 0; i < 2; i++)
                    CreateBullet(4, pos, 6f + i*0.5f, m_CurrentAngle, accel);
            }
            else {
                pos = GetScreenPosition(m_FirePosition.position);
                for (int i = 0; i < 4; i++)
                    CreateBullet(4, pos, 6.5f + i*0.5f, m_CurrentAngle, accel);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        m_RoateState = 2;

        if (m_SystemManager.m_Difficulty == 0) {
            while (true) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 6f, m_CurrentAngle, accel, 2, 18f);
                yield return new WaitForSeconds(0.12f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while (true) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 6f, m_CurrentAngle, accel, 5, 14f);
                yield return new WaitForSeconds(0.08f);
            }
        }
        else {
            while (true) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 6f, m_CurrentAngle, accel, 7, 11f);
                CreateBulletsSector(4, pos, 6.18f, m_CurrentAngle, accel, 7, 11f);
                yield return new WaitForSeconds(0.08f);
            }
        }
    }

    private IEnumerator Pattern3()
    {
        float speed1 = 1.5f, speed2 = 7f;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(speed1, 0.4f);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(speed2, 0.7f);
        Vector3 pos1, pos2;
        m_RoateState = 3;
        float gap = 0.14f;
        
        if (m_Side == 1) {
            yield return new WaitForSeconds(0.4f);
        }

        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 30; i++) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(1, pos1, 8f, m_CurrentAngle, accel1, BulletType.ERASE_AND_CREATE, 0.4f,
                1, speed1, BulletDirection.CURRENT, 15f, accel2);
                CreateBullet(1, pos2, 8f, m_CurrentAngle, accel1, BulletType.ERASE_AND_CREATE, 0.4f,
                1, speed1, BulletDirection.CURRENT, 15f, accel2);
                yield return new WaitForSeconds(0.014f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            for (int i = 0; i < 36; i++) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(1, pos1, 8f, m_CurrentAngle, accel1, BulletType.ERASE_AND_CREATE, 0.4f,
                1, speed1, BulletDirection.CURRENT, 15f, accel2, 2, 6f);
                CreateBullet(1, pos2, 8f, m_CurrentAngle, accel1, BulletType.ERASE_AND_CREATE, 0.4f,
                1, speed1, BulletDirection.CURRENT, 15f, accel2, 2, 6f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else {
            for (int i = 0; i < 36; i++) {
                pos1 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(1, pos1, 8f, m_CurrentAngle, accel1, BulletType.ERASE_AND_CREATE, 0.4f,
                1, speed1, BulletDirection.CURRENT, 15f, accel2, 2, 6f);
                CreateBullet(1, pos2, 8f, m_CurrentAngle, accel1, BulletType.ERASE_AND_CREATE, 0.4f,
                1, speed1, BulletDirection.CURRENT, 15f, accel2, 2, 6f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        StopPattern(true);
    }
}