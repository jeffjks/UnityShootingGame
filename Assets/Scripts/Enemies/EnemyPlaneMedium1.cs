using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyPlaneMedium1 : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[3];
    
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 1.6f;
    private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 0.2f;

    void Start ()
    {
        float time_limit = 10f;
        m_PositionY = transform.position.y;

        StartCoroutine(Pattern1());

        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -2.2f + m_VSpeed*m_AppearanceTime, m_AppearanceTime).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));

        Invoke("TimeLimit", m_AppearanceTime + time_limit);
    }

    protected override void Update()
    {
        m_AddPositionY -= m_VSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, m_PositionY + m_AddPositionY, transform.position.z);

        if (!m_TimeLimitState) {
            if (m_SystemManager.m_PlayState > 0) {
                CancelInvoke("TimeLimit");
                TimeLimit();
                m_Sequence.Kill();
                if (transform.position.x > 0f)
                    m_Sequence = DOTween.Sequence()
                    .Append(transform.DOMoveX(Size.GAME_BOUNDARY_RIGHT + 4f, 3f).SetEase(Ease.InQuad));
                else
                    m_Sequence = DOTween.Sequence()
                    .Append(transform.DOMoveX(Size.GAME_BOUNDARY_LEFT - 4f, 3f).SetEase(Ease.InQuad));
            }
        }
        
        base.Update();
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
    }

    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        EnemyBulletAccel accel1 = new EnemyBulletAccel(7.2f, 1f);
        yield return new WaitForSeconds(m_AppearanceTime);
        while(!m_TimeLimitState) {
            Vector3 pos0 = m_FirePosition[0].position;
            Vector3 pos1 = m_FirePosition[1].position;
            Vector3 pos2 = m_FirePosition[2].position;
            float target_angle = GetAngleToTarget(transform.position, m_PlayerManager.m_Player.transform.position);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(3, pos0, 6.4f, target_angle, accel, 6, 12);
                CreateBulletsSector(5, pos0, 5.2f, target_angle, accel, 5, 12);
                yield return new WaitForSeconds(1f);
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBullet(1, pos1, 6.0f, 0f, accel);
                    CreateBullet(1, pos2, 6.0f, 0f, accel);
                    yield return new WaitForSeconds(0.06f);
                }
                yield return new WaitForSeconds(1.5f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 3; i++) {
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBulletsSector(3, pos0, 6.4f, target_angle + Random.Range(-3f, 3f), accel, 9, 10f);
                    yield return new WaitForSeconds(0.2f);
                }
                yield return new WaitForSeconds(1f);
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBullet(1, pos1, 4.2f, 0f, accel1);
                    CreateBullet(1, pos2, 4.2f, 0f, accel1);
                    yield return new WaitForSeconds(0.06f);
                }
                yield return new WaitForSeconds(1.2f);
            }
            else {
                CreateBulletsSector(3, pos0, 6.4f, target_angle - 1.5f, accel, 10, 9f);
                CreateBulletsSector(3, pos0, 6.4f, target_angle + 1.5f, accel, 10, 9f);
                yield return new WaitForSeconds(0.2f);
                for (int i = 0; i < 4; i++) {
                    float random_value = Random.Range(-3f, 3f);
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBulletsSector(3, pos0, 6.4f, target_angle + random_value, accel, 9, 10f);
                    
                    yield return new WaitForSeconds(0.2f);
                }
                yield return new WaitForSeconds(0.1f);
                SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                CreateBulletsSector(5, pos0, 6.4f, target_angle, accel, 13, 8f);
                yield return new WaitForSeconds(1f);
                for (int i = 0; i < 4; i++) {
                    SetBulletVariables(ref pos0, ref pos1, ref pos2, ref target_angle);
                    CreateBullet(1, pos1, 4.2f, -40f, accel1);
                    CreateBullet(1, pos1, 4.2f, 0f, accel1);
                    CreateBullet(1, pos2, 4.2f, 0f, accel1);
                    CreateBullet(1, pos2, 4.2f, 40f, accel1);
                    yield return new WaitForSeconds(0.06f);
                }
                yield return new WaitForSeconds(1.2f);
            }
        }
        yield break;
    }

    private void SetBulletVariables(ref Vector3 pos0, ref Vector3 pos1, ref Vector3 pos2, ref float target_angle) {
        pos0 = m_FirePosition[0].position;
        pos1 = m_FirePosition[1].position;
        pos2 = m_FirePosition[2].position;
        target_angle = GetAngleToTarget(transform.position, m_PlayerManager.m_Player.transform.position);
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector2(2f, 0f));
        ExplosionEffect(0, -1, new Vector2(-2f, 0f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
