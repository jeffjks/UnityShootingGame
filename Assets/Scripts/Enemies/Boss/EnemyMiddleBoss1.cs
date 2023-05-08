using System.Collections;
using UnityEngine;

public class EnemyMiddleBoss1 : EnemyUnit, IEnemyBossMain
{
    public EnemyMiddleBoss1Turret[] m_Turret = new EnemyMiddleBoss1Turret[2];
    public Transform m_Rotator;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int m_Phase;
    private Vector3 m_TargetPosition;
    private const int APPEARANCE_TIME = 2000;
    private const int TIME_LIMIT = 17000;
    private const float ROLLING_ANGLE_MAX = 30f;

    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        m_TargetPosition = new Vector3(0f, -5f, Depth.ENEMY);
        m_Rotator.rotation = Quaternion.Euler(0f, 36f, 20f);

        DisableInteractableAll();
        
        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDying;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.m_HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        MovePattern();
    }

    private void MovePattern() {
        if (m_TimeLimitState) {
            return;
        }
        if (m_Phase != 1) {
            return;
        }
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

    public IEnumerator AppearanceSequence() {
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        Vector3 init_vector = transform.position;
        Quaternion init_quarternion = m_Rotator.rotation;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            float t_rot = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_vector, m_TargetPosition, t_pos);
            m_Rotator.rotation = Quaternion.Lerp(init_quarternion, Quaternion.identity, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }
        OnAppearanceComplete();
        
        //m_Sequence = DOTween.Sequence()
        //.Append(transform.DOMove(m_TargetPosition, APPEARANCE_TIME).SetEase(Ease.OutQuad))
        //.Join(transform.DORotateQuaternion(m_TargetQuaternion, APPEARANCE_TIME).SetEase(Ease.InQuad));
        yield break;
    }

    public void OnAppearanceComplete() {
        float[] random_direction = { 70f, 110f, -70f, -110f };
        m_MoveVector = new MoveVector(0.8f, random_direction[Random.Range(0, 4)]);
        m_Phase = 1;

        m_CurrentPattern1 = PatternA(m_SystemManager.GetDifficulty());
        StartCoroutine(m_CurrentPattern1);
        
        EnableInteractableAll();

        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME + time_limit);
        m_TimeLimitState = true;

        int frame = 3500 * Application.targetFrameRate / 1000;

        Quaternion init_quarternion = m_Rotator.rotation;
        float target_xspeed;

        if (Mathf.DeltaAngle(0f, m_MoveVector.direction) < 180f) {
            target_xspeed = 18f;
        }
        else {
            target_xspeed = -18f;
        }

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);
            float t_rot = (i+1) / frame;
            
            transform.Translate(new Vector3(Mathf.Lerp(0f, target_xspeed / Application.targetFrameRate, t_pos), 0f, 0f));
            m_Rotator.rotation = Quaternion.Lerp(init_quarternion, Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f), t_rot);
            yield return new WaitForMillisecondFrames(0);
        }

        //transform.DOMoveX(-20f, 4f).SetEase(Ease.InQuad);
        //transform.DORotateQuaternion(Quaternion.Euler(0f, 30f, 0f), 3f).SetEase(Ease.Linear);
        yield break;
    }

    private void ToNextPhase() {
        m_Phase++;
        m_SystemManager.EraseBullets(1000);
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        
        m_CurrentPattern2 = PatternB();
        if (m_SystemManager.GetDifficulty() == 2)
            StartCoroutine(m_CurrentPattern2);

        m_Turret[0].StopPattern();
        m_Turret[1].StopPattern();

        m_Turret[0].StartPattern();
        m_Turret[1].StartPattern();

        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private IEnumerator PatternA(int difficulty) {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(600);
        if (difficulty == 0) {
            while(true) {
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 5, 17f);
                yield return new WaitForMillisecondFrames(1520);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 6, 17f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 6, 17f);
                yield return new WaitForMillisecondFrames(1520);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 7, 17f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 7, 17f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 7, 17f);
                yield return new WaitForMillisecondFrames(3120); // --------------------------
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 24, 15f);
                yield return new WaitForMillisecondFrames(1200);
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 24, 15f);
                yield return new WaitForMillisecondFrames(3600);
            }
        }
        else if (difficulty == 1) {
            while(true) {
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 8, 10f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 8, 10f);
                yield return new WaitForMillisecondFrames(1400);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 9, 10f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 9, 10f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 9, 10f);
                yield return new WaitForMillisecondFrames(1400);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 10f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 10f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 10f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 10f);
                yield return new WaitForMillisecondFrames(2200); // --------------------------
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 40, 9f);
                yield return new WaitForMillisecondFrames(600);
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 40, 9f);
                yield return new WaitForMillisecondFrames(600);
                CreateBulletsSector(0, transform.position, 5f, 0f, accel, 40, 9f);
                yield return new WaitForMillisecondFrames(2400);
            }
        }
        else {
            while(true) {
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 8f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 10, 8f);
                yield return new WaitForMillisecondFrames(1400);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 11, 8f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 11, 8f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 11, 8f);
                yield return new WaitForMillisecondFrames(1400);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 12, 8f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 12, 8f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 12, 8f);
                yield return new WaitForMillisecondFrames(120);
                CreateBulletsSector(5, transform.position, 5f, 0f, accel, 12, 8f);
                yield return new WaitForMillisecondFrames(2000); // --------------------------
                CreateBulletsSector(0, transform.position, 6.6f, 0f, accel, 45, 8f);
                yield return new WaitForMillisecondFrames(600);
                CreateBulletsSector(0, transform.position, 6.6f, 0f, accel, 45, 8f);
                yield return new WaitForMillisecondFrames(600);
                CreateBulletsSector(0, transform.position, 6.6f, 0f, accel, 45, 8f);
                yield return new WaitForMillisecondFrames(2200);
            }
        }
    }

    private IEnumerator PatternB() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(1800);
        while(true) {
            float target_angle = GetAngleToTarget(transform.position, m_PlayerManager.GetPlayerPosition());
            CreateBulletsSector(0, transform.position, 7f, target_angle, accel, 8, 13f);
            yield return new WaitForMillisecondFrames(2000);
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_SystemManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(1f, 0f);
        m_Phase = -1;
        
        yield break;
    }

    public void OnBossDying() {
        m_SystemManager.MiddleBossClear();
    }

    public void OnBossDeath() {
        m_SystemManager.ScreenEffect(0);
    }
}
