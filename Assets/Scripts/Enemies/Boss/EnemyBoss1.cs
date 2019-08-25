using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss1 : EnemyUnit
{
    public EnemyBoss1Turret0 m_Turret0;
    public EnemyBoss1Turret1[] m_Turret1 = new EnemyBoss1Turret1[2];
    public EnemyBoss1Turret2[] m_Turret2 = new EnemyBoss1Turret2[2];
    public EnemyBoss1Part m_Part;
    public Transform[] m_FirePosition = new Transform[4];

    [HideInInspector] public byte m_Phase = 0;

    private Vector3[] m_TargetPosition = new Vector3[2];
    private Quaternion[] m_TargetQuaternion = new Quaternion[2];
    private Quaternion m_QuaternionTurnRight, m_QuaternionTurnLeft;
    private float m_AppearanceTime = 2f;

    private IEnumerator m_CurrentPhase, m_CurrentPattern;
    private bool m_InPattern = false;

    void Start()
    {
        DisableAttackable(m_AppearanceTime);

        m_UpdateTransform = false;
        m_TargetPosition[0] = new Vector3(4f, -1f, Depth.ENEMY);
        m_TargetPosition[1] = new Vector3(0f, -5f, Depth.ENEMY);

        transform.rotation = Quaternion.Euler(0f, -35f, 0f);
        m_TargetQuaternion[0] = Quaternion.Euler(0f, 20f, 0f);
        m_TargetQuaternion[1] = Quaternion.identity;

        m_QuaternionTurnRight = Quaternion.Euler(0f, -15f, 0f);
        m_QuaternionTurnLeft = Quaternion.Euler(0f, 15f, 0f);
        
        // m_Pattern1 = Pattern1(m_SystemManager.m_Difficulty);

        OnPhase0();

        Invoke("OnAppearanceComplete", m_AppearanceTime);
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

    public void ToPhase1() {
        m_Phase = 1;
        StopCoroutine(m_CurrentPattern);
        StopCoroutine(m_CurrentPhase);
        m_Turret0.StopPattern();
        m_Turret1[0].StopPattern();
        m_Turret1[1].StopPattern();
        m_Turret2[0].StopPattern();
        m_Turret2[1].StopPattern();
        m_CurrentPhase = PatternPhase1();
        StartCoroutine(m_CurrentPhase);
        m_Sequence.Kill();
        OnPhase1();
    }

    private void OnAppearanceComplete() {
        m_Sequence.Kill();
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveX(-1f, 2f).SetEase(Ease.Linear))
        .Append(transform.DOMoveX(1f, 4f).SetEase(Ease.Linear))
        .Append(transform.DOMoveX(0f, 2f).SetEase(Ease.Linear))
        .SetLoops(-1, LoopType.Restart);
        m_CurrentPhase = PatternPhase0();
        StartCoroutine(m_CurrentPhase);
    }


    private void OnPhase0() {
        float appearance_time_1 = 0.55f;
        float appearance_time_2 = 1f - 0.55f;
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveX(3f, m_AppearanceTime*appearance_time_1).SetEase(Ease.OutQuad))
        .Join(transform.DOMoveY(-2f, m_AppearanceTime*appearance_time_1).SetEase(Ease.Linear))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[0], m_AppearanceTime*appearance_time_1).SetEase(Ease.InOutQuad))
        .Append(transform.DOMoveX(0f, m_AppearanceTime*appearance_time_2).SetEase(Ease.InOutQuad))
        .Join(transform.DOMoveY(-4.5f, m_AppearanceTime*appearance_time_2).SetEase(Ease.OutQuad))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[1], m_AppearanceTime*appearance_time_2).SetEase(Ease.InQuad));
    }

    private void OnPhase1() {
        if (transform.position.x < 0f) {
            m_Sequence = DOTween.Sequence()
            .Append(transform.DOMove(new Vector3(Random.Range(-2f, -1f), Random.Range(-4.5f, -5.5f)), 1.5f).SetEase(Ease.InOutQuad))
            .OnComplete(()=> Move(true));
        }
        else {
            m_Sequence = DOTween.Sequence()
            .Append(transform.DOMove(new Vector3(Random.Range(1f, 2f), Random.Range(-4.5f, -5.5f)), 1.5f).SetEase(Ease.InOutQuad))
            .OnComplete(()=> Move(false));
        }
    }

    private void Move(bool right) {
        float delay = 0.7f;
        float time = 1.5f;
        if (right) {
            m_Sequence = DOTween.Sequence()
            .AppendInterval(delay)
            .Append(transform.DORotateQuaternion(m_QuaternionTurnRight, time*0.5f).SetEase(Ease.OutQuad))
            .Append(transform.DORotateQuaternion(Quaternion.identity, time*0.5f).SetEase(Ease.InQuad))
            .Insert(delay, transform.DOMove(new Vector3(Random.Range(1f, 2f), Random.Range(-4.5f, -5.5f)), time).SetEase(Ease.InOutQuad))
            .OnComplete(()=> Move(false));
        }
        else {
            m_Sequence = DOTween.Sequence()
            .AppendInterval(delay)
            .Append(transform.DORotateQuaternion(m_QuaternionTurnLeft, time*0.5f).SetEase(Ease.OutQuad))
            .Append(transform.DORotateQuaternion(Quaternion.identity, time*0.5f).SetEase(Ease.InQuad))
            .Insert(delay, transform.DOMove(new Vector3(Random.Range(-2f, -1f), Random.Range(-4.5f, -5.5f)), time).SetEase(Ease.InOutQuad))
            .OnComplete(()=> Move(true));
        }
    }


    

    private IEnumerator PatternPhase0() { // 페이즈0 패턴 ============================
        yield return new WaitForSeconds(1f);
        while(m_Phase == 0) {
            m_CurrentPattern = Pattern1();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
                
            m_CurrentPattern = Pattern2();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;

            m_Part.OpenPart();
            m_CurrentPattern = Pattern3();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
            m_Part.ClosePart();
            yield return new WaitForSeconds(2f);
        }
        yield return null;
    }

    private IEnumerator PatternPhase1() { // 페이즈1 패턴 ============================
        yield return new WaitForSeconds(1f);
        while(m_Phase == 1) {
            m_CurrentPattern = Pattern4();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
        }
        yield return null;
    }


    private IEnumerator Pattern1() {
        int random_value = Random.Range(0, 2);
        m_InPattern = true;
        m_Turret2[0].StartPattern(1);
        m_Turret2[1].StartPattern(1);
        yield return new WaitForSeconds(0.3f);

        m_Turret1[random_value].StartPattern(1);
        yield return new WaitForSeconds(1.7f);
        m_Turret1[1 - random_value].StartPattern(1);
        yield return new WaitForSeconds(1.7f);
        m_Turret1[random_value].StartPattern(1);
        yield return new WaitForSeconds(1.7f);
        m_Turret1[1 - random_value].StartPattern(1);
        
        m_Turret2[0].StopPattern();
        m_Turret2[1].StopPattern();
        yield return new WaitForSeconds(1.7f);
        m_Turret0.StartPattern(1);
        if (m_SystemManager.m_Difficulty <= 1)
            yield return new WaitForSeconds(3f);

        m_InPattern = false;
        yield return null;
    }

    private IEnumerator Pattern2() { // Blue Bomb
        Vector3 pos;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, 0.8f);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0f);
        int random_value = Random.Range(0, 2);
        float random_dir;
        m_InPattern = true;
        
        pos = m_FirePosition[random_value].position;
        for (int i = 0; i < 2; i++) {
            random_dir = Random.Range(0f, 360f);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 0.8f,
                3, 5.4f, BulletDirection.FIXED, random_dir, accel2, 15, 24f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 0.8f,
                3, 5.4f, BulletDirection.FIXED, random_dir, accel2, 20, 18f);
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 0.8f,
                5, 4.2f, BulletDirection.FIXED, random_dir + 9f, accel2, 20, 18f);
            }
            else {
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 0.8f,
                3, 5.4f, BulletDirection.FIXED, random_dir, accel2, 24, 15f);
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 0.8f,
                5, 4.6f, BulletDirection.FIXED, random_dir + 7.5f, accel2, 24, 15f);
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 0.8f,
                5, 3.8f, BulletDirection.FIXED, random_dir, accel2, 24, 15f);
            }
            pos = m_FirePosition[1 - random_value].position;
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(2f);

        m_InPattern = false;
        yield return null;
    }

    private IEnumerator Pattern3() { // 청침탄 흩뿌리기
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float target_angle1, target_angle2;
        float[] fire_delay = { 0.15f, 0.1f, 0.083f };
        float[] fire_number = { 20, 30, 36 };
        m_InPattern = true;
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < fire_number[m_SystemManager.m_Difficulty]; i++) {
            pos1 = m_FirePosition[2].position;
            pos2 = m_FirePosition[3].position;
            target_angle1 = GetAngleToTarget(pos1, m_PlayerManager.m_Player.transform.position);
            target_angle2 = GetAngleToTarget(pos2, m_PlayerManager.m_Player.transform.position);
            
            CreateBullet(4, pos1, 4.8f, target_angle1 + Random.Range(-52f, 52f), accel);
            CreateBullet(4, pos2, 4.8f, target_angle2 + Random.Range(-52f, 52f), accel);
                
            yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
        }
        yield return new WaitForSeconds(0.5f);

        m_InPattern = false;
        yield return null;
    }

    private IEnumerator Pattern4() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        int random_value;
        m_InPattern = true;
        float difficulty_timer = 0f;
        if (m_SystemManager.m_Difficulty == 0)
            difficulty_timer = 0.4f;
        
        m_Turret0.StartPattern(2);
        yield return new WaitForSeconds(0.9f + difficulty_timer);

        random_value = Random.Range(0, 2);
        if (random_value == 0) {
            m_Turret1[0].StartPattern(2, true);
            m_Turret1[1].StartPattern(2, false);
        }
        else {
            m_Turret1[0].StartPattern(2, false);
            m_Turret1[1].StartPattern(2, true);
        }
        yield return new WaitForSeconds(2f);

        m_Turret0.StartPattern(2);
        yield return new WaitForSeconds(0.6f);
        m_Turret1[0].StartPattern(3);
        m_Turret1[1].StartPattern(3);

        random_value = Random.Range(0, 2);
        m_Turret2[random_value].StartPattern(2);
        yield return new WaitForSeconds(0.5f + difficulty_timer);
        m_Turret2[1 - random_value].StartPattern(2);
        yield return new WaitForSeconds(1.8f);

        m_InPattern = false;
        yield return null;
    }



    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        StopCoroutine(m_CurrentPattern);
        StopCoroutine(m_CurrentPhase);
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(0.6f, 0f);
        m_Turret0.OnDeath();

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
