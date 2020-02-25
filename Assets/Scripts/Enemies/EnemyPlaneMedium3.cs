using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyPlaneMedium3 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private EnemyPlaneMedium3Turret[] m_Turret = new EnemyPlaneMedium3Turret[2];
    
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 1.2f;
    private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 1.2f;

    void Start ()
    {
        float time_limit = 7.8f;
        m_PositionY = transform.position.y;
        
        StartCoroutine(Pattern1());

        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -1.8f + m_VSpeed*m_AppearanceTime, m_AppearanceTime).SetEase(Ease.OutQuad));
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
        Vector3 pos;
        float target_angle, random_value;
        yield return new WaitForSeconds(m_AppearanceTime + Random.Range(-0.5f, 0.5f));

        while(!m_TimeLimitState) {
            pos = transform.position;
            target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
            random_value = Random.Range(-2f, 2f);
            m_Turret[0].StartCoroutine("Pattern1");
            m_Turret[1].StartCoroutine("Pattern1");

            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 3; i++) {
                    CreateBullet(2, pos, 5f, target_angle + random_value, accel);
                    yield return new WaitForSeconds(0.084f);
                }
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 4; i++) {
                    CreateBulletsSector(2, pos, 6f, target_angle + random_value, accel, 3, 10f);
                    yield return new WaitForSeconds(0.07f);
                }
            }
            else {
                for (int i = 0; i < 4; i++) {
                    CreateBulletsSector(2, pos, 7f, target_angle + random_value, accel, 3, 10f);
                    yield return new WaitForSeconds(0.06f);
                }
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
        yield break;
    }
}