using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EnemyMiddleBoss1 : EnemyUnit
{
    public EnemyMiddleBoss1Turret[] m_Turret = new EnemyMiddleBoss1Turret[2];
    [HideInInspector] public sbyte m_Phase;
    
    private Vector3 m_TargetPosition;
    private Quaternion m_TargetQuaternion;
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 2f;

    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        float time_limit = 19f;
        DisableAttackable(m_AppearanceTime);

        m_UpdateTransform = false;
        m_TargetPosition = new Vector3(0f, -5f, Depth.ENEMY);
        m_TargetQuaternion = Quaternion.identity;
        transform.rotation = Quaternion.Euler(0f, 36f, 20f);
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMove(m_TargetPosition, m_AppearanceTime).SetEase(Ease.OutQuad))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion, m_AppearanceTime).SetEase(Ease.InQuad));
        
        m_CurrentPattern1 = PatternA(m_SystemManager.m_Difficulty);
        StartCoroutine(m_CurrentPattern1);
        Invoke("TimeLimit", m_AppearanceTime + time_limit);
        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.4f) { // 체력 40% 이하
                ToNextPhase();
            }
        }
        
        if (!m_TimeLimitState) {
            if (!m_IsUnattackable) {
                if (transform.position.x > m_TargetPosition.x + 2f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
                    transform.position = new Vector3(m_TargetPosition.x + 2f, transform.position.y, transform.position.z);
                }
                if (transform.position.x < m_TargetPosition.x - 2f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
                    transform.position = new Vector3(m_TargetPosition.x - 2f, transform.position.y, transform.position.z);
                }
                if (transform.position.y > m_TargetPosition.y + 0.6f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                    transform.position = new Vector3(transform.position.x, m_TargetPosition.y + 0.6f, transform.position.z);
                }
                if (transform.position.y < m_TargetPosition.y - 0.6f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                    transform.position = new Vector3(transform.position.x, m_TargetPosition.y - 0.6f, transform.position.z);
                }
            }
        }

        base.Update();
    }

    private void OnAppearanceComplete() {
        float[] random_direction = { 70f, 110f, -70f, -110f };
        m_MoveVector = new MoveVector(0.8f, random_direction[Random.Range(0, 4)]);
        m_UpdateTransform = true;
        m_Phase = 1;
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
        m_UpdateTransform = false;
        transform.DOMoveX(-20f, 4f).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(Quaternion.Euler(0f, 30f, 0f), 3f).SetEase(Ease.Linear);
    }

    private void ToNextPhase() {
        m_Phase++;
        m_SystemManager.EraseBullets(1f);
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        
        m_CurrentPattern2 = PatternB();
        if (m_SystemManager.m_Difficulty == 2)
            StartCoroutine(m_CurrentPattern2);

        m_Turret[0].StopPattern();
        m_Turret[1].StopPattern();

        m_Turret[0].StartPattern();
        m_Turret[1].StartPattern();

        ExplosionEffect(2, -1, new Vector2(2f, 0f));
        ExplosionEffect(2, 1, new Vector2(-2f, 0f));
    }

    private IEnumerator PatternA(byte difficulty) {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(1f);
        if (difficulty == 0) {
            while(true) {
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 5, 17f);
                yield return new WaitForSeconds(1.52f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 6, 17f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 6, 17f);
                yield return new WaitForSeconds(1.52f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 7, 17f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 7, 17f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 7, 17f);
                yield return new WaitForSeconds(3.12f); // --------------------------
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 24, 15f);
                yield return new WaitForSeconds(1.2f);
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 24, 15f);
                yield return new WaitForSeconds(3.6f);
            }
        }
        else if (difficulty == 1) {
            while(true) {
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 8, 10f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 8, 10f);
                yield return new WaitForSeconds(1.4f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 9, 10f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 9, 10f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 9, 10f);
                yield return new WaitForSeconds(1.4f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 10f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 10f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 10f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 10f);
                yield return new WaitForSeconds(2.2f); // --------------------------
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 40, 9f);
                yield return new WaitForSeconds(0.6f);
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 40, 9f);
                yield return new WaitForSeconds(0.6f);
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 40, 9f);
                yield return new WaitForSeconds(2.4f);
            }
        }
        else {
            while(true) {
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 8f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 8f);
                yield return new WaitForSeconds(1.4f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 11, 8f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 11, 8f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 11, 8f);
                yield return new WaitForSeconds(1.4f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 12, 8f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 12, 8f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 12, 8f);
                yield return new WaitForSeconds(0.12f);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 12, 8f);
                yield return new WaitForSeconds(2f); // --------------------------
                CreateBulletsSector(0, transform.position, 6.6f, 0f, accel, 45, 8f);
                yield return new WaitForSeconds(0.6f);
                CreateBulletsSector(0, transform.position, 6.6f, 0f, accel, 45, 8f);
                yield return new WaitForSeconds(0.6f);
                CreateBulletsSector(0, transform.position, 6.6f, 0f, accel, 45, 8f);
                yield return new WaitForSeconds(2.2f);
            }
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
        m_MoveVector = new MoveVector(1f, 0f);
        m_Phase = -1;

        StartCoroutine(DeathExplosion1(1.5f));
        StartCoroutine(DeathExplosion2(1.5f));

        yield return new WaitForSeconds(1.6f);
        ExplosionEffect(2, 2); // 최종 파괴
        ExplosionEffect(0, -1, new Vector2(3f, 0f));
        ExplosionEffect(0, -1, new Vector2(-3f, 0f));
        ExplosionEffect(0, -1, new Vector2(0f, 2f));
        ExplosionEffect(0, -1, new Vector2(0f, -1.5f));
        m_SystemManager.ScreenEffect(0);
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.5f);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(0, 0, random_pos);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(0, -1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.4f, 0.7f);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(1, 1, random_pos);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(1, -1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }
}
