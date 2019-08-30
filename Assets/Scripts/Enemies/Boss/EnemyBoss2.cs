using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss2 : EnemyUnit
{
    public EnemyBoss2Turret0_0 m_Turret0_0;
    public EnemyBoss2Turret0_1[] m_Turret0_1 = new EnemyBoss2Turret0_1[2];
    public EnemyBoss2Turret1_0 m_Turret1_0;
    public EnemyBoss2Turret1_1 m_Turret1_1;
    public EnemyBoss2Turret1_2[] m_Turret1_2 = new EnemyBoss2Turret1_2[2];
    public EnemyBoss2Turret2_0 m_Turret2_0;
    public EnemyBoss2Turret2_1 m_Turret2_1;
    public EnemyBoss2Turret2_2[] m_Turret2_2 = new EnemyBoss2Turret2_2[2];

    public EnemyBoss2Part[] m_Part = new EnemyBoss2Part[2];

    [HideInInspector] public sbyte m_Phase;
    
    private Vector3 m_TargetPosition;
    private float m_AppearanceTime = 10f;
    private bool m_InPattern = false;

    private IEnumerator m_CurrentPhase, m_CurrentPattern;

    void Start()
    {
        m_Phase = -1;
        m_TargetPosition = transform.position;
        ToNextPhase(m_AppearanceTime);

        for (int i = 0; i < m_Part[1].m_Turret.Length; i++) {
            m_Part[1].m_Turret[i].DisableAttackable(-1f);
        }

        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (transform.position.x >= m_TargetPosition.x + 1f) {
            m_MoveVector.direction = Random.Range(-110f, -70f);
        }
        else if (transform.position.x <= m_TargetPosition.x - 1f) {
            m_MoveVector.direction = Random.Range(70f, 110f);
        }
        else if (transform.position.z >= m_TargetPosition.z + 0.3f) {
            m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
        }
        else if (transform.position.z <= m_TargetPosition.z - 0.3f) {
            m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
        }

        base.Update();
    }

    private void OnAppearanceComplete() {
        float random_direction = Random.Range(70f, 110f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.5f, random_direction);
        m_CurrentPhase = PatternPhase0();
        StartCoroutine(m_CurrentPhase);
    }

    public void ToNextPhase(float duration) {
        m_Phase++;
        if (m_Phase > 0) {
            StopCoroutine(m_CurrentPattern);
            StopCoroutine(m_CurrentPhase);

            if (m_Phase == 1) {
                m_CurrentPhase = PatternPhase1();
                StartCoroutine(m_CurrentPhase);
            }
            else if (m_Phase == 2) {
                m_CurrentPhase = PatternPhase2();
                StartCoroutine(m_CurrentPhase);
            }
        }
        if (m_Phase <= 1) {
            for (int i = 0; i < m_Part[m_Phase].m_Turret.Length; i++) {
                m_Part[m_Phase].m_Turret[i].DisableAttackable(duration);
            }
        }
        else {
            m_Collider2D[0].gameObject.SetActive(true);
            DisableAttackable(-1f);
            DisableAttackable(duration);
        }
        if (m_Phase == 1) {
            m_SystemManager.m_BackgroundCamera.transform.DOMoveZ(m_SystemManager.m_BackgroundCamera.transform.position.z + 13f, duration).SetEase(Ease.InOutQuad);
        }
        else if (m_Phase == 2) {
            m_SystemManager.m_BackgroundCamera.transform.DOMoveZ(m_SystemManager.m_BackgroundCamera.transform.position.z - 7.5f, duration).SetEase(Ease.InOutQuad);
        }
    }

    private IEnumerator PatternPhase0() { // 페이즈0 패턴 ============================
        m_Turret0_1[0].StartPattern(0);
        m_Turret0_1[1].StartPattern(0);
        yield return new WaitForSeconds(1f);
        int[] side = {-1, 1};
        int random_value;
        while(m_Phase == 0) {
            random_value = Random.Range(0, 2);
            m_CurrentPattern = Pattern1();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
            m_Turret0_1[0].PrepareRotate(0, side[random_value]);
            m_Turret0_1[1].PrepareRotate(0, side[random_value]);
            random_value = Random.Range(0, 2);
            yield return new WaitForSeconds(1.2f);
                
            m_CurrentPattern = Pattern2();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
            
            m_Turret0_1[0].PrepareRotate(0, side[random_value]);
            m_Turret0_1[1].PrepareRotate(0, side[1 - random_value]);
            yield return new WaitForSeconds(1.5f);

            m_CurrentPattern = Pattern3();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
            yield return new WaitForSeconds(2f);
        }
        yield return null;
    }

    private IEnumerator Pattern1() {
        m_InPattern = true;
        m_Turret0_0.StartPattern(1);
        m_Turret0_1[0].StartPattern(1);
        m_Turret0_1[1].StartPattern(1);
        while(m_Turret0_0.m_InPattern) {
            yield return null;
        }
        m_Turret0_1[0].StopPattern();
        m_Turret0_1[1].StopPattern();
        m_InPattern = false;
        yield return null;
    }

    private IEnumerator Pattern2() {
        m_InPattern = true;
        m_Turret0_0.StartPattern(2);
        m_Turret0_1[0].StartPattern(2);
        m_Turret0_1[1].StartPattern(2);
        yield return new WaitForSeconds(6f);
        m_Turret0_0.StopPattern();
        m_Turret0_1[0].StopPattern();
        m_Turret0_1[1].StopPattern();

        m_InPattern = false;
        yield return null;
    }

    private IEnumerator Pattern3() {
        m_InPattern = true;
        m_Turret0_1[0].StartPattern(3);
        m_Turret0_1[1].StartPattern(3);
        yield return new WaitForSeconds(0.5f);

        m_InPattern = false;
        yield return null;
    }

    private IEnumerator PatternPhase1() { // 페이즈1 패턴 ============================
        yield return new WaitForSeconds(5f);
        m_Turret1_2[0].StartPattern(0);
        m_Turret1_2[1].StartPattern(0);
        while(m_Phase == 1) {
            m_CurrentPattern = Pattern4();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
            yield return new WaitForSeconds(1.5f);

            m_CurrentPattern = Pattern5();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    private IEnumerator Pattern4() {
        m_InPattern = true;
        m_Turret1_1.StartPattern(1);
        if (m_PlayerManager.m_Player.transform.position.x < 0f) {
            m_Turret1_2[0].StartPattern(1, true);
            m_Turret1_2[1].StartPattern(1, false);
        }
        else {
            m_Turret1_2[0].StartPattern(1, false);
            m_Turret1_2[1].StartPattern(1, true);
        }
        while(m_Turret1_1.m_InPattern) {
            yield return null;
        }
        m_InPattern = false;
        yield return null;
    }

    private IEnumerator Pattern5() {
        m_InPattern = true;
        m_Turret1_0.StartPattern(1);
        while(m_Turret1_0.m_InPattern) {
            yield return null;
        }
        m_InPattern = false;
        yield return null;
    }

    private IEnumerator PatternPhase2() { // 페이즈1 패턴 ============================
        yield return new WaitForSeconds(5f);
        m_Turret2_0.StartPattern(1);
        m_Turret2_1.StartPattern();
        m_Turret2_2[0].StartPattern();
        m_Turret2_2[1].StartPattern();
        while (m_Health >= 700f)
            yield return null;
        m_Turret2_0.StopPattern();
        yield return new WaitForSeconds(0.5f);
        m_Turret2_0.StartPattern(2);
        yield return null;
    }



    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        StopCoroutine(m_CurrentPattern);
        StopCoroutine(m_CurrentPhase);
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(0f, 0f);
        m_Part[2].OnDeath();
        
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(DeathExplosion1(2.8f));
        StartCoroutine(DeathExplosion2(2.8f));
        StartCoroutine(DeathExplosion3(2.8f));

        yield return new WaitForSeconds(3.1f);
        ExplosionEffect(2, 2); // 최종 파괴
        ExplosionEffect(1, -1, new Vector2(1.2f, 2.2f));
        ExplosionEffect(1, -1, new Vector2(-1.2f, 2.2f));
        ExplosionEffect(0, -1, new Vector2(2f, 0f));
        ExplosionEffect(0, -1, new Vector2(-2f, 0f));
        ExplosionEffect(1, -1, new Vector2(1.2f, -2.2f));
        ExplosionEffect(1, -1, new Vector2(-1.2f, -2.2f));
        m_SystemManager.ScreenEffect(1);
        
        CreateItems();
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.45f, 0.6f);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, 0, new Vector3(random_pos.x, 5.5f, random_pos.y + 4f));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, -1, new Vector3(random_pos.x, 5.5f, random_pos.y + 4f));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.25f, 0.35f);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, 1, new Vector3(random_pos.x, 5.5f, random_pos.y - 2f));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, -1, new Vector3(random_pos.x, 5.5f, random_pos.y - 2f));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion3(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.35f, 0.45f);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, -1, new Vector3(random_pos.x, 5.5f, random_pos.y - 8f));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, -1, new Vector3(random_pos.x, 5.5f, random_pos.y - 8f));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }
}
