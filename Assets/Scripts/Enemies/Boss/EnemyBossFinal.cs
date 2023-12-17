using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossFinal : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public GameObject m_BombBarrier;
    public ParticleSystem m_ParticleFireEffect;
    public ParticleSystem m_ParticleLightningEffect;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;
    public readonly float[] m_CustomDirectionDelta = new float[2];
    private int _directionSide = 1;

    private int m_Phase;
    private readonly Vector3 TARGET_POSITION = new (0f, -3.8f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 1600;

    private IEnumerator m_CurrentPhase;

    private void Start()
    {
        // IsColliderInit = false;
        m_CustomDirection = new CustomDirection(2);
        
        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnEndBossDeathAnimation;
    }

    private IEnumerator AppearanceSequence() {
        Vector3 init_position = transform.position;
        Vector3 init_scale = transform.localScale;
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
            float t_scale = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_position, TARGET_POSITION, t_pos);
            transform.localScale = Vector3.Lerp(init_scale, new Vector3(1f, 1f, 1f), t_pos);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        float[] random_direction = { 70f, 110f, -70f, -110f };
        m_MoveVector = new MoveVector(1f, random_direction[Random.Range(0, 4)]);

        // IsColliderInit = true;
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        StageManager.IsTrueBossEnabled = false;
        
        PlayerInvincibility.Action_OnInvincibilityChanged += SetBombBarrier;
        SetBombBarrier(PlayerInvincibility.IsInvincible);

        EnableInteractableAll();
        
        // SystemManager.OnBossStart();
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase > 0) {
            if (transform.position.x > TARGET_POSITION.x + 1.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
                transform.position = new Vector3(TARGET_POSITION.x + 1.5f, transform.position.y, transform.position.z);
            }
            if (transform.position.x < TARGET_POSITION.x - 1.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
                transform.position = new Vector3(TARGET_POSITION.x - 1.5f, transform.position.y, transform.position.z);
            }
            if (transform.position.y > TARGET_POSITION.y + 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                transform.position = new Vector3(transform.position.x, TARGET_POSITION.y + 0.4f, transform.position.z);
            }
            if (transform.position.y < TARGET_POSITION.y - 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                transform.position = new Vector3(transform.position.x, TARGET_POSITION.y - 0.4f, transform.position.z);
            }
        }

        for (var i = 0; i < m_CustomDirection.Length; ++i)
        {
            m_CustomDirection[i] += m_CustomDirectionDelta[i] * _directionSide / Application.targetFrameRate * Time.timeScale;
        }
    }

    private void SetBombBarrier(bool state) {
        if (DebugOption.InvincibleMod)
            return;
        if (state)
        {
            m_EnemyHealth.SetInvincibility();
        }
        else
        {
            m_EnemyHealth.DisableInvincibility();
        }
        m_BombBarrier.SetActive(state);
    }

    public void ToNextPhase() {
        m_Phase++;
        StopAllPatterns();
        BulletManager.SetBulletFreeState(2000);
        m_ParticleFireEffect.gameObject.SetActive(false);
        m_ParticleLightningEffect.gameObject.SetActive(true);
        NextPhaseExplosion();

        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        var side = 1;
        yield return new WaitForMillisecondFrames(3000);
        StartPattern("1A1", new EnemyBossFinal_BulletPattern_1A1(this));
        yield return StartPattern("1A2", new EnemyBossFinal_BulletPattern_1A2(this));
        StopAllPatterns();
        yield return new WaitForMillisecondFrames(3000);

        while (m_Phase == 1)
        {
            StartPattern("1B1", new EnemyBossFinal_BulletPattern_1B1(this));
            yield return new WaitForMillisecondFrames(3000);
            StartPattern("1B2", new EnemyBossFinal_BulletPattern_1B2(this));
            yield return new WaitForMillisecondFrames(4000);
            StartPattern("1B2", new EnemyBossFinal_BulletPattern_1B2(this));
            yield return new WaitForMillisecondFrames(1500);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
            
            StartPattern("1C1", new EnemyBossFinal_BulletPattern_1C1(this));
            yield return new WaitForMillisecondFrames(2000);
            StartPattern("1C2", new EnemyBossFinal_BulletPattern_1C2(this));
            yield return new WaitForMillisecondFrames(8000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            StartPattern("1D1", new EnemyBossFinal_BulletPattern_1D1(this, side));
            StartPattern("1D2", new EnemyBossFinal_BulletPattern_1D2(this));
            yield return new WaitForMillisecondFrames(14000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            StartPattern("1E1", new EnemyBossFinal_BulletPattern_1E1(this, side));
            StartPattern("1E2", new EnemyBossFinal_BulletPattern_1E2(this));
            yield return new WaitForMillisecondFrames(8000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            _directionSide *= -1;
            side *= -1;
        }
    }
    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(2000);
        yield return StartPattern("2A", new EnemyBossFinal_BulletPattern_2A(this));
        StopAllPatterns();
        
        yield return new WaitForMillisecondFrames(5000);
        StartPattern("FinalA", new EnemyBossFinal_BulletPattern_FinalA(this));
        yield return new WaitForMillisecondFrames(4000);
        StartPattern("FinalB", new EnemyBossFinal_BulletPattern_FinalB(this));
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        StopAllPatterns();
        PlayerInvincibility.Action_OnInvincibilityChanged -= SetBombBarrier;
        m_BombBarrier.SetActive(false);
        m_ParticleFireEffect.gameObject.SetActive(false);
        m_ParticleLightningEffect.gameObject.SetActive(false);
        
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0.7f, 0f);
        
        InGameDataManager.Instance.SaveElapsedTime();
        
        yield break;
    }

    public void OnBossKilled() {
        SystemManager.OnBossClear();
    }

    public void OnEndBossDeathAnimation() {
        SystemManager.Instance.StartStageClearCoroutine();
        InGameScreenEffectService.WhiteEffect(true);
        MainCamera.ShakeCamera(2f);
    }
}
