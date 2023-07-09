using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge1 : EnemyUnit
{
    public GameObject[] m_Part = new GameObject[2];
    public EnemyPlaneLarge1Turret[] m_Turret = new EnemyPlaneLarge1Turret[2];
    public Transform m_Rotator;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int[] m_FireDelay1 = { 1600, 800, 600 };
    private int[] m_FireDelay2 = { 1500, 1000, 800 };
    
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;
    private IEnumerator m_TimeLimit;
    private const int APPEARANCE_TIME = 2800;
    private const int TIME_LIMIT = 20000;
    private const float ROLLING_ANGLE_MAX = 30f;
    private readonly Vector3 TARGET_POSITION = new (0f, -5.2f, Depth.ENEMY);
    private int m_Phase;

    void Start ()
    {
        m_Rotator.rotation = Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f);

        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());
        
        /*
        m_Sequence.Append(transform.DOMove(TARGET_POSITION, APPEARANCE_TIME).SetEase(Ease.OutQuad));
        m_Sequence.Join(transform.DORotateQuaternion(m_TargetQuaternion, APPEARANCE_TIME).SetEase(Ease.InQuad));
        */
    }

    public IEnumerator AppearanceSequence() {
        Vector3 init_position = transform.position;
        Quaternion init_rotation = m_Rotator.rotation;
        Quaternion target_rotation = Quaternion.identity;

        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            float t_rot = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);

            transform.position = Vector3.Lerp(init_position, TARGET_POSITION, t_pos);
            m_Rotator.rotation = Quaternion.Lerp(init_rotation, target_rotation, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
        yield break;
    }

    public void OnAppearanceComplete() {
        m_Phase = 1;

        m_CurrentPattern1 = PatternA();
        StartCoroutine(m_CurrentPattern1);

        EnableInteractableAll();

        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;
        m_MoveVector.direction = -90f;

        float init_speed = m_MoveVector.speed;
        Quaternion init_rotation = m_Rotator.rotation;
        float target_speed = 7f;
        Quaternion target_rotation = Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f);
        int frame = 2000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            float t_rot = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, target_speed, t_spd);
            m_Rotator.rotation = Quaternion.Lerp(init_rotation, target_rotation, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.m_HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }
    }

    private void ToNextPhase() {
        m_Phase++;
        m_Turret[0].m_EnemyDeath.OnDying();
        m_Turret[1].m_EnemyDeath.OnDying();
        Destroy(m_Part[0]);
        Destroy(m_Part[1]);

        StopCoroutine(m_CurrentPattern1);
        m_CurrentPattern2 = PatternB();
        StartCoroutine(m_CurrentPattern2);

        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }
    
    private IEnumerator PatternA() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;
        float target_angle, random_value;
        int state = Random.Range(-1, 1);
        if (state == 0) {
            state = 1;
        }
        yield return new WaitForMillisecondFrames(200);

        while(!m_TimeLimitState) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                random_value = Random.Range(-8f, 0f);
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                for (int i = 0; i < 4; i++) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 5.6f + i*0.32f, target_angle + (random_value + i*2f)*state, accel, 5, 20f);
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(280);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                random_value = Random.Range(-8f, 0f);
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                for (int i = 0; i < 6; i++) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 6f + i*0.4f, target_angle + (random_value + i*2f)*state, accel, 5, 20f);
                    yield return new WaitForMillisecondFrames(70);
                }
                yield return new WaitForMillisecondFrames(140);
            }
            else {
                random_value = Random.Range(-8f, 0f);
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                for (int i = 0; i < 8; i++) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(3, pos, 5.6f + i*0.4f, target_angle + (random_value + i*2f)*state, accel, 5, 20f);
                    yield return new WaitForMillisecondFrames(70);
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay1[(int) SystemManager.Difficulty]);
            state *= -1;


            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 6; i++) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(0, pos, 6.2f, random_value + i*12f*state, accel, 8, 45f);
                    yield return new WaitForMillisecondFrames(150);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 6; i++) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(0, pos, 6.2f, random_value + i*9f*state, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state + 28f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state + 14f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state - 14f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state - 28f, accel, 4, 90f);
                    yield return new WaitForMillisecondFrames(150);
                }
            }
            else {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 6; i++) {
                    pos = m_FirePosition[0].position;
                    CreateBulletsSector(0, pos, 6.6f, random_value + i*9f*state, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state + 28f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state + 14f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state - 14f, accel, 4, 90f);
                    CreateBulletsSector(1, pos, 6f, random_value + i*9f*state - 28f, accel, 4, 90f);
                    yield return new WaitForMillisecondFrames(150);
                }
            }
            yield return new WaitForMillisecondFrames(200);

            m_Turret[0].StartPattern();
            m_Turret[1].StartPattern();
            yield return new WaitForMillisecondFrames(m_FireDelay1[(int) SystemManager.Difficulty]);
        }
        yield break;
    }
    
    private IEnumerator PatternB() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3 pos;
        float target_angle, random_value;
        
        yield return new WaitForMillisecondFrames(800);

        while(!m_TimeLimitState) {
            random_value = Random.Range(-3f, 3f);

            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                CreateBulletsSector(5, pos, 6.1f, target_angle + random_value, accel, 6, 16f);
                CreateBulletsSector(5, pos, 6.4f, target_angle + random_value, accel, 6, 16f);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                CreateBulletsSector(5, pos, 5.6f, target_angle + random_value, accel, 6, 12f);
                CreateBulletsSector(5, pos, 6.1f, target_angle + random_value, accel, 12, 6f);
                CreateBulletsSector(5, pos, 6.6f, target_angle + random_value, accel, 6, 12f);
                CreateBulletsSector(5, pos, 7.1f, target_angle + random_value, accel, 6, 12f);
            }
            else {
                pos = m_FirePosition[0].position;
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                CreateBulletsSector(5, pos, 5.6f, target_angle + random_value, accel, 8, 12f);
                CreateBulletsSector(5, pos, 6.1f, target_angle + random_value, accel, 14, 6f);
                CreateBulletsSector(5, pos, 6.6f, target_angle + random_value, accel, 8, 12f);
                CreateBulletsSector(5, pos, 7.1f, target_angle + random_value, accel, 8, 12f);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay2[(int) SystemManager.Difficulty]);
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_MoveVector = new MoveVector(1.2f, 0f);
        m_Phase = -1;
        
        yield break;
    }
}
