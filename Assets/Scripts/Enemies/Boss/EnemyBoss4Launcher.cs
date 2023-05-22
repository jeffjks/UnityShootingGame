using System.Collections;
using System;
using UnityEngine;

public class EnemyBoss4Launcher : EnemyUnit
{
    public Transform m_FirePosition;
    public int m_Position;
    
    private IEnumerator m_CurrentPattern;
    private bool m_Moving = false;
    private int m_MoveDirection;
    private byte m_Pattern = 0;
    private float m_Direction;

    public event Func<float> Func_GetDirection;

    void Start()
    {
        m_MoveDirection = m_Position;
    }

    protected override void Update() {
        base.Update();
        
        if (m_Position == 1) {
            if (transform.localPosition.x > 9f) {
                m_MoveDirection *= -1;
                transform.localPosition = new Vector3(9f , transform.localPosition.y, transform.localPosition.z);
            }
            else if (transform.localPosition.x < 5f) {
                m_MoveDirection *= -1;
                transform.localPosition = new Vector3(5f , transform.localPosition.y, transform.localPosition.z);
            }
        }
        else {
            if (transform.localPosition.x < -9f) {
                m_MoveDirection *= -1;
                transform.localPosition = new Vector3(-9f , transform.localPosition.y, transform.localPosition.z);
            }
            else if (transform.localPosition.x > -5f) {
                m_MoveDirection *= -1;
                transform.localPosition = new Vector3(-5f , transform.localPosition.y, transform.localPosition.z);
            }
        }

        float dir = m_MoveDirection*3f / Application.targetFrameRate * Time.timeScale;
        if (m_Moving) {
            transform.localPosition = new Vector3(transform.localPosition.x + dir , transform.localPosition.y, transform.localPosition.z);
        }
        else if (transform.localPosition.x != 5f*m_Position) {
            transform.localPosition = new Vector3(transform.localPosition.x + dir , transform.localPosition.y, transform.localPosition.z);
        }

        if (m_Direction > 360f)
            m_Direction -= 360f;
        else if (m_Direction < 0f)
            m_Direction += 360f;
    }

    public void SetMoving(bool boolean) {
        m_Moving = boolean;
    }

    public void StartPattern(byte num, int dir = 0) {
        m_Pattern = num;
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        else if (num == 3)
            m_CurrentPattern = Pattern3(dir);
        else
            return;
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        m_Pattern = 0;
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }


    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        while (true) {
            pos = GetScreenPosition(m_FirePosition.position);
            if (m_SystemManager.GetDifficulty() == GameDifficulty.Normal) {
                CreateBulletsSector(4, pos, 6.1f, Func_GetDirection.Invoke(), accel, 4, 90f);
                yield return new WaitForFrames(9);
            }
            else if (m_SystemManager.GetDifficulty() == GameDifficulty.Expert) {
                CreateBulletsSector(4, pos, 6.4f, Func_GetDirection.Invoke(), accel, 5, 72f);
                yield return new WaitForFrames(7);
            }
            else {
                CreateBulletsSector(4, pos, 6.8f, Func_GetDirection.Invoke(), accel, 6, 60f);
                yield return new WaitForFrames(5);
            }
        }
    }

    private IEnumerator Pattern2() {
        Vector3 pos = GetScreenPosition(m_FirePosition.position);
        EnemyBulletAccel accel = new EnemyBulletAccel(4.8f, 500);
        
        if (m_SystemManager.GetDifficulty() == GameDifficulty.Normal) {
            CreateBulletsSector(2, pos, 7f, UnityEngine.Random.Range(0f, 360f), accel, 24, 15f);
        }
        else if (m_SystemManager.GetDifficulty() == GameDifficulty.Expert) {
            CreateBulletsSector(2, pos, 7.6f, UnityEngine.Random.Range(0f, 360f), accel, 36, 10f);
        }
        else {
            CreateBulletsSector(2, pos, 8f, UnityEngine.Random.Range(0f, 360f), accel, 45, 8f);
        }
        yield break;
    }

    private IEnumerator Pattern3(int dir) {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int rotate;
        m_Direction = UnityEngine.Random.Range(0f, 360f);
        if (dir == 0)
            rotate = -1;
        else
            rotate = 1;
        
        while (true) {
            pos = GetScreenPosition(m_FirePosition.position);
            if (m_SystemManager.GetDifficulty() == GameDifficulty.Normal) {
                CreateBulletsSector(2, pos, 4.3f, (m_Direction - 1.4f)*rotate, accel, 8, 45f);
                CreateBulletsSector(2, pos, 4.5f, (m_Direction)*rotate, accel, 8, 45f);
                CreateBulletsSector(2, pos, 4.7f, (m_Direction + 1.4f)*rotate, accel, 8, 45f);
                m_Direction += 12f;
                yield return new WaitForMillisecondFrames(1000 + UnityEngine.Random.Range(0, 300));
            }
            else if (m_SystemManager.GetDifficulty() == GameDifficulty.Expert) {
                CreateBulletsSector(2, pos, 4.25f, (m_Direction - 1.5f)*rotate, accel, 12, 30f);
                CreateBulletsSector(2, pos, 4.5f, (m_Direction)*rotate, accel, 12, 30f);
                CreateBulletsSector(2, pos, 4.75f, (m_Direction + 1.5f)*rotate, accel, 12, 30f);
                m_Direction += 10f;
                yield return new WaitForMillisecondFrames(600 + UnityEngine.Random.Range(0, 200));
            }
            else {
                CreateBulletsSector(2, pos, 4.25f, (m_Direction - 2.25f)*rotate, accel, 12, 30f);
                CreateBulletsSector(2, pos, 4.5f, (m_Direction - 0.75f)*rotate, accel, 12, 30f);
                CreateBulletsSector(2, pos, 4.75f, (m_Direction + 0.75f)*rotate, accel, 12, 30f);
                CreateBulletsSector(2, pos, 5f, (m_Direction + 2.25f)*rotate, accel, 12, 30f);
                m_Direction += 10f;
                yield return new WaitForMillisecondFrames(400 + UnityEngine.Random.Range(0, 200));
            }
        }
    }
}