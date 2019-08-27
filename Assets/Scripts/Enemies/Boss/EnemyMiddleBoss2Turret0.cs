using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMiddleBoss2Turret0 : EnemyUnit
{
    public GameObject m_Barrel;
    public float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        GetCoordinates();
        m_CurrentPattern = Pattern1();
        StartCoroutine(m_CurrentPattern);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 50f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(2.5f);

        while (true) {
            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 6.4f - i*0.5f, m_CurrentAngle - 64 + i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 6.4f - i*0.5f, m_CurrentAngle + 64 - i*8f, accel);
                    yield return new WaitForSeconds(0.24f);
                }
                StartCoroutine(ShootAnimation());
                for (int i = 0; i < 3; i++) {
                    float random_value = Random.Range(-2f, 2f);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 4.4f + i*1.1f, m_CurrentAngle + random_value, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 4.4f + i*1.1f, m_CurrentAngle - random_value, accel);
                }
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 7; i++) {
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 6.4f - i*0.3f, m_CurrentAngle - 64 + i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 6.4f - i*0.3f, m_CurrentAngle + 64 - i*8f, accel);
                    yield return new WaitForSeconds(0.17f);
                }
                StartCoroutine(ShootAnimation());
                for (int i = 0; i < 6; i++) {
                    float random_value = Random.Range(-2f, 2f);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 4f + i*0.8f, m_CurrentAngle + random_value, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 4f + i*0.8f, m_CurrentAngle - random_value, accel);
                }
            }
            else {
                for (int i = 0; i < 7; i++) {
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 6.8f - i*0.3f, m_CurrentAngle - 66 + i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 6f - i*0.3f, m_CurrentAngle - 62 + i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 6.8f - i*0.3f, m_CurrentAngle + 66 - i*8f, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 6f - i*0.3f, m_CurrentAngle + 62 - i*8f, accel);
                    yield return new WaitForSeconds(0.17f);
                }
                StartCoroutine(ShootAnimation());
                for (int i = 0; i < 6; i++) {
                    float random_value = Random.Range(-1f, 1f);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[0].position), 4f + i*0.8f, m_CurrentAngle + random_value, accel);
                    CreateBullet(3, GetScreenPosition(m_FirePosition[1].position), 4f + i*0.8f, m_CurrentAngle - random_value, accel);
                }
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator ShootAnimation() {
        m_Barrel.transform.DOLocalMoveZ(0.2f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        m_Barrel.transform.DOLocalMoveZ(0.47f, 0.4f);
        yield break;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ((EnemyMiddleBoss2) m_ParentEnemy).ToPhase1();
        Destroy(gameObject);
        yield break;
    }
}
