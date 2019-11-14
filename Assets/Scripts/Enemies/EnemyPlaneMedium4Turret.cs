using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneMedium4Turret : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        GetCoordinates();
        m_CurrentPattern = Pattern1();
    }

    protected override void Update()
    {
        m_CurrentAngle += 180f * Time.deltaTime * transform.localScale.x;
        
        base.Update();
    }

    public void StartPattern() {
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3[] pos = new Vector3[2];
        float factor;

        if (m_SystemManager.m_Difficulty == 0) {
            while(true) {
                factor = SpeedOval(m_CurrentAngle);
                pos[0] = m_FirePosition[0].position;
                pos[1] = m_FirePosition[1].position;
                CreateBullet(1, pos[0], 6f * factor, m_CurrentAngle, accel);
                CreateBullet(1, pos[1], 6f * factor, m_CurrentAngle - 180f, accel);
                yield return new WaitForSeconds(0.16f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while(true) {
                factor = SpeedOval(m_CurrentAngle);
                pos[0] = m_FirePosition[0].position;
                pos[1] = m_FirePosition[1].position;
                CreateBulletsSector(1, pos[0], 6f * factor, m_CurrentAngle, accel, 2, 1f);
                CreateBulletsSector(1, pos[1], 6f * factor, m_CurrentAngle - 180f, accel, 2, 1f);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else {
            while(true) {
                factor = SpeedOval(m_CurrentAngle);
                pos[0] = m_FirePosition[0].position;
                pos[1] = m_FirePosition[1].position;
                CreateBulletsSector(1, pos[0], 6f * factor, m_CurrentAngle, accel, 2, 2f);
                CreateBulletsSector(1, pos[1], 6f * factor, m_CurrentAngle - 180f, accel, 2, 2f);
                yield return new WaitForSeconds(0.08f);
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
