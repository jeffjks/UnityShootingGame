using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EnemyMiddleBoss4 : EnemyUnit
{
    public EnemyMiddleBoss4Turret1 m_Turret1;
    public EnemyMiddleBoss4Turret2 m_Turret2;
    [HideInInspector] public sbyte m_Phase;
    
    private Vector3 m_TargetPosition;
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 1.5f;

    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        float time_limit = 19f;
        DisableAttackable(m_AppearanceTime);

        m_UpdateTransform = false;
        m_TargetPosition = new Vector3(0f, -5f, Depth.ENEMY);
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(m_TargetPosition.y, m_AppearanceTime).SetEase(Ease.OutQuad));
        
        m_CurrentPattern1 = PatternA(m_SystemManager.m_Difficulty);
        StartCoroutine(m_CurrentPattern1);
        Invoke("TimeLimit", m_AppearanceTime + time_limit);
        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void FixedUpdate()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.33f) { // 체력 33% 이하
                ToNextPhase();
            }
        }
        
        if (!m_TimeLimitState) {
            if (m_IsAttackable) {
                if (transform.position.x >= m_TargetPosition.x + 2.4f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
                }
                else if (transform.position.x <= m_TargetPosition.x - 2.4f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
                }
                else if (transform.position.y >= m_TargetPosition.y + 0.5f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                }
                else if (transform.position.y <= m_TargetPosition.y - 0.5f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                }
            }
        }

        base.FixedUpdate();
    }

    private void OnAppearanceComplete() {
        float[] random_direction = { 80f, 100f, -80f, -100f };
        m_MoveVector = new MoveVector(0.8f, random_direction[Random.Range(0, 4)]);
        m_UpdateTransform = true;
        m_Phase = 1;
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
        m_UpdateTransform = false;
        transform.DOMoveY(12f, 4f).SetEase(Ease.InQuad);
    }

    private void ToNextPhase() {
        m_Phase++;
        m_SystemManager.EraseBullets(1f);
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        
        m_CurrentPattern2 = PatternB();
        if (m_SystemManager.m_Difficulty == 2)
            StartCoroutine(m_CurrentPattern2);

        ExplosionEffect(2, -1, new Vector2(2f, 0f));
        ExplosionEffect(2, 1, new Vector2(-2f, 0f));
    }

    private IEnumerator PatternA(byte difficulty) {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(1f);
        if (difficulty == 0) {
        }
        else if (difficulty == 1) {
        }
        else {
        }
    }

    private IEnumerator PatternB() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(1.8f);
        while(true) {
            float target_angle = GetAngleToTarget(transform.position, m_PlayerManager.m_Player.transform.position);
            CreateBulletsSector(0, transform.position, 7f, target_angle, accel, 8, 13f);
            yield return new WaitForSeconds(2f);
        }
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(1.4f, 0f);
        m_Phase = -1;

        StartCoroutine(DeathExplosion1(1.5f));
        StartCoroutine(DeathExplosion2(1.5f));

        yield return new WaitForSeconds(2f);
        ExplosionEffect(2, 2); // 최종 파괴
        ExplosionEffect(0, -1, new Vector2(3f, 0f));
        ExplosionEffect(0, -1, new Vector2(-3f, 0f));
        ExplosionEffect(0, -1, new Vector2(0f, 2f));
        ExplosionEffect(0, -1, new Vector2(0f, -1.5f));
        m_SystemManager.ScreenEffect(0);
        
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.4f);
            random_pos = new Vector2(Random.Range(-1.3f, 1.3f), Random.Range(-1.8f, 2.4f));
            ExplosionEffect(0, 0, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.5f, 0.8f);
            random_pos = new Vector2(Random.Range(-1.3f, 1.3f), Random.Range(-1.8f, 2.4f));
            ExplosionEffect(1, 1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }
}
