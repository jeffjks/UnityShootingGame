using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankMedium2 : EnemyUnit
{
    public Transform m_FirePosition;
    [SerializeField] private Transform[] m_ArmorPosition = new Transform[2];

    private bool m_ShootState = false;
    private Vector3[] m_ArmorPositionTarget = new Vector3[2];

    void Start()
    {
        m_ArmorPositionTarget[0] = new Vector3(0.365f, m_ArmorPosition[0].localPosition.y, m_ArmorPosition[0].localPosition.z);
        m_ArmorPositionTarget[1] = new Vector3(-0.365f, m_ArmorPosition[1].localPosition.y, m_ArmorPosition[1].localPosition.z);
    }
    
    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);

        if (!m_ShootState) {
            if (m_Position2D.y < - 1.2f) {
                if (m_ArmorPosition[0].localPosition != m_ArmorPositionTarget[0]) {
                    float maxDistanceDelta = 0.6f / Application.targetFrameRate * Time.timeScale;
                    m_ArmorPosition[0].localPosition = Vector3.MoveTowards(m_ArmorPosition[0].localPosition, m_ArmorPositionTarget[0], maxDistanceDelta);
                    m_ArmorPosition[1].localPosition = Vector3.MoveTowards(m_ArmorPosition[1].localPosition, m_ArmorPositionTarget[1], maxDistanceDelta);
                }
                else {
                    m_ShootState = true;
                    StartCoroutine(Pattern1());
                }
            }
        }
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(Random.Range(0, 1000));
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(5, pos, 6.6f, Random.Range(0f, 360f), accel, 18, 20f); // 1
                yield return new WaitForMillisecondFrames(1000);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                CreateBulletsSector(2, pos, 6.6f, Random.Range(0f, 360f), accel, 10, 36f,
                2, 200, 1, 5.8f, BulletDirection.PLAYER, Random.Range(-2f, 2f), accel);
                yield return new WaitForMillisecondFrames(1000);
            }

            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int i = 0; i < 5; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(5, pos, 6.6f, Random.Range(0f, 360f), accel, 30, 12f); // 1
                    yield return new WaitForMillisecondFrames(380);
                }
                yield return new WaitForMillisecondFrames(700);
                for (int i = 0; i < 3; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 6.6f, Random.Range(0f, 360f), accel, 24, 15f,
                    2, 200, 1, 5.8f, BulletDirection.PLAYER, Random.Range(-2f, 2f), accel);
                    yield return new WaitForMillisecondFrames(380);
                }
                yield return new WaitForMillisecondFrames(700);
            }

            else if (SystemManager.Difficulty == GameDifficulty.Hell) {
                for (int i = 0; i < 5; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(5, pos, 7.5f, Random.Range(0f, 360f), accel, 36, 10f); // 1
                    yield return new WaitForMillisecondFrames(380);
                }
                yield return new WaitForMillisecondFrames(700);
                for (int i = 0; i < 3; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    CreateBulletsSector(2, pos, 7.5f, Random.Range(0f, 360f), accel, 30, 12f,
                    2, 200, 1, 6.5f, BulletDirection.PLAYER, Random.Range(-2f, 2f), accel);
                    yield return new WaitForMillisecondFrames(380);
                }
                yield return new WaitForMillisecondFrames(700);
            }
        }
    }
}
