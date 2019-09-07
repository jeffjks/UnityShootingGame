using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyPlaneMedium4 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private EnemyPlaneMedium4Turret[] m_Turret = new EnemyPlaneMedium4Turret[2];
    
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 1.5f;
    private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 0.4f;

    void Start ()
    {
        float time_limit = 12.5f;
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
        yield return new WaitForSeconds(m_AppearanceTime);

        while(!m_TimeLimitState) {
            m_Turret[0].StartPattern();
            m_Turret[1].StartPattern();
            yield return new WaitForSeconds(2f);

            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
            
            if (m_SystemManager.m_Difficulty <= 1) {
                for (int i = 0; i < 10; i++) {
                    pos = transform.position;
                    target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                    random_value = Random.Range(-1f, 1f);
                    CreateBullet(2, pos, 7.2f+i*0.7f, target_angle + random_value, accel);
                    yield return new WaitForSeconds(0.06f);
                }
                yield return new WaitForSeconds(0.06f * 5);
            }
            else {
                for (int i = 0; i < 15; i++) {
                    pos = transform.position;
                    target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                    random_value = Random.Range(-1f, 1f);
                    CreateBulletsSector(0, pos, 6.8f+i*0.7f, target_angle + random_value, accel, 2, 24f);
                    CreateBullet(2, pos, 6.6f+i*0.7f, target_angle + random_value, accel);
                    yield return new WaitForSeconds(0.06f);
                }
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
        yield break;
    }
}
