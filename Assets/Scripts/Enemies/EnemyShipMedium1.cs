using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipMedium1 : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[7];
    private int[] m_FireDelay = { 2200, 2000, 1500 };
    
    void Start()
    {
        StartCoroutine(Pattern1());
        StartCoroutine(Pattern2());
    }
    
    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[5];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] target_angle = new float[5];

        for (int i = 0; i < 5; i++) {
            target_angle[i] = m_FirePosition[i].parent.parent.parent.localRotation.eulerAngles.y;
        }

        while(true) {
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 5; j++) {
                    pos[j] = BackgroundCamera.GetScreenPosition(m_FirePosition[j].position);
                    CreateBullet(1, pos[j], 4.3f, CurrentAngle - target_angle[j], accel);
                }
                yield return new WaitForMillisecondFrames(100);
            }
            yield return new WaitForMillisecondFrames(1600);
        }
    }
    
    private IEnumerator Pattern2() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(6f, 800);

        while(true) {
            for (int i = 0; i < 2; i++) {
                pos[i] = BackgroundCamera.GetScreenPosition(m_FirePosition[i+5].position);
                if (SystemManager.Difficulty == GameDifficulty.Normal)
                    CreateBulletsSector(5, pos[i], 2f, Random.Range(0f, 360f), accel, 12, 30f);
                else if (SystemManager.Difficulty == GameDifficulty.Expert)
                    CreateBulletsSector(5, pos[i], 2f, Random.Range(0f, 360f), accel, 24, 15f);
                else
                    CreateBulletsSector(5, pos[i], 2f, Random.Range(0f, 360f), accel, 30, 12f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }
}
