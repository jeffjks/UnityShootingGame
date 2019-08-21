using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyGunship : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    
    private bool m_TimeLimitState = false;

    void Start()
    {
        float time_limit = 8f;
        
        GetCoordinates();
        InvokeRepeating("Pattern1", 1f, m_FireDelay[m_SystemManager.m_Difficulty]);
        StartCoroutine(Pattern1());
        RotateImmediately(m_PlayerPosition);

        Invoke("TimeLimit", time_limit);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
        float time_limit_direction = Mathf.Sign(transform.position.x);
        if (time_limit_direction == 1) {
            m_MoveVector.direction = Random.Range(80f, 100f);
        }
        else {
            m_MoveVector.direction = Random.Range(-80f, -100f);
        }
        DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 5.4f, 0.8f).SetEase(Ease.OutQuad);
    }

    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(1.2f);
        
        while (!m_TimeLimitState) {
            for (int i = 0; i < 4; i++) {
                Vector3 pos1 = m_FirePosition[0].position;
                Vector3 pos2 = m_FirePosition[1].position;
                if (m_SystemManager.m_Difficulty == 0) {
                    CreateBullet(4, pos1, 5.8f, m_CurrentAngle + 2f, accel);
                    CreateBullet(4, pos2, 5.8f, m_CurrentAngle - 2f, accel);
                    break;
                }
                else if (m_SystemManager.m_Difficulty == 1) {
                    CreateBullet(4, pos1, 5.8f, m_CurrentAngle + 2f, accel);
                    CreateBullet(4, pos2, 5.8f, m_CurrentAngle - 2f, accel);
                }
                else {
                    CreateBullet(4, pos1, 5.8f, m_CurrentAngle + 2f, accel);
                    CreateBullet(4, pos1, 5.8f, m_CurrentAngle + 8f, accel);
                    CreateBullet(4, pos2, 5.8f, m_CurrentAngle - 8f, accel);
                    CreateBullet(4, pos2, 5.8f, m_CurrentAngle - 2f, accel);
                }
                yield return new WaitForSeconds(0.16f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
        yield break;
    }
}
