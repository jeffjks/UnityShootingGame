using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge3 : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[2];
    private int[] m_FireDelay = { 1000, 550, 250 };
    
    void Start()
    {
        StartCoroutine(Pattern1());
    }
    
    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] target_angle = new float[2];

        while(true) {
            for (int i = 0; i < 2; i++) {
                pos[i] = GetScreenPosition(m_FirePosition[i].position);
                target_angle[i] = GetAngleToTarget(pos[i], m_PlayerManager.GetPlayerPosition());
                for (int j = 0; j < 5; j++) {
                    pos[i] = GetScreenPosition(m_FirePosition[i].position);
                    CreateBullet(4, pos[i], 8f, target_angle[i], accel);
                    yield return new WaitForMillisecondFrames(60);
                }
                yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
            }
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector3(-1.4f, 0f, -1.4f));
        ExplosionEffect(0, -1, new Vector3(1.4f, 0f, -1.4f));
        ExplosionEffect(0, -1, new Vector3(-1.4f, 0f, 1.4f));
        ExplosionEffect(0, -1, new Vector3(1.4f, 0f, 1.4f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
