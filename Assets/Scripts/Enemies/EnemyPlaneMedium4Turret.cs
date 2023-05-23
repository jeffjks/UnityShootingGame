using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium4Turret : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[2];
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        m_CurrentPattern = Pattern1();
    }

    protected override void Update()
    {
        base.Update();
        
        m_CurrentAngle += 180f / Application.targetFrameRate * transform.localScale.x * Time.timeScale;
    }

    public void StartPattern() {
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3[] pos = new Vector3[2];
        float factor;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true) {
                factor = SpeedOval(m_CurrentAngle);
                pos[0] = m_FirePosition[0].position;
                pos[1] = m_FirePosition[1].position;
                CreateBullet(1, pos[0], 6f * factor, m_CurrentAngle, accel);
                CreateBullet(1, pos[1], 6f * factor, m_CurrentAngle - 180f, accel);
                yield return new WaitForMillisecondFrames(160);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                factor = SpeedOval(m_CurrentAngle);
                pos[0] = m_FirePosition[0].position;
                pos[1] = m_FirePosition[1].position;
                CreateBulletsSector(1, pos[0], 6f * factor, m_CurrentAngle, accel, 2, 1f);
                CreateBulletsSector(1, pos[1], 6f * factor, m_CurrentAngle - 180f, accel, 2, 1f);
                yield return new WaitForMillisecondFrames(100);
            }
        }
        else {
            while(true) {
                factor = SpeedOval(m_CurrentAngle);
                pos[0] = m_FirePosition[0].position;
                pos[1] = m_FirePosition[1].position;
                CreateBulletsSector(1, pos[0], 6f * factor, m_CurrentAngle, accel, 2, 2f);
                CreateBulletsSector(1, pos[1], 6f * factor, m_CurrentAngle - 180f, accel, 2, 2f);
                yield return new WaitForMillisecondFrames(80);
            }
        }
    }

    private float SpeedOval(float angle) {
        float cos = Mathf.Cos(m_CurrentAngle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(m_CurrentAngle * Mathf.Deg2Rad);
        float result = Mathf.Pow(cos, 2) + Mathf.Pow(sin, 2) * 0.7f;
        return result;
    }
}
