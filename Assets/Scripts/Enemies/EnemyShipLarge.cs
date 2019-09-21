using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipLarge : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];

    private int m_Phase = 0;
    
    void Start()
    {
        GetCoordinates();
        RotateImmediately(m_MoveVector.direction);
    }
    
    protected override void FixedUpdate()
    {
        RotateImmediately(m_MoveVector.direction);

        if (m_Phase == 0) {
            if (3 * m_Health < m_MaxHealth) {
                m_Phase = 1;
                StartCoroutine(Pattern1());
            }
        }
        
        base.FixedUpdate();
    }
    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, 0.6f);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0f);

        while(true) {
            for (int i = 0; i < 2; i++) {
                pos[i] = GetScreenPosition(m_FirePosition[i].position);
                CreateBullet(0, pos[i], 3.6f, Random.Range(0f, 360f), accel1, 2, 0.6f,
                4, 6f, BulletDirection.PLAYER, Random.Range(-15f, 15f), accel2);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        float timer = 0f, random_timer = 0f;
        float explosion_height = 2f;
        Vector3 random_pos1, random_pos2;
        while (timer < 0.7f) {
            random_timer = Random.Range(0.1f, 0.25f);
            random_pos1 = Random.insideUnitCircle * 1;
            random_pos2 = Random.insideUnitCircle * 1;
            ExplosionEffect(0, 0, new Vector3(random_pos1.x, explosion_height, random_pos1.z) + new Vector3(0f, 0f, 1.7f));
            ExplosionEffect(0, -1, new Vector3(random_pos2.x, explosion_height, random_pos2.z) + new Vector3(0f, 0f, -2f));
            yield return new WaitForSeconds(random_timer);
            timer += random_timer;
        }
        ExplosionEffect(1, 1, new Vector3(0f, 2f, 1.7f));
        ExplosionEffect(0, -1, new Vector3(0f, 2f, 0f));
        ExplosionEffect(1, -1, new Vector3(0f, 2f, -2f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
