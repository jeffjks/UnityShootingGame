using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyPlaneMedium2 : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[5];

    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 1.6f;
    private float m_PositionY, m_AddPositionY = 0f;
    private float m_VSpeed = 1.1f;

    void Start ()
    {
        float time_limit = 8f;
        m_PositionY = transform.position.y;

        StartCoroutine(Pattern1());
        StartCoroutine(Pattern2());

        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -2.5f + m_VSpeed*m_AppearanceTime, m_AppearanceTime).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));

        Invoke("TimeLimit", m_AppearanceTime + time_limit);
    }

    protected override void Update()
    {
        m_AddPositionY -= m_VSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, m_PositionY + m_AddPositionY, transform.position.z);
        
        base.Update();
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
    }

    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(m_AppearanceTime);
        while(!m_TimeLimitState) {
            Vector3[] pos = new Vector3[m_FirePosition.Length];
            float target_angle;

            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos[1], ref pos[2], ref pos[3], ref pos[4]);
                    CreateBullet(4, pos[1], 6.2f, 0f, accel);
                    CreateBullet(4, pos[2], 6.2f, 0f, accel);
                    yield return new WaitForSeconds(0.08f);
                }
                yield return new WaitForSeconds(1f);
                target_angle = GetAngleToTarget(transform.position, m_PlayerManager.m_Player.transform.position);
                pos[0] = m_FirePosition[0].position;
                CreateBullet(3, pos[0], 5f + Random.Range(0f, 1.2f), target_angle + Random.Range(-24f, 24f), accel);
                CreateBullet(5, pos[0], 5f + Random.Range(0f, 1.2f), target_angle + Random.Range(-24f, 24f), accel);
                yield return new WaitForSeconds(2f);
            }

            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos[1], ref pos[2], ref pos[3], ref pos[4]);
                    CreateBullet(4, pos[1], 6.2f, 0f, accel);
                    CreateBullet(4, pos[2], 6.2f, 0f, accel);
                    yield return new WaitForSeconds(0.08f);
                }
                yield return new WaitForSeconds(1f);
                target_angle = GetAngleToTarget(transform.position, m_PlayerManager.m_Player.transform.position);
                pos[0] = m_FirePosition[0].position;
                for (int i = 0; i < 4; i++) {
                    CreateBullet(3, pos[0], 5f + Random.Range(0f, 1.2f), target_angle + Random.Range(-24f, 24f), accel);
                    CreateBullet(5, pos[0], 5f + Random.Range(0f, 1.2f), target_angle + Random.Range(-24f, 24f), accel);
                }
                yield return new WaitForSeconds(2f);
            }

            else {
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos[1], ref pos[2], ref pos[3], ref pos[4]);
                    CreateBullet(4, pos[1], 6.2f, 0f, accel);
                    CreateBullet(4, pos[2], 6.2f, 0f, accel);
                    yield return new WaitForSeconds(0.08f);
                }
                yield return new WaitForSeconds(0.8f);

                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos[1], ref pos[2], ref pos[3], ref pos[4]);
                    CreateBullet(4, pos[3], 6.2f, 0f, accel);
                    CreateBullet(4, pos[4], 6.2f, 0f, accel);
                    yield return new WaitForSeconds(0.08f);
                }
                yield return new WaitForSeconds(0.8f);
            }
        }
        yield break;
    }

    private IEnumerator Pattern2() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float target_angle;

        yield return new WaitForSeconds(m_AppearanceTime);
        while(!m_TimeLimitState) {
            yield return new WaitForSeconds(1f);

            if (m_SystemManager.m_Difficulty == 2) {
                target_angle = GetAngleToTarget(transform.position, m_PlayerManager.m_Player.transform.position);
                pos = m_FirePosition[0].position;
                for (int i = 0; i < 5; i++) {
                    CreateBullet(3, pos, 4.8f + Random.Range(0f, 1.8f), target_angle + Random.Range(-24f, 24f), accel);
                    CreateBullet(5, pos, 4.8f + Random.Range(0f, 1.8f), target_angle + Random.Range(-24f, 24f), accel);
                }
            }
            yield return new WaitForSeconds(2f);
        }
        yield break;
    }

    private void SetBulletVariables(ref Vector3 pos1, ref Vector3 pos2, ref Vector3 pos3, ref Vector3 pos4) {
        pos1 = m_FirePosition[1].position;
        pos2 = m_FirePosition[2].position;
        pos3 = m_FirePosition[3].position;
        pos4 = m_FirePosition[4].position;
    }
}
