using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankMedium3 : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    private float m_Direction;

    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1());
        m_Direction = Random.Range(0f, 360f);
    }

    
    protected override void FixedUpdate()
    {
        RotateImmediately(m_MoveVector.direction);

        if (m_Direction >= 360f) {
            m_Direction -= 360f;
        }
        m_Direction += 90f * Time.fixedDeltaTime;
        
        base.FixedUpdate();
    }

    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        while(true) {
            pos[0] = GetScreenPosition(m_FirePosition[0].position);
            pos[1] = GetScreenPosition(m_FirePosition[1].position);
            CreateBullet(4, pos[0], 5.2f, m_Direction, accel);
            CreateBullet(4, pos[1], 5.2f, m_Direction + 180f, accel);
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }
}
