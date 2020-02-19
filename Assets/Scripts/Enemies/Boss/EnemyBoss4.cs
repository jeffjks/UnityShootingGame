using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss4 : EnemyUnit
{
    public EnemyUnit[] m_SmallTurrets = new EnemyUnit[4];
    public EnemyUnit[] m_FrontTurrets = new EnemyUnit[2];
    public EnemyUnit m_MainTurret;
    public EnemyUnit[] m_SubTurrets = new EnemyUnit[2];
    public Transform[] m_FirePosition = new Transform[2];

    [HideInInspector] public sbyte m_Phase;
    
    private Vector3 m_TargetPosition;
    private float m_AppearanceTime = 5f;
    private bool m_InPattern = false;
    private int m_MoveDirection;
    private float m_MoveSpeed, m_DefaultSpeed = 0.005f;
    private float m_Direction;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2, m_CurrentPattern3;

    void Start()
    {
        m_TargetPosition = transform.position;
        m_MoveVector = new MoveVector(-4.5f, 180f);
        
        DisableAttackable(m_AppearanceTime);
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            m_SmallTurrets[i].DisableAttackable(m_AppearanceTime);
        }
        for (int i = 0; i < m_FrontTurrets.Length; i++) {
            m_FrontTurrets[i].DisableAttackable(m_AppearanceTime);
        }

        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        transform.position = new Vector3(transform.position.x + m_MoveSpeed, transform.position.y, transform.position.z);
        if (m_MoveVector.speed < 0f) {
            m_MoveVector.speed += Time.deltaTime*1f;
        }
        else {
            m_MoveVector.speed = 0f;
        }

        if (m_Phase > 0) {
            if (transform.position.x >= m_TargetPosition.x + 0.7f) {
                m_MoveDirection = -1;
            }
            else if (transform.position.x <= m_TargetPosition.x - 0.7f) {
                m_MoveDirection = 1;
            }
            if (m_MoveDirection == 1) {
                if (m_MoveSpeed < m_DefaultSpeed) {
                    m_MoveSpeed += Time.deltaTime*m_MoveDirection*0.01f;
                }
            }
            else if (m_MoveDirection == -1) {
                if (m_MoveSpeed > -m_DefaultSpeed) {
                    m_MoveSpeed += Time.deltaTime*m_MoveDirection*0.01f;
                }
            }
        }

        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.60f) { // 체력 60% 이하
                for (int i = 0; i < m_FrontTurrets.Length; i++) {
                    if (m_FrontTurrets[i] != null)
                        m_FrontTurrets[i].OnDeath();
                }
                m_SystemManager.EraseBullets(2f);
                ExplosionEffect(1, 2, new Vector3(0f, 2f, -4.2f));
                ExplosionEffect(0, -1, new Vector3(-2.8f, 2f, 0f));
                ExplosionEffect(0, -1, new Vector3(2.8f, 2f, 0f));
                ExplosionEffect(0, -1, new Vector3(-2.5f, 2f, 3.3f));
                ExplosionEffect(0, -1, new Vector3(2.5f, 2f, 3.3f));
                ExplosionEffect(0, -1, new Vector3(-2.8f, 2f, -3.2f));
                ExplosionEffect(0, -1, new Vector3(2.8f, 2f, -3.2f));
                for (int i = 0; i < m_SmallTurrets.Length; i++) {
                    if (m_SmallTurrets[i] != null)
                        ((EnemyBoss4SmallTurret) m_SmallTurrets[i]).StopPattern();
                }
                for (int i = 0; i < m_SubTurrets.Length; i++) {
                    if (m_SubTurrets[i] != null)
                        ((EnemyBoss4SubTurret) m_SubTurrets[i]).StopPattern();
                }
                ((EnemyBoss4MainTurret) m_MainTurret).StopPattern();
                ToNextPhase();
            }
        }

        base.Update();
    }

    private void OnAppearanceComplete() {
        int random_speed = Random.Range(0, 2);
        m_MoveDirection = random_speed*2 - 1;
        m_MoveSpeed = m_DefaultSpeed*m_MoveDirection;
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
    }

    public void ToNextPhase() {
        ((EnemyBoss4SubTurret) m_SubTurrets[0]).m_RotatePattern = 10;
        ((EnemyBoss4SubTurret) m_SubTurrets[1]).m_RotatePattern = 10;
        m_Phase++;
        StopAllPatterns();
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        if (m_Phase == 2) {
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
        }
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            if (m_SmallTurrets[i] != null)
                ((EnemyBoss4SmallTurret) m_SmallTurrets[i]).StartPattern(1);
        }
        yield return new WaitForSeconds(1f);
        m_InPattern = true;
        m_CurrentPattern1 = Pattern1A1();
        StartCoroutine(m_CurrentPattern1);
        m_CurrentPattern2 = Pattern1A2();
        StartCoroutine(m_CurrentPattern2);
        while (m_InPattern)
            yield return null;

        StopAllPatterns();

        while(m_Phase == 1) {
            ((EnemyBoss4SubTurret) m_SubTurrets[0]).m_RotatePattern = 20;
            ((EnemyBoss4SubTurret) m_SubTurrets[1]).m_RotatePattern = 20;
            ((EnemyBoss4FrontTurret) m_FrontTurrets[0]).m_RotatePattern = 20;
            ((EnemyBoss4FrontTurret) m_FrontTurrets[1]).m_RotatePattern = 20;
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < m_SmallTurrets.Length; i++) {
                if (m_SmallTurrets[i] != null)
                    ((EnemyBoss4SmallTurret) m_SmallTurrets[i]).StopPattern();
            }

            m_CurrentPattern1 = Pattern1B1();
            StartCoroutine(m_CurrentPattern1);
            m_CurrentPattern2 = Pattern1B2_1();
            StartCoroutine(m_CurrentPattern2);
            
            yield return new WaitForSeconds(2f);
            ((EnemyBoss4MainTurret) m_MainTurret).StartPattern(1);
            yield return new WaitForSeconds(6f);
            ((EnemyBoss4MainTurret) m_MainTurret).StopPattern();
            ((EnemyBoss4SubTurret) m_SubTurrets[0]).m_RotatePattern = 10;
            ((EnemyBoss4SubTurret) m_SubTurrets[1]).m_RotatePattern = 10;
            ((EnemyBoss4FrontTurret) m_FrontTurrets[0]).m_RotatePattern = 10;
            ((EnemyBoss4FrontTurret) m_FrontTurrets[1]).m_RotatePattern = 10;
            StopAllPatterns();
            yield return new WaitForSeconds(1f);
            ((EnemyBoss4SubTurret) m_SubTurrets[0]).m_RotatePattern = 31;
            ((EnemyBoss4SubTurret) m_SubTurrets[1]).m_RotatePattern = 32;
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < m_SmallTurrets.Length; i++) {
                if (m_SmallTurrets[i] != null)
                    ((EnemyBoss4SmallTurret) m_SmallTurrets[i]).StartPattern(1);
            }
            yield return new WaitForSeconds(1f);

            ((EnemyBoss4SubTurret) m_SubTurrets[0]).m_RotatePattern = 30;
            ((EnemyBoss4SubTurret) m_SubTurrets[1]).m_RotatePattern = 30;
            ((EnemyBoss4SubTurret) m_SubTurrets[0]).StartPattern(3);
            ((EnemyBoss4SubTurret) m_SubTurrets[1]).StartPattern(3);
            m_CurrentPattern1 = Pattern1C1();
            StartCoroutine(m_CurrentPattern1);
            m_CurrentPattern2 = Pattern1C2();
            StartCoroutine(m_CurrentPattern2);

            yield return new WaitForSeconds(5f);
            ((EnemyBoss4SubTurret) m_SubTurrets[0]).StopPattern();
            ((EnemyBoss4SubTurret) m_SubTurrets[1]).StopPattern();
            ((EnemyBoss4SubTurret) m_SubTurrets[0]).m_RotatePattern = 10;
            ((EnemyBoss4SubTurret) m_SubTurrets[1]).m_RotatePattern = 10;
            StopAllPatterns();

            yield return new WaitForSeconds(2f);
            m_InPattern = true;
            m_CurrentPattern1 = Pattern1D();
            StartCoroutine(m_CurrentPattern1);
            m_CurrentPattern2 = Pattern1A2();
            StartCoroutine(m_CurrentPattern2);
            while (m_InPattern)
                yield return null;

            StopAllPatterns();
        }
        yield break;
    }

    private IEnumerator Pattern1A1() {
        int r = Random.Range(0, 2);
        int[] n = {6, 12, 12};
        for (int i = 0; i < n[m_SystemManager.m_Difficulty]; i++) {
            ((EnemyBoss4SubTurret) m_SubTurrets[r]).StartPattern(1);
            
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(1f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForSeconds(0.5f);
            }
            else {
                yield return new WaitForSeconds(0.5f);
            }
            r = 1-r;
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1A2() {
        int r = Random.Range(0, 2);
        yield return new WaitForSeconds(1f);

        while (true) {
            if (m_FrontTurrets[r] != null) {
                ((EnemyBoss4FrontTurret) m_FrontTurrets[r]).StartPattern(1);
            }
            else if (m_FrontTurrets[1-r] != null) {
                ((EnemyBoss4FrontTurret) m_FrontTurrets[1-r]).StartPattern(1);
            }
            else {
                yield break;
            }
            
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(6f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForSeconds(3f);
            }
            else {
                yield return new WaitForSeconds(2f);
            }
            r = 1-r;
        }
    }

    private IEnumerator Pattern1B1() {
        int[] n = {2, 3, 4};

        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < m_SubTurrets.Length; j++) {
                if (m_SubTurrets[j] != null)
                    ((EnemyBoss4SubTurret) m_SubTurrets[j]).StartPattern(2);
            }
            for (int j = 0; j < m_FrontTurrets.Length; j++) {
                if (m_FrontTurrets[j] != null)
                    ((EnemyBoss4FrontTurret) m_FrontTurrets[j]).StartPattern(2);
            }
            
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(3.2f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForSeconds(2.4f);
            }
            else {
                yield return new WaitForSeconds(1.8f);
            }
        }
    }

    private IEnumerator Pattern1B2_1() {
        int r1 = Random.Range(0, 2), r2 = Random.Range(0, 2);

        while (true) {
            m_Direction = Random.Range(0f, 360f);
            m_CurrentPattern3 = Pattern1B2_2(r1);
            StartCoroutine(m_CurrentPattern3);
            
            for (int i = 0; i < 40; i++) {
                m_Direction += Time.deltaTime * 30f * (2*r2 - 1);
                yield return null;
            }
            for (int i = 0; i < 40; i++) {
                m_Direction -= Time.deltaTime * 30f * (2*r2 - 1);
                yield return null;
            }
            if (m_CurrentPattern3 != null) {
                StopCoroutine(m_CurrentPattern3);
            }
            r1 = 1 - r1;
            yield return null;
        }
    }

    private IEnumerator Pattern1B2_2(int rand) {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);

        while (true) {
            if (m_SystemManager.m_Difficulty == 0) {
                pos[rand] = GetScreenPosition(m_FirePosition[rand].position);
                CreateBulletsSector(4, pos[rand], 6.1f, m_Direction, accel, 4, 90f);
                yield return new WaitForSeconds(0.1f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos[rand] = GetScreenPosition(m_FirePosition[rand].position);
                CreateBulletsSector(4, pos[rand], 6.4f, m_Direction, accel, 5, 72f);
                yield return new WaitForSeconds(0.08f);
            }
            else {
                pos[rand] = GetScreenPosition(m_FirePosition[rand].position);
                CreateBulletsSector(4, pos[rand], 6.8f, m_Direction, accel, 6, 60f);
                yield return new WaitForSeconds(0.07f);
            }
        }
    }

    private IEnumerator Pattern1C1() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(4.8f, 0.5f);

        while (true) {
            int rand = Random.Range(0, 2);
            if (m_SystemManager.m_Difficulty == 0) {
                pos[rand] = GetScreenPosition(m_FirePosition[rand].position);
                CreateBulletsSector(2, pos[rand], 7f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForSeconds(1.8f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                pos[rand] = GetScreenPosition(m_FirePosition[rand].position);
                CreateBulletsSector(2, pos[rand], 7.6f, Random.Range(0f, 360f), accel, 36, 10f);
                yield return new WaitForSeconds(1.2f);
            }
            else {
                pos[rand] = GetScreenPosition(m_FirePosition[rand].position);
                CreateBulletsSector(2, pos[rand], 8f, Random.Range(0f, 360f), accel, 45, 8f);
                yield return new WaitForSeconds(0.8f);
            }
        }
    }

    private IEnumerator Pattern1C2() {
        int r = Random.Range(0, 2);
        yield return new WaitForSeconds(1f);

        while (true) {
            if (m_FrontTurrets[r] != null) {
                ((EnemyBoss4FrontTurret) m_FrontTurrets[r]).StartPattern(3);
            }
            else if (m_FrontTurrets[1-r] != null) {
                ((EnemyBoss4FrontTurret) m_FrontTurrets[1-r]).StartPattern(3);
            }
            else {
                yield break;
            }
            
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(6f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForSeconds(3f);
            }
            else {
                yield return new WaitForSeconds(2f);
            }
            r = 1-r;
        }
    }

    private IEnumerator Pattern1D() {
        int r = Random.Range(0, 2);
        int[] n = {6, 12, 12};
        for (int i = 0; i < n[m_SystemManager.m_Difficulty]; i++) {
            ((EnemyBoss4SubTurret) m_SubTurrets[r]).StartPattern(4);
            
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(0.8f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForSeconds(0.4f);
            }
            else {
                yield return new WaitForSeconds(0.32f);
            }
            r = 1-r;
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield break;
    }

    private IEnumerator Pattern2A() {
        yield break;
    }

    private IEnumerator Pattern2B() {
        yield break;
    }

    private IEnumerator Phase3() { // 페이즈3 패턴 ============================
        yield break;
    }


    private void StopAllPatterns() {
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
        if (m_CurrentPattern3 != null)
            StopCoroutine(m_CurrentPattern3);
    }



    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_Phase = -1;
        StopAllPatterns();
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            if (m_SmallTurrets[i] != null)
                m_SmallTurrets[i].OnDeath();
        }
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(0.5f, 0f);

        StartCoroutine(DeathExplosion1(3.2f));
        StartCoroutine(DeathExplosion2(3.2f));
        yield return new WaitForSeconds(0.8f);
        ExplosionEffect(3, 2, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        yield return new WaitForSeconds(0.8f);
        ExplosionEffect(3, 2, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        yield return new WaitForSeconds(0.8f);
        ExplosionEffect(3, 2, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 2f, Random.Range(-4.5f, 3.5f)));
        yield return new WaitForSeconds(0.8f);
        
        ExplosionEffect(4, 3, new Vector3(0f, 2f, 0f)); // 최종 파괴
        ExplosionEffect(3, -1, new Vector3(-2f, 2f, -2f), new MoveVector(1.6f, -45f));
        ExplosionEffect(3, -1, new Vector3(2f, 2f, -2f), new MoveVector(1.6f, 45f));
        ExplosionEffect(3, -1, new Vector3(2f, 2f, 2f), new MoveVector(1.6f, 135f));
        ExplosionEffect(3, -1, new Vector3(-2f, 2f, 2f), new MoveVector(1.6f, -135f));
        ExplosionEffect(4, -1, new Vector3(-3.6f, 3.6f, -3.6f));
        ExplosionEffect(4, -1, new Vector3(3.6f, 3.6f, -3.6f));
        ExplosionEffect(4, -1, new Vector3(3.6f, 3.6f, 3.6f));
        ExplosionEffect(4, -1, new Vector3(-3.6f, 3.6f, 3.6f));
        ExplosionEffect(3, -1, new Vector3(-3f, 2f, 0f), new MoveVector(1.2f, -90f));
        ExplosionEffect(3, -1, new Vector3(0f, 2f, -3f), new MoveVector(1.2f, 0f));
        ExplosionEffect(3, -1, new Vector3(3f, 2f, 0f), new MoveVector(1.2f, 90f));
        ExplosionEffect(3, -1, new Vector3(0f, 2f, 3f), new MoveVector(1.2f, 180f));
        m_SystemManager.ScreenEffect(1);
        m_SystemManager.ShakeCamera(1f);
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            t_add = Random.Range(0.25f, 0.35f);
            ExplosionEffect(1, 0, new Vector3(Random.Range(-4f, 4f), 2f, Random.Range(-5.4f, 4f)));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-4f, 4f), 2f, Random.Range(-5.4f, 4f)));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-4f, 4f), 2f, Random.Range(-5.4f, 4f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.6f);
            ExplosionEffect(0, 1, new Vector3(Random.Range(-4f, 4f), 2f, Random.Range(-5.4f, 4f)), new MoveVector(2f, Random.Range(0f, 360f)));
            ExplosionEffect(0, -1, new Vector3(Random.Range(-4f, 4f), 2f, Random.Range(-5.4f, 4f)), new MoveVector(2f, Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }
}
