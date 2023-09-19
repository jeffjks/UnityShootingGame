using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 파괴 후 Quaternion.identity 상태로 돌아가는 용도

public class EnemyBoss1 : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public EnemyUnit m_Turret1;
    public EnemyUnit[] m_Turret2 = new EnemyUnit[2];
    public EnemyUnit[] m_Turret3 = new EnemyUnit[2];
    public EnemyBoss1_Part m_Part;
    public Transform m_Rotator;
    public AnimationCurve m_AnimationCurve_Turn;

    private int m_Phase;

    private readonly Vector3[] m_TargetPosition = new Vector3[2];
    private const int APPEARANCE_TIME = 2000;
    private const float ROLLING_ANGLE_START = -35f;
    private const float ROLLING_ANGLE_MID = 20f;
    private const float ROLLING_ANGLE_MAX = 15f;

    private IEnumerator m_CurrentPhase;
    private IEnumerator m_CurrentMovement;

    private void Start()
    {
        m_TargetPosition[0] = new Vector3(4f, -1f, Depth.ENEMY);
        m_TargetPosition[1] = new Vector3(0f, -5f, Depth.ENEMY);

        m_Rotator.rotation = Quaternion.Euler(0f, ROLLING_ANGLE_START, 0f);

        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnEndBossDeathAnimation;
        m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
        m_Part.m_EnemyDeath.Action_OnKilled += ToNextPhase;
    }

    protected override void Update()
    {
        base.Update();

        OnPhase1();
    }

    private void DestroyChildEnemy() {
        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.30f) { // 체력 30% 이하
                if (m_Part != null)
                    m_Part.m_EnemyDeath.KillEnemy();
            }
        }
    }

    public void ToNextPhase()
    {
        if (m_Phase != 1)
            return;
        m_Phase++;
        m_MoveVector = new MoveVector(0f, 0f);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        m_Turret1.StopAllPatterns();
        m_Turret2[0].StopAllPatterns();
        m_Turret2[1].StopAllPatterns();
        m_Turret3[0].StopAllPatterns();
        m_Turret3[1].StopAllPatterns();
        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);
        m_Turret2[0].SetRotatePattern(new RotatePattern_TargetAngle(0f, 100f));
        m_Turret2[1].SetRotatePattern(new RotatePattern_TargetAngle(0f, 100f));
        
        m_CurrentMovement = OnPhase2();
        StartCoroutine(m_CurrentMovement);
    }


    private IEnumerator AppearanceSequence()
    {
        int frame1 = 1100 * Application.targetFrameRate / 1000;
        int frame2 = 900 * Application.targetFrameRate / 1000;

        Vector3 init_vector = transform.position;
        Quaternion init_quaternion = transform.rotation;
        Quaternion tilt_quaternion = Quaternion.Euler(0f, ROLLING_ANGLE_MID, 0f);

        for (int i = 0; i < frame1; ++i) {
            float t_posx = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame1);
            float t_posy = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame1);
            float t_rot = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame1);

            float position_x = Mathf.Lerp(init_vector.x, 3f, t_posx);
            float position_y = Mathf.Lerp(init_vector.y, -2f, t_posy);
            transform.position = new Vector3(position_x, position_y, transform.position.z);
            m_Rotator.rotation = Quaternion.Lerp(init_quaternion, tilt_quaternion, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }

        init_vector = transform.position;
        init_quaternion = m_Rotator.rotation;

        for (int i = 0; i < frame2; ++i) {
            float t_posx = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame2);
            float t_posy = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame2);
            float t_rot = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame2);

            float position_x = Mathf.Lerp(init_vector.x, 0f, t_posx);
            float position_y = Mathf.Lerp(init_vector.y, -4.5f, t_posy);
            transform.position = new Vector3(position_x, position_y, transform.position.z);
            m_Rotator.rotation = Quaternion.Lerp(init_quaternion, Quaternion.identity, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }
        
        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        int rand = Random.Range(0, 2);
        m_MoveVector = new MoveVector(1f, 90f + 180f*rand);

        EnableInteractableAll();

        // SystemManager.OnBossStart();
    }

    private void OnPhase1() {
        if (m_Phase != 1) {
            return;
        }
        if (transform.position.x < -1f) {
            m_MoveVector.direction -= 180f;
        }
        else if (transform.position.x > 1f) {
            m_MoveVector.direction += 180f;
        }
    }

    private IEnumerator OnPhase2() {
        int frame = 1500 * Application.targetFrameRate / 1000;
        Quaternion quaternion_rightTurn = Quaternion.Euler(0f, -ROLLING_ANGLE_MAX, 0f);
        Quaternion quaternion_leftTurn = Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f);

        yield return new WaitForMillisecondFrames(700);

        while (true) {
            Vector3 init_vector;
            Vector3 target_vector;
            Quaternion init_quaternion;
            
            if (transform.position.x < 0f) {
                init_vector = transform.position;
                target_vector = new Vector3(Random.Range(1f, 2f), Random.Range(-4.5f, -5.5f), Depth.ENEMY);
                init_quaternion = m_Rotator.rotation;

                for (int i = 0; i < frame; ++i) {
                    float t_pos = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
                    float t_rot = m_AnimationCurve_Turn.Evaluate((float) (i+1) / frame);
                    
                    transform.position = Vector3.Lerp(init_vector, target_vector, t_pos);
                    m_Rotator.rotation = Quaternion.Lerp(init_quaternion, quaternion_rightTurn, t_rot);
                    //m_Rotator.rotation = Quaternion.Lerp(init_quaternion, m_TargetQuaternion[0], t_rot);
                    yield return new WaitForMillisecondFrames(0);
                }
            }
            else {
                init_vector = transform.position;
                target_vector = new Vector3(Random.Range(-2f, -1f), Random.Range(-4.5f, -5.5f), Depth.ENEMY);
                init_quaternion = m_Rotator.rotation;

                for (int i = 0; i < frame; ++i) {
                    float t_pos = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
                    float t_rot = m_AnimationCurve_Turn.Evaluate((float) (i+1) / frame);
                    
                    transform.position = Vector3.Lerp(init_vector, target_vector, t_pos);
                    m_Rotator.rotation = Quaternion.Lerp(init_quaternion, quaternion_leftTurn, t_rot);
                    //m_Rotator.rotation = Quaternion.Lerp(init_quaternion, m_TargetQuaternion[0], t_rot);
                    yield return new WaitForMillisecondFrames(0);
                }
            }
            yield return new WaitForMillisecondFrames(700);
        }
    }

    

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            yield return Phase1_PatternA();
            
            yield return StartPattern("1B", new BulletPattern_EnemyBoss1_1B(this)); // Blue Bomb
            
            m_Part.SetOpenState(true);
            yield return StartPattern("1C", new BulletPattern_EnemyBoss1_1C(this)); // 청침탄 흩뿌리기
            
            m_Part.SetOpenState(false);
            yield return new WaitForMillisecondFrames(2000);
        }
    }

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 2) {
            yield return Phase2_PatternA();
        }
    }


    private IEnumerator Phase1_PatternA() {
        int random_value = Random.Range(0, 2);
        
        m_Turret3[0].StartPattern("1A", new BulletPattern_EnemyBoss1_Turret3_1A(m_Turret3[0]));
        m_Turret3[1].StartPattern("1A", new BulletPattern_EnemyBoss1_Turret3_1A(m_Turret3[1]));
        yield return new WaitForMillisecondFrames(300);

        m_Turret2[random_value].StartPattern("1A", new BulletPattern_EnemyBoss1_Turret2_1A(m_Turret2[random_value]));
        yield return new WaitForMillisecondFrames(1700);
        m_Turret2[1 - random_value].StartPattern("1A", new BulletPattern_EnemyBoss1_Turret2_1A(m_Turret2[1 - random_value]));
        yield return new WaitForMillisecondFrames(1700);
        m_Turret2[random_value].StartPattern("1A", new BulletPattern_EnemyBoss1_Turret2_1A(m_Turret2[random_value]));
        yield return new WaitForMillisecondFrames(1700);
        m_Turret2[1 - random_value].StartPattern("1A", new BulletPattern_EnemyBoss1_Turret2_1A(m_Turret2[1 - random_value]));
        
        m_Turret3[0].StopPattern("1A");
        m_Turret3[1].StopPattern("1A");
        yield return new WaitForMillisecondFrames(1700);
        
        m_Turret1.StartPattern("1A", new BulletPattern_EnemyBoss1_Turret1_1A(m_Turret1));
        if (SystemManager.Difficulty <= GameDifficulty.Expert)
            yield return new WaitForMillisecondFrames(3000);
    }

    private IEnumerator Phase2_PatternA() {
        int random_value;
        
        int difficulty_timer = 0;
        if (SystemManager.Difficulty == GameDifficulty.Normal)
            difficulty_timer = 400;
        
        m_Turret1.StartPattern("2A", new BulletPattern_EnemyBoss1_Turret1_2A(m_Turret1));
        yield return new WaitForMillisecondFrames(900 + difficulty_timer);

        random_value = Random.Range(0, 2);
        m_Turret2[0].StartPattern("2A", new BulletPattern_EnemyBoss1_Turret2_2A(m_Turret2[0], 1 - random_value));
        m_Turret2[1].StartPattern("2A", new BulletPattern_EnemyBoss1_Turret2_2A(m_Turret2[1], random_value));
        yield return new WaitForMillisecondFrames(2000);

        m_Turret1.StartPattern("2A", new BulletPattern_EnemyBoss1_Turret1_2A(m_Turret1));
        yield return new WaitForMillisecondFrames(600);
        m_Turret2[0].StartPattern("2B", new BulletPattern_EnemyBoss1_Turret2_2B(m_Turret2[0]));
        m_Turret2[1].StartPattern("2B", new BulletPattern_EnemyBoss1_Turret2_2B(m_Turret2[1]));

        random_value = Random.Range(0, 2);
        m_Turret3[random_value].StartPattern("2A", new BulletPattern_EnemyBoss1_Turret3_2A(m_Turret3[random_value]));
        yield return new WaitForMillisecondFrames(500 + difficulty_timer);
        m_Turret3[1 - random_value].StartPattern("2A", new BulletPattern_EnemyBoss1_Turret3_2A(m_Turret3[1 - random_value]));
        yield return new WaitForMillisecondFrames(1800);
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        if (m_CurrentMovement != null)
            StopCoroutine(m_CurrentMovement);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0.6f, 0f);
        m_Turret1.m_EnemyDeath.KillEnemy();

        m_Rotator.DORotateQuaternion(Quaternion.identity, 1f).SetEase(Ease.Linear);
        yield break;
    }

    public void OnBossKilled() {
        SystemManager.OnBossClear();
    }

    public void OnEndBossDeathAnimation() {
        SystemManager.Instance.StartStageClearCoroutine();
        InGameScreenEffectService.WhiteEffect(true);
        MainCamera.ShakeCamera(1f);
    }
}
