using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankMedium3 : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[2];
    private int[] m_FireDelay = { 400, 200, 150 };
    private float m_Direction;

    void Start()
    {
        StartCoroutine(Pattern1());
        m_Direction = Random.Range(0f, 360f);
    }

    
    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);

        if (m_Direction >= 360f) {
            m_Direction -= 360f;
        }
        m_Direction += 90f / Application.targetFrameRate * Time.timeScale;
    }

    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        while(true) {
            pos[0] = GetScreenPosition(m_FirePosition[0].position);
            pos[1] = GetScreenPosition(m_FirePosition[1].position);
            CreateBullet(4, pos[0], 5.2f, m_Direction, accel);
            CreateBullet(4, pos[1], 5.2f, m_Direction + 180f, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}
