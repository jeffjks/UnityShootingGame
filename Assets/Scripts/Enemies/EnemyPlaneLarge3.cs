using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyPlaneLarge3 : EnemyUnit
{
    [SerializeField] private EnemyPlaneLarge3Turret m_Turret = null;
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 1.5f;
    private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 0.3f;

    void Start ()
    {
        float time_limit = 14f;
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
        m_Turret.StopCoroutine("Pattern1");
    }

    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3[] pos = new Vector3[2];
        yield return new WaitForSeconds(m_AppearanceTime);

        while(!m_TimeLimitState) {
            if (m_SystemManager.m_Difficulty == 0) {
                for (int i = 0; i < 3; i++) {
                    pos[0] = m_FirePosition[0].position;
                    pos[1] = m_FirePosition[1].position;
                    CreateBulletsSector(4, pos[0], 5.4f, 0f, accel, 3, 29f);
                    CreateBulletsSector(4, pos[1], 5.4f, 0f, accel, 3, 29f);
                    yield return new WaitForSeconds(0.4f);
                }
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 4; i++) {
                    pos[0] = m_FirePosition[0].position;
                    pos[1] = m_FirePosition[1].position;
                    CreateBulletsSector(4, pos[0], 5.4f, 0f, accel, 5, 21f);
                    CreateBulletsSector(4, pos[1], 5.4f, 0f, accel, 5, 21f);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            else {
                for (int i = 0; i < 5; i++) {
                    pos[0] = m_FirePosition[0].position;
                    pos[1] = m_FirePosition[1].position;
                    CreateBulletsSector(4, pos[0], 5.4f, 0f, accel, 5, 21f);
                    CreateBulletsSector(4, pos[1], 5.4f, 0f, accel, 5, 21f);
                    yield return new WaitForSeconds(0.24f);
                }
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
        yield return null;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector2(0.8f, 0.24f));
        ExplosionEffect(0, -1, new Vector2(-0.8f, 0.24f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
