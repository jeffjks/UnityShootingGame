using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2 : EnemyUnit
{
    public EnemyUnit[] m_Part1_Turrets = new EnemyUnit[3];
    public EnemyUnit[] m_Part2_Turrets = new EnemyUnit[4];
    public EnemyUnit[] m_Part3_Turrets = new EnemyUnit[4];
    public int m_NextPhaseDelay;

    [HideInInspector] public int m_Phase;
    
    private Vector3 m_TargetPosition;
    private const int APPEARNCE_TIME = 11000;
    private bool m_InPattern = false;

    private IEnumerator m_CurrentPhase, m_CurrentPattern;

    void Start()
    {
        m_TargetPosition = transform.position;
        
        for (int i = 0; i < m_Part1_Turrets.Length; i++) {
            m_Part1_Turrets[i].DisableAttackable();
        }
        for (int i = 0; i < m_Part2_Turrets.Length; i++) {
            m_Part2_Turrets[i].DisableAttackable();
        }
        for (int i = 0; i < m_Part3_Turrets.Length; i++) {
            m_Part3_Turrets[i].DisableAttackable();
        }

        StartCoroutine(AppearanceSequence());
    }

    protected override void Update()
    {
        if (m_Phase > 0) {
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
        }

        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 625 / 1000) { // 체력 62.5% 이하
                for (int i = 0; i < m_Part1_Turrets.Length; i++) {
                    m_Part1_Turrets[i].OnDeath();
                }
                m_SystemManager.EraseBullets(2000);
                ToNextPhase(m_NextPhaseDelay);
            }
        }
        else if (m_Phase == 2) {
            if (m_Health <= m_MaxHealth / 4) { // 체력 25% 이하
                for (int i = 0; i < m_Part2_Turrets.Length; i++) {
                    m_Part2_Turrets[i].OnDeath();
                }
                m_SystemManager.EraseBullets(2000);
                ToNextPhase(m_NextPhaseDelay);
            }
        }

        base.Update();
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARNCE_TIME);
        OnAppearanceComplete();
        yield break;
    }

    private void OnAppearanceComplete() {
        float random_direction = Random.Range(70f, 110f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.5f, random_direction);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        for (int i = 0; i < m_Part1_Turrets.Length; i++) {
            m_Part1_Turrets[i].EnableAttackable();
        }
    }

    public void ToNextPhase(int duration) {
        if (m_Phase == -1)
            return;

        m_Phase++;
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        if (m_Phase == 2) { // Phase 1 to 2
            for (int i = 0; i < m_Part2_Turrets.Length; i++) {
                m_Part2_Turrets[i].DisableAttackable(duration);
            }
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
            StartCoroutine(m_SystemManager.MoveBackgroundCameraDuration(true, 13f, duration));
        }
        else if (m_Phase == 3) { // Phase 2 to 3
            m_Collider2D[0].gameObject.SetActive(true);
            //DisableAttackable();
            DisableAttackable(duration);

            m_CurrentPhase = Phase3();
            StartCoroutine(m_CurrentPhase);
            StartCoroutine(m_SystemManager.MoveBackgroundCameraDuration(true, -7.5f, duration));
        }
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StartPattern(0);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StartPattern(0);
        yield return new WaitForMillisecondFrames(1000);
        int[] side = {-1, 1};
        int random_value;
        while(m_Phase == 1) {
            random_value = Random.Range(0, 2);
            m_CurrentPattern = Pattern1A();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).PrepareRotate(0, side[random_value]);
            ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).PrepareRotate(0, side[random_value]);
            random_value = Random.Range(0, 2);
            yield return new WaitForMillisecondFrames(1200);
                
            m_CurrentPattern = Pattern1B();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            
            ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).PrepareRotate(0, side[random_value]);
            ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).PrepareRotate(0, side[1 - random_value]);
            yield return new WaitForMillisecondFrames(1500);

            m_CurrentPattern = Pattern1C();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            yield return new WaitForMillisecondFrames(2000);
        }
        yield break;
    }

    private IEnumerator Pattern1A() {
        m_InPattern = true;
        
        ((EnemyBoss2Turret0_0) m_Part1_Turrets[0]).StartPattern(1);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StartPattern(1);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StartPattern(1);
        while(((EnemyBoss2Turret0_0)m_Part1_Turrets[0]).m_InPattern) {
            yield return new WaitForMillisecondFrames(0);
        }
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StopPattern();
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StopPattern();
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1B() {
        m_InPattern = true;
        ((EnemyBoss2Turret0_0) m_Part1_Turrets[0]).StartPattern(2);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StartPattern(2);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StartPattern(2);
        yield return new WaitForMillisecondFrames(6000);
        ((EnemyBoss2Turret0_0) m_Part1_Turrets[0]).StopPattern();
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StopPattern();
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StopPattern();

        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1C() {
        m_InPattern = true;
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StartPattern(3);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StartPattern(3);
        yield return new WaitForMillisecondFrames(500);

        m_InPattern = false;
        yield break;
    }

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        ((EnemyBoss2Turret1_2) m_Part2_Turrets[2]).StartPattern(0);
        ((EnemyBoss2Turret1_2) m_Part2_Turrets[3]).StartPattern(0);
        yield return new WaitForMillisecondFrames(5000);
        while(m_Phase == 2) {
            m_CurrentPattern = Pattern2A();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            yield return new WaitForMillisecondFrames(1500);

            m_CurrentPattern = Pattern2B();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            yield return new WaitForMillisecondFrames(1000);
        }
        yield break;
    }

    private IEnumerator Pattern2A() {
        m_InPattern = true;
        ((EnemyBoss2Turret1_1) m_Part2_Turrets[1]).StartPattern(1);
        if (m_PlayerManager.m_Player.transform.position.x < 0f) {
            ((EnemyBoss2Turret1_2) m_Part2_Turrets[2]).StartPattern(1, true);
           ((EnemyBoss2Turret1_2) m_Part2_Turrets[3]).StartPattern(1, false);
        }
        else {
            ((EnemyBoss2Turret1_2) m_Part2_Turrets[2]).StartPattern(1, false);
           ((EnemyBoss2Turret1_2) m_Part2_Turrets[3]).StartPattern(1, true);
        }
        while(((EnemyBoss2Turret1_1) m_Part2_Turrets[1]).m_InPattern) {
            yield return new WaitForMillisecondFrames(0);
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern2B() {
        m_InPattern = true;
        ((EnemyBoss2Turret1_0) m_Part2_Turrets[0]).StartPattern(1);
        while(((EnemyBoss2Turret1_0) m_Part2_Turrets[0]).m_InPattern) {
            yield return new WaitForMillisecondFrames(0);
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Phase3() { // 페이즈3 패턴 ============================
        yield return new WaitForMillisecondFrames(5000);
        ((EnemyBoss2Turret2_0) m_Part3_Turrets[0]).StartPattern(1);
        ((EnemyBoss2Turret2_1) m_Part3_Turrets[1]).StartPattern();
        ((EnemyBoss2Turret2_2) m_Part3_Turrets[2]).StartPattern();
        ((EnemyBoss2Turret2_2) m_Part3_Turrets[3]).StartPattern();
        while (m_Health >= 7000)
            yield return new WaitForMillisecondFrames(0);
        ((EnemyBoss2Turret2_0) m_Part3_Turrets[0]).StopPattern();
        yield return new WaitForMillisecondFrames(500);
        ((EnemyBoss2Turret2_0) m_Part3_Turrets[0]).StartPattern(2);
        yield break;
    }



    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_Phase = -1;
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        m_SystemManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0f, 0f);

        for (int i = 0; i < m_Part3_Turrets.Length; i++) {
            m_Part3_Turrets[i].OnDeath();
        }
        
        yield return new WaitForMillisecondFrames(1500);

        StartCoroutine(DeathExplosion1(2800));
        StartCoroutine(DeathExplosion2(2800));
        StartCoroutine(DeathExplosion3(2800));

        yield return new WaitForMillisecondFrames(3100);
        ExplosionEffect(2, 2, new Vector3(0f, 0f, -3f)); // 최종 파괴
        ExplosionEffect(1, -1, new Vector3(2f, 0f, 0f));
        ExplosionEffect(1, -1, new Vector3(-2f, 0f, 0f));
        ExplosionEffect(0, -1, new Vector3(3f, 0f, -3f));
        ExplosionEffect(0, -1, new Vector3(-3f, 0f, -3f));
        ExplosionEffect(1, -1, new Vector3(2f, 0f, -6f));
        ExplosionEffect(1, -1, new Vector3(-2f, 0f, -6f));
        m_SystemManager.ScreenEffect(1);
        m_SystemManager.ShakeCamera(1f);
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(450, 600);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, 0, new Vector3(random_pos.x, 5.5f, random_pos.y + 4f));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, -1, new Vector3(random_pos.x, 5.5f, random_pos.y + 4f));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(250, 350);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, 1, new Vector3(random_pos.x, 5.5f, random_pos.y - 2f));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, -1, new Vector3(random_pos.x, 5.5f, random_pos.y - 2f));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion3(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(250, 450);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, -1, new Vector3(random_pos.x, 5.5f, random_pos.y - 8f));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, -1, new Vector3(random_pos.x, 5.5f, random_pos.y - 8f));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }
}
