using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret0_1 : EnemyUnit
{
    private int[] m_FireDelay = { 2400, 1500, 1000 };
    public Transform m_FirePosition;

    [HideInInspector] public int m_Phase;
    
    private IEnumerator m_CurrentPattern;
    private int m_Side = 1, m_RoateState;
    private bool m_Pattern2Rotate = false;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Pattern2Rotate) {
            if (m_RoateState == -1)
                RotateSlightly(PlayerManager.GetPlayerPosition(), 180f);
            if (m_RoateState == 0)
                RotateSlightly(90f + 90f*transform.localScale.x*m_Side, 180f); // Prepare 1
            if (m_RoateState == 1)
                RotateSlightly(90f + 90f*m_Side, 180f); // Prepare 2
            if (m_RoateState == 2)
                RotateSlightly(CurrentAngle + 60f*transform.localScale.x, 360f); // Rotate
            if (m_RoateState == 3)
                RotateSlightly(CurrentAngle + 150f*transform.localScale.x, 850f); // Rotate Fast
        }
        else {
            if (PlayerManager.IsPlayerAlive)
                RotateImmediately(PlayerManager.GetPlayerPosition());
            else
                RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
        }
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
        yield return new WaitForMillisecondFrames(1000);
        m_Pattern2Rotate = false;
    }

    public void PrepareRotate(int rotate_state, int side) {
        m_Side = side;
        m_RoateState = rotate_state;
        m_Pattern2Rotate = true;
    }

    private IEnumerator Pattern0() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos = m_FirePosition.position;
        while (m_Phase == 1) {
            if (PlayerManager.GetPlayerPosition().y >= -5.8f) {
                if (!m_Pattern2Rotate) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(0, pos, 6.6f, CurrentAngle, accel, 8, 2.5f);
                }
            }
            yield return new WaitForMillisecondFrames(500);
        }
    }

    private IEnumerator Pattern1()
    {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos = m_FirePosition.position;
        if (transform.localScale.x == -1f)
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty] * 500);
        while (true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                for (int i = 0; i < 2; i++)
                    CreateBullet(4, pos, 6f + i*0.5f, CurrentAngle, accel);
            }
            else {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                for (int i = 0; i < 4; i++)
                    CreateBullet(4, pos, 6.5f + i*0.5f, CurrentAngle, accel);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }

    private IEnumerator Pattern2()
    {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;
        m_RoateState = 2;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while (true) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 6f, CurrentAngle, accel, 2, 18f);
                yield return new WaitForMillisecondFrames(120);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while (true) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 6f, CurrentAngle, accel, 5, 14f);
                yield return new WaitForMillisecondFrames(80);
            }
        }
        else {
            while (true) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(4, pos, 6f, CurrentAngle, accel, 7, 11f);
                CreateBulletsSector(4, pos, 6.18f, CurrentAngle, accel, 7, 11f);
                yield return new WaitForMillisecondFrames(80);
            }
        }
    }

    private IEnumerator Pattern3()
    {
        float speed1 = 1.5f, speed2 = 7f;
        BulletAccel accel1 = new BulletAccel(speed1, 400);
        BulletAccel accel2 = new BulletAccel(speed2, 700);
        Vector3 pos1, pos2;
        m_RoateState = 3;
        float gap = 0.14f;
        
        if (m_Side == 1) {
            yield return new WaitForMillisecondFrames(400);
        }

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 18; i++) {
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(1, pos1, 8f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 400,
                1, speed1, BulletPivot.Current, 15f, accel2);
                CreateBullet(1, pos2, 8f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 400,
                1, speed1, BulletPivot.Current, 15f, accel2);
                yield return new WaitForFrames(2);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 36; i++) {
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(1, pos1, 8f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 400,
                1, speed1, BulletPivot.Current, 15f, accel2, 2, 6f);
                CreateBullet(1, pos2, 8f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 400,
                1, speed1, BulletPivot.Current, 15f, accel2, 2, 6f);
                yield return new WaitForFrames(1);
            }
        }
        else {
            for (int i = 0; i < 36; i++) {
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                CreateBullet(1, pos1, 10f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 360,
                1, speed1, BulletPivot.Current, 15f, accel2, 2, 6f);
                CreateBullet(1, pos1, 10f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 560,
                1, speed1, BulletPivot.Current, 9f, accel2, 2, 6f);
                CreateBullet(1, pos2, 10f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 360,
                1, speed1, BulletPivot.Current, 15f, accel2, 2, 6f);
                CreateBullet(1, pos2, 10f, CurrentAngle, accel1, BulletSpawnType.EraseAndCreate, 560,
                1, speed1, BulletPivot.Current, 9f, accel2, 2, 6f);
                yield return new WaitForFrames(1);
            }
        }
        StopPattern(true);
    }
}