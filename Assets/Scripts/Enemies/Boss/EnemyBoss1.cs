using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss1 : EnemyUnit
{
    public EnemyBoss1Turret0 m_Turret0;
    public EnemyBoss1Turret1[] m_Turret1 = new EnemyBoss1Turret1[2];

    [HideInInspector] public byte m_Phase = 0;

    private Vector3[] m_TargetPosition = new Vector3[2];
    private Quaternion[] m_TargetQuaternion = new Quaternion[2];
    private float m_AppearanceTime = 2f;

    private IEnumerator m_Pattern1, m_Pattern2;

    void Start()
    {
        DisableAttackable(m_AppearanceTime);

        m_UpdateTransform = false;
        m_TargetPosition[0] = new Vector3(4f, -1f, Depth.ENEMY);
        m_TargetPosition[1] = new Vector3(0f, -5f, Depth.ENEMY);

        transform.rotation = Quaternion.Euler(0f, -35f, 0f);
        m_TargetQuaternion[0] = Quaternion.Euler(0f, 20f, 0f);
        m_TargetQuaternion[1] = Quaternion.identity;

        float appearance_time_1 = 0.55f;
        float appearance_time_2 = 1f - 0.55f;
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveX(3f, m_AppearanceTime*appearance_time_1).SetEase(Ease.OutQuad))
        .Join(transform.DOMoveY(-2f, m_AppearanceTime*appearance_time_1).SetEase(Ease.Linear))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[0], m_AppearanceTime*appearance_time_1).SetEase(Ease.InOutQuad))
        .Append(transform.DOMoveX(0f, m_AppearanceTime*appearance_time_2).SetEase(Ease.InOutQuad))
        .Join(transform.DOMoveY(-4.5f, m_AppearanceTime*appearance_time_2).SetEase(Ease.OutQuad))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[1], m_AppearanceTime*appearance_time_2).SetEase(Ease.InQuad));
        
        // m_Pattern1 = Pattern1(m_SystemManager.m_Difficulty);
        //StartCoroutine(m_Pattern1);

        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (m_Phase == 0) {
            if (m_Health <= m_MaxHealth * 0.4f) { // 체력 40% 이하
                // StopCoroutine(m_Pattern1);
                // m_Pattern2 = Pattern2();
                m_ChildEnemies[0].OnDeath();
                OnPhase1();
            }
        }

        base.Update();
    }

    private void OnAppearanceComplete() {
        m_Sequence.Kill();
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveX(-1f, 2f).SetEase(Ease.Linear))
        .Append(transform.DOMoveX(1f, 4f).SetEase(Ease.Linear))
        .Append(transform.DOMoveX(0f, 2f).SetEase(Ease.Linear))
        .SetLoops(-1, LoopType.Restart);
    }

    private void OnPhase1() {
        m_Sequence.Kill();
        m_Sequence = DOTween.Sequence()
        .AppendInterval(1f)
        .Append(transform.DOMove(new Vector3(Random.Range(0f, 2f), Random.Range(4.5f, 5.5f)), 1.5f).SetEase(Ease.InOutQuad))
        .AppendInterval(1f)
        .Append(transform.DOMove(new Vector3(Random.Range(-2f, 0f), Random.Range(4.5f, 5.5f)), 1.5f).SetEase(Ease.InOutQuad))
        .SetLoops(-1, LoopType.Restart);
        //.Join(transform.DORotateQuaternion(m_TargetQuaternion[0], m_AppearanceTime*appearance_time_1).SetEase(Ease.InOutQuad))
    }


    

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(0.6f, 0f);
        m_Turret0.OnDeath();

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(DeathExplosion1(2.8f));
        StartCoroutine(DeathExplosion2(2.8f));

        yield return new WaitForSeconds(3.5f);
        ExplosionEffect(2, 2); // 최종 파괴
        ExplosionEffect(1, -1, new Vector2(1.2f, 2.2f));
        ExplosionEffect(1, -1, new Vector2(-1.2f, 2.2f));
        ExplosionEffect(0, -1, new Vector2(2f, 0f));
        ExplosionEffect(0, -1, new Vector2(-2f, 0f));
        ExplosionEffect(1, -1, new Vector2(1.2f, -2.2f));
        ExplosionEffect(1, -1, new Vector2(-1.2f, -2.2f));
        m_SystemManager.ScreenEffect(1);
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, random_t = 0f;
        Vector2 random_pos;
        while (t < timer) {
            random_t = Random.Range(0.35f, 0.5f);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(0, 0, random_pos);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(0, -1, random_pos);
            yield return new WaitForSeconds(random_t);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, random_t = 0f;
        Vector2 random_pos;
        while (t < timer) {
            random_t = Random.Range(0.15f, 0.3f);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(1, 1, random_pos);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(1, -1, random_pos);
            yield return new WaitForSeconds(random_t);
        }
        yield return null;
    }
}
