using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge2 : EnemyUnit
{
    private int[] m_FireDelay = { 1000, 400, 400 };
    private float m_Direction;

    void Start()
    {
        StartCoroutine(Pattern1());
    }
    
    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            m_Direction += 97f / Application.targetFrameRate * Time.timeScale;
        }
        else {
            m_Direction += 123f / Application.targetFrameRate * Time.timeScale;
        }

        if (m_Direction >= 360f) {
            m_Direction -= 360f;
        }
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        BulletAccel accel = new BulletAccel(0f, 0);
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                CreateBulletsSector(3, pos[0], 6f, CurrentAngle + m_Direction, accel, 3, 120f);
                CreateBulletsSector(3, pos[1], 6f, CurrentAngle - m_Direction, accel, 3, 120f);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                CreateBulletsSector(3, pos[0], 6.8f, CurrentAngle + m_Direction, accel, 4, 90f);
                CreateBulletsSector(3, pos[1], 6.8f, CurrentAngle - m_Direction, accel, 4, 90f);
            }
            else {
                pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                for (int i = 0; i < 3; i++) {
                    CreateBulletsSector(3, pos[0], 6.3f + 0.5f*i, CurrentAngle + m_Direction, accel, 4, 90f);
                    CreateBulletsSector(3, pos[1], 6.3f + 0.5f*i, CurrentAngle - m_Direction, accel, 4, 90f);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}