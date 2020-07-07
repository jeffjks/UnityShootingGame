using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyPlaneLarge1 : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay1 = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private float[] m_FireDelay2 = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private GameObject[] m_Part = new GameObject[2];
    [SerializeField] private EnemyPlaneLarge1Turret[] m_Turret = new EnemyPlaneLarge1Turret[2];
    [SerializeField] private Transform m_FirePosition = null;
    
    private Vector3 m_TargetPosition;
    private Quaternion m_TargetQuaternion;
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 2.3f;
    private sbyte m_Phase;

    void Start ()
    {
        float time_limit = 20f;
        DisableAttackable(m_AppearanceTime);

        m_UpdateTransform = false;
        m_TargetPosition = new Vector3(0f, -5.5f, Depth.ENEMY);
        m_TargetQuaternion = Quaternion.identity;
        transform.rotation = Quaternion.Euler(0f, 30f, 0f);
        
        m_Sequence.Append(transform.DOMove(m_TargetPosition, m_AppearanceTime).SetEase(Ease.OutQuad));
        m_Sequence.Join(transform.DORotateQuaternion(m_TargetQuaternion, m_AppearanceTime).SetEase(Ease.InQuad));
        
        Invoke("TimeLimit", m_AppearanceTime + time_limit);
        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }
        
        base.Update();
    }

    private void OnAppearanceComplete() {
        m_UpdateTransform = true;
        m_Phase = 1;
        
        m_CurrentPattern1 = PatternA();
        StartCoroutine(m_CurrentPattern1);
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
        m_UpdateTransform = false;
        transform.DOMoveX(-20f, 4f).SetEase(Ease.InQuad);
        transform.DORotateQuaternion(Quaternion.Euler(0f, 30f, 0f), 3f).SetEase(Ease.Linear);
    }

    private void ToNextPhase() {
        m_Phase++;
        m_Turret[0].OnDeath();
        m_Turret[1].OnDeath();
        Destroy(m_Part[0]);
        Destroy(m_Part[1]);

        StopCoroutine(m_CurrentPattern1);
        m_CurrentPattern2 = PatternB();
        StartCoroutine(m_CurrentPattern2);
    }
    
    private IEnumerator PatternA() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float target_angle, random_value;
        int state = Random.Range(-1, 1);
        if (state == 0) {
            state = 1;
        }
        yield return new WaitForSeconds(0.2f);

        while(!m_TimeLimitState) {
            if (m_SystemManager.m_Difficulty == 0) {
                random_value = Random.Range(-8f, 0f);
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                for (int i = 0; i < 4; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(3, pos, 5.6f + i*0.32f, target_angle + (random_value + i*2f)*state, accel, 5, 20f);
                    yield return new WaitForSeconds(0.07f);
                }
                yield return new WaitForSeconds(0.28f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                random_value = Random.Range(-8f, 0f);
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                for (int i = 0; i < 6; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(3, pos, 6f + i*0.4f, target_angle + (random_value + i*2f)*state, accel, 5, 20f);
                    yield return new WaitForSeconds(0.07f);
                }
                yield return new WaitForSeconds(0.14f);
            }
            else {
                random_value = Random.Range(-8f, 0f);
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                for (int i = 0; i < 8; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(3, pos, 5.6f + i*0.4f, target_angle + (random_value + i*2f)*state, accel, 5, 20f);
                    yield return new WaitForSeconds(0.07f);
                }
            }
            yield return new WaitForSeconds(m_FireDelay1[m_SystemManager.m_Difficulty]);
            state *= -1;


            if (m_SystemManager.m_Difficulty == 0) {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 6; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(0, pos, 6.2f, random_value + i*12f*state, accel, 8, 45f);
                    yield return new WaitForSeconds(0.15f);
                }
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 6; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(0, pos, 6.2f, random_value + i*9f*state, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state + 28f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state + 14f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state - 14f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state - 28f, accel, 4, 90f);
                    yield return new WaitForSeconds(0.15f);
                }
            }
            else {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 6; i++) {
                    pos = m_FirePosition.position;
                    CreateBulletsSector(0, pos, 6.6f, random_value + i*9f*state, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state + 28f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state + 14f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state - 14f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state - 28f, accel, 4, 90f);
                    yield return new WaitForSeconds(0.15f);
                }
            }
            yield return new WaitForSeconds(0.2f);

            m_Turret[0].StartCoroutine("PatternA");
            m_Turret[1].StartCoroutine("PatternA");
            yield return new WaitForSeconds(m_FireDelay1[m_SystemManager.m_Difficulty]);
        }
        yield break;
    }
    
    private IEnumerator PatternB() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float target_angle, random_value;
        
        yield return new WaitForSeconds(0.8f);

        while(!m_TimeLimitState) {
            random_value = Random.Range(-3f, 3f);

            if (m_SystemManager.m_Difficulty == 0) {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                CreateBulletsSector(5, pos, 6.1f, target_angle + random_value, accel, 6, 16f);
                CreateBulletsSector(5, pos, 6.4f, target_angle + random_value, accel, 6, 16f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                CreateBulletsSector(5, pos, 5.6f, target_angle + random_value, accel, 6, 12f);
                CreateBulletsSector(5, pos, 6.1f, target_angle + random_value, accel, 12, 6f);
                CreateBulletsSector(5, pos, 6.6f, target_angle + random_value, accel, 6, 12f);
                CreateBulletsSector(5, pos, 7.1f, target_angle + random_value, accel, 6, 12f);
            }
            else {
                pos = m_FirePosition.position;
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                CreateBulletsSector(5, pos, 5.6f, target_angle + random_value, accel, 8, 12f);
                CreateBulletsSector(5, pos, 6.1f, target_angle + random_value, accel, 14, 6f);
                CreateBulletsSector(5, pos, 6.6f, target_angle + random_value, accel, 8, 12f);
                CreateBulletsSector(5, pos, 7.1f, target_angle + random_value, accel, 8, 12f);
            }
            yield return new WaitForSeconds(m_FireDelay2[m_SystemManager.m_Difficulty]);
        }
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        float timer = 0f, random_timer = 0f;
        Vector2 random_pos1, random_pos2, random_pos3;
        m_MoveVector = new MoveVector(1.2f, 0f);
        m_Phase = -1;

        while (timer < 0.6f) {
            random_timer = Random.Range(0.1f, 0.15f);
            random_pos1 = Random.insideUnitCircle * 2;
            random_pos2 = Random.insideUnitCircle * 1.5f;
            random_pos3 = Random.insideUnitCircle * 2f;
            ExplosionEffect(1, 0, new Vector2(0f, 3f) + random_pos1);
            ExplosionEffect(1, -1, new Vector2(0f, -1.8f) + random_pos2);
            ExplosionEffect(3, -1, new Vector2(0f, -0f) + random_pos3);
            yield return new WaitForSeconds(random_timer);
            timer += random_timer;
        }
        ExplosionEffect(0, 1, new Vector2(0f, 0f));
        ExplosionEffect(1, -1, new Vector2(1.5f, 3.5f));
        ExplosionEffect(1, -1, new Vector2(-1.5f, 3.5f));
        ExplosionEffect(1, -1, new Vector2(2f, 0f));
        ExplosionEffect(1, -1, new Vector2(-2f, 0f));
        ExplosionEffect(1, -1, new Vector2(0f, -1.8f));
        ExplosionEffect(2, -1, new Vector2(2f, -4.5f));
        ExplosionEffect(2, -1, new Vector2(-2f, -4.5f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
