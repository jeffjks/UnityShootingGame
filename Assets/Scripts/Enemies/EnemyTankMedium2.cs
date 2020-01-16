using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankMedium2 : EnemyUnit
{
    [SerializeField] private Transform m_FirePosition = null;
    [SerializeField] private Transform[] m_ArmorPosition = new Transform[2];

    private bool m_ShootState = false;
    private Vector3[] m_ArmorPositionTarget = new Vector3[2];

    void Start()
    {
        GetCoordinates();

        m_ArmorPositionTarget[0] = new Vector3(0.365f, m_ArmorPosition[0].localPosition.y, m_ArmorPosition[0].localPosition.z);
        m_ArmorPositionTarget[1] = new Vector3(-0.365f, m_ArmorPosition[1].localPosition.y, m_ArmorPosition[1].localPosition.z);
    }
    
    protected override void Update()
    {
        RotateImmediately(m_MoveVector.direction);

        if (!m_ShootState) {
            if (m_Position2D.y < - 1.2f) {
                if (m_ArmorPosition[0].localPosition != m_ArmorPositionTarget[0]) {
                    m_ArmorPosition[0].localPosition = Vector3.MoveTowards(m_ArmorPosition[0].localPosition, m_ArmorPositionTarget[0], 0.6f*Time.deltaTime);
                    m_ArmorPosition[1].localPosition = Vector3.MoveTowards(m_ArmorPosition[1].localPosition, m_ArmorPositionTarget[1], 0.6f*Time.deltaTime);
                }
                else {
                    m_ShootState = true;
                    StartCoroutine(Pattern1());
                }
            }
        }
        
        base.Update();
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(Random.Range(0f, 1f));
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(5, pos, 6.6f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForSeconds(1f);
                pos = GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(2, pos, 6.6f, Random.Range(0f, 360f), accel, 30, 12f,
                2, 0.5f, 1, 5.8f, BulletDirection.PLAYER, Random.Range(-2f, 2f), accel);
                yield return new WaitForSeconds(1f);
            }

            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 5; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(5, pos, 6.6f, Random.Range(0f, 360f), accel, 30, 12f);
                    yield return new WaitForSeconds(0.38f);
                }
                yield return new WaitForSeconds(0.7f);
                for (int i = 0; i < 3; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 6.6f, Random.Range(0f, 360f), accel, 18, 20f,
                    2, 0.2f, 1, 5.8f, BulletDirection.PLAYER, Random.Range(-2f, 2f), accel);
                    yield return new WaitForSeconds(0.38f);
                }
                yield return new WaitForSeconds(0.7f);
            }

            else if (m_SystemManager.m_Difficulty == 2) {
                for (int i = 0; i < 5; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(5, pos, 7.5f, Random.Range(0f, 360f), accel, 36, 10f);
                    yield return new WaitForSeconds(0.38f);
                }
                yield return new WaitForSeconds(0.7f);
                for (int i = 0; i < 3; i++) {
                    pos = GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 7.5f, Random.Range(0f, 360f), accel, 24, 15f,
                    2, 0.2f, 1, 6.5f, BulletDirection.PLAYER, Random.Range(-2f, 2f), accel);
                    yield return new WaitForSeconds(0.38f);
                }
                yield return new WaitForSeconds(0.7f);
            }
        }
    }
}
