using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss3 : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;
    
    private Vector2 m_TargetPosition;
    private int m_Phase;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2;
    private const int APPEARANCE_TIME = 3500;

    protected override void Start()
    {
        base.Start();

        IsColliderInit = false;
        m_TargetPosition = new Vector2(0f, -4.3f);
        m_CustomDirection = new CustomDirection(2);

        DisableInteractableAll();

        int delay = 2000;
        StartCoroutine(AppearanceSequence(delay));

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnBossKilled;
    }

    private IEnumerator AppearanceSequence(int delay) {
        yield return new WaitForMillisecondFrames(delay);

        float init_position_y = transform.position.y;
        int frame = (APPEARANCE_TIME - delay) * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos_y = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float position_y = Mathf.Lerp(init_position_y, 2.4f, t_pos_y);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        float[] random_direction = { 90f, -90f };
        m_MoveVector = new MoveVector(0.6f, random_direction[Random.Range(0, 2)]);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        IsColliderInit = true;
        StartCoroutine(m_CurrentPhase);

        EnableInteractableAll();

        SystemManager.OnMiddleBossStart();
    }

    protected override void Update()
    {
        base.Update();
        
        Vector3 pos = transform.position;
        var backgroundSpeed = BackgroundCamera.GetBackgroundCameraMoveVector().z;
        transform.position = new Vector3(pos.x, pos.y, pos.z + backgroundSpeed / Application.targetFrameRate * Time.timeScale); // 배경 카메라 속도에 맞춰서 이동

        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }
        
        if (!TimeLimitState && m_Phase > 0) {
            if (transform.position.x > m_TargetPosition.x + 1.6f) {
                m_MoveVector.direction = -90f;
            }
            if (transform.position.x < m_TargetPosition.x - 1.6f) {
                m_MoveVector.direction = 90f;
            }
        }

        m_CustomDirection[0] += 111f / Application.targetFrameRate * Time.timeScale;
        m_CustomDirection[1] += 79f / Application.targetFrameRate * Time.timeScale;
    }

    public void ToNextPhase() {
        if (m_Phase == 2)
            return;
        m_Phase = 2;

        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        StopAllPatterns();

        StartPattern("2A1", new BulletPattern_EnemyMiddleBoss3_2A1(this));
        StartPattern("2A2", new BulletPattern_EnemyMiddleBoss3_2A2(this));

        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            StartPattern("1A1", new BulletPattern_EnemyMiddleBoss3_1A1(this));
            StartPattern("1A2", new BulletPattern_EnemyMiddleBoss3_1A2(this));
            yield return new WaitForMillisecondFrames(4000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(2000);
            
            StartPattern("1B1", new BulletPattern_EnemyMiddleBoss3_1B1(this));
            StartPattern("1B2", new BulletPattern_EnemyMiddleBoss3_1B2(this));
            yield return new WaitForMillisecondFrames(3000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(2000);
        }
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        BulletManager.BulletsToGems(2000);
        m_MoveVector.speed = 0f;
        m_Phase = -1;
        
        yield break;
    }

    public void OnBossKilled() {
        SystemManager.OnMiddleBossClear();
    }

    public void OnEndBossDeathAnimation() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}
