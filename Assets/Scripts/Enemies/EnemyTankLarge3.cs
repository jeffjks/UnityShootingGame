using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge3 : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    
    void Start()
    {
        GetCoordinates();
        StartCoroutine(Pattern1());
    }
    
    protected override void FixedUpdate()
    {
        RotateImmediately(m_MoveVector.direction);
        base.FixedUpdate();
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float[] target_angle = new float[2];

        while(true) {
            for (int i = 0; i < 2; i++) {
                pos[i] = GetScreenPosition(m_FirePosition[i].position);
                target_angle[i] = GetAngleToTarget(pos[i], m_PlayerManager.m_Player.transform.position);
                for (int j = 0; j < 5; j++) {
                    pos[i] = GetScreenPosition(m_FirePosition[i].position);
                    CreateBullet(4, pos[i], 8f, target_angle[i], accel);
                    yield return new WaitForSeconds(0.06f);
                }
                yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
            }
        }
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector3(-1.4f, 0f, -1.4f));
        ExplosionEffect(0, -1, new Vector3(1.4f, 0f, -1.4f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
