using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss2 : EnemyUnit
{
    public EnemyBoss2Turret0_0 m_EnemyBoss2Turret0_0;
    public EnemyBoss2Turret0_1[] m_EnemyBoss2Turret0_1 = new EnemyBoss2Turret0_1[2];

    [HideInInspector] public byte m_Phase = 0;
    
    private float m_AppearanceTime = 4f;

    private IEnumerator m_CurrentPhase, m_CurrentPattern;

    void Start()
    {
        m_EnemyBoss2Turret0_0.DisableAttackable(m_AppearanceTime);
        m_EnemyBoss2Turret0_1[0].DisableAttackable(m_AppearanceTime);
        m_EnemyBoss2Turret0_1[1].DisableAttackable(m_AppearanceTime);
    }

    protected override void Update()
    {
        if (m_Phase == 0) {
            if (m_Health <= m_MaxHealth * 0.33f) { // 체력 33% 이하
                ToPhase1();
                m_ChildEnemies[0].OnDeath();
            }
        }

        base.Update();
    }

    private void ToPhase0() {
    }

    private void ToPhase1() {
    }


    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        StopCoroutine(m_CurrentPattern);
        StopCoroutine(m_CurrentPhase);
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(0.6f, 0f);

        StartCoroutine(DeathExplosion3(4.9f));
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
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.45f, 0.6f);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(0, 0, random_pos);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(0, -1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.15f, 0.3f);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(1, 1, random_pos);
            random_pos = (Vector2) Random.insideUnitCircle * 2.5f;
            ExplosionEffect(1, -1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion3(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            ExplosionEffect(0, -1, new Vector2(0f, 1.225f), new MoveVector(5f, 180f));
            t += t_add;
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }
}
