using UnityEngine;
using System.Collections;

public class EnemyMiddleBoss5b : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public GameObject m_Hull;
    public EnemyMiddleBoss5b_Turret m_Turret;
    public Transform m_Renderer;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private IEnumerator m_MovementPattern;
    
    private readonly Vector3 TARGET_POSITION = new (0f, -3.8f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 2000;
    private int _phase;

    private void Start()
    {
        _phase = 1;
        m_MovementPattern = AppearanceSequence();
        StartCoroutine(m_MovementPattern);
        
        RotateUnit(AngleToPlayer);

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnBossKilled;

        // SystemManager.OnMiddleBossStart();
    }

    private IEnumerator AppearanceSequence() {
        int duration = 3000;
        int random_sign = Random.Range(-1, 1);
        if (random_sign == 0)
            random_sign = 1;

        yield return MovementPattern(TARGET_POSITION, EaseType.OutQuad, APPEARANCE_TIME);

        StartPattern("A", new BulletPattern_EnemyMiddleBoss5b_A(this));
        
        yield return MovementPattern(new Vector3(-3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, 2000);
        yield return MovementPattern(new Vector3(3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(-3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(-3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(-3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(0f, -3.8f, Depth.ENEMY), EaseType.InOutQuad, 2000);
        yield return MovementPattern(new Vector3(0f, 10f, Depth.ENEMY), EaseType.InQuad, 3000);
    }

    private IEnumerator MovementPattern(Vector3 target_position, EaseType positionEase, int duration) {
        Vector3 init_position = transform.position;
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)positionEase].Evaluate((float) (i+1) / frame);

            transform.position = Vector3.Lerp(init_position, target_position, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (Time.timeScale == 0)
            return;
        
        SetRotatePattern(new RotatePattern_TargetPlayer(40f, 100f));
    }

    public void ToNextPhase()
    {
        // if (SystemManager.GameMode != GameMode.Replay)
        {
            switch (_phase)
            {
                case 1:
                    if (m_EnemyHealth.HealthRatioScaled > 400) // 체력 40% 이하
                        return;
                    break;
                default:
                    return;
            }
        }
        
        m_EnemyHealth.WriteReplayHealthData();
        
        _phase++;
        StopPattern("B1");
        StopPattern("B2");
        m_Hull.SetActive(false);
        
        m_Turret.StartPattern("A", new BulletPattern_EnemyMiddleBoss5b_Turret_A(m_Turret));
        
        NextPhaseExplosion();
        
        // if (SystemManager.GameMode != GameMode.Replay)
            // m_EnemyHealth.Action_OnHealthChanged -= ToNextPhase;
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    public int GetCurrentPhase()
    {
        return _phase;
    }


    private IEnumerator DeathPattern(Quaternion target_rotation, Vector3 target_scale, EaseType rotationEase, EaseType scaleEase, int duration) {
        Quaternion init_rotation = transform.rotation;
        Vector3 init_scale = m_Renderer.transform.localScale;
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_rot = AC_Ease.ac_ease[(int)rotationEase].Evaluate((float) (i+1) / frame);
            float t_scale = AC_Ease.ac_ease[(int)scaleEase].Evaluate((float) (i+1) / frame);

            transform.rotation = Quaternion.Lerp(init_rotation, target_rotation, t_rot);
            m_Renderer.transform.localScale = Vector3.Lerp(init_scale, target_scale, t_scale);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_MoveVector = new MoveVector(1.2f, 0f);
        BulletManager.BulletsToGems(3000);
        _phase = 2;
        StopAllPatterns();
        m_Turret.m_EnemyDeath.KillEnemy();

        if (m_MovementPattern != null) {
            StopCoroutine(m_MovementPattern);
        }

        StartCoroutine(DeathPattern(new Quaternion(-0.2f, 0.9f, -0.4f, -0.2f), new Vector3(0.7f, 0.7f, 0.7f), EaseType.Linear, EaseType.Linear, 2500));
        
        /*
        m_Sequence = DOTween.Sequence()
        .Append(m_Renderer.DORotateQuaternion(new Quaternion(-0.2f, 0.9f, -0.4f, -0.2f), 2.5f).SetEase(Ease.Linear))
        .Join(m_Renderer.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 2.5f).SetEase(Ease.Linear));*/
        
        yield break;
    }

    public void OnBossKilled() {
        SystemManager.OnMiddleBossClear();
    }

    public void OnEndBossDeathAnimation() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}
