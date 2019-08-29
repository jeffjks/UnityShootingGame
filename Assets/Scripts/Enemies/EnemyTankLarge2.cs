using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge2 : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    private float m_Direction = 0f;

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1());
    }
    
    protected override void Update()
    {
        RotateImmediately(m_MoveVector.direction);

        if (m_SystemManager.m_Difficulty == 0) {
            m_Direction += 97f * Time.deltaTime;
        }
        else {
            m_Direction += 123f * Time.deltaTime;
        }

        if (m_Direction >= 360f) {
            m_Direction -= 360f;
        }
        
        base.Update();
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                CreateBulletsSector(3, pos[0], 6f, m_CurrentAngle + m_Direction, accel, 3, 120f);
                CreateBulletsSector(3, pos[1], 6f, m_CurrentAngle - m_Direction, accel, 3, 120f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                CreateBulletsSector(3, pos[0], 6.8f, m_CurrentAngle + m_Direction, accel, 4, 90f);
                CreateBulletsSector(3, pos[1], 6.8f, m_CurrentAngle - m_Direction, accel, 4, 90f);
            }
            else {
                pos[0] = GetScreenPosition(m_FirePosition[0].position);
                pos[1] = GetScreenPosition(m_FirePosition[1].position);
                for (int i = 0; i < 3; i++) {
                    CreateBulletsSector(3, pos[0], 6.3f + 0.5f*i, m_CurrentAngle + m_Direction, accel, 4, 90f);
                    CreateBulletsSector(3, pos[1], 6.3f + 0.5f*i, m_CurrentAngle - m_Direction, accel, 4, 90f);
                }
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector3(0f, 0f, 2f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}