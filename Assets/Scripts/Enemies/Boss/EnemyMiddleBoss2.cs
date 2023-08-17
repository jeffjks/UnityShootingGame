using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2 : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public EnemyMiddleBoss2_MainTurret m_MainTurret;
    public EnemyMiddleBoss2_SubTurret[] m_SubTurret = new EnemyMiddleBoss2_SubTurret[2];
    private int m_Phase;

    private IEnumerator m_MovementPattern;
    private IEnumerator _currentPhase;
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        m_Phase = 1;
        m_MoveVector = new MoveVector(-3f, 120f + 180f);
        m_CustomDirection = new CustomDirection();

        m_MovementPattern = AppearanceSequence();
        StartCoroutine(m_MovementPattern);

        _currentPhase = Phase1();
        StartCoroutine(_currentPhase);
        
        DisableInteractable(-1);

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnBossKilled;
        m_MainTurret.m_EnemyDeath.Action_OnKilled += ToNextPhase;
        SetRotatePattern(new RotatePattern_MoveDirection());

        SystemManager.OnMiddleBossStart();
    }

    private IEnumerator AppearanceSequence() {
        //MoveVector init_moveVector;
        yield return new WaitForMillisecondFrames(4000);
        yield return MovementPattern(new MoveVector(0f, m_MoveVector.direction), EaseType.OutQuad, EaseType.InOutQuad, 1000); // stop
        yield return new WaitForMillisecondFrames(2000);
        yield return MovementPattern(new MoveVector(-2f, m_MoveVector.direction), EaseType.InQuad, EaseType.InOutQuad, 1000); // speed to 2f
        yield return new WaitForMillisecondFrames(1000);
        yield return MovementPattern(new MoveVector(m_MoveVector.speed, 220f + 180f), EaseType.Linear, EaseType.InOutQuad, 3500); // direction to 220f
        yield return MovementPattern(new MoveVector(0f, m_MoveVector.direction), EaseType.OutQuad, EaseType.InOutQuad, 1000); // stop
        yield return new WaitForMillisecondFrames(500);
        yield return MovementPattern(new MoveVector(-2f, m_MoveVector.direction), EaseType.InQuad, EaseType.InOutQuad, 1000); // speed to 2f
        yield return MovementPattern(new MoveVector(m_MoveVector.speed, 170f + 180f), EaseType.Linear, EaseType.InOutQuad, 2500); // direction to 170f
        yield return MovementPattern(new MoveVector(0f, m_MoveVector.direction), EaseType.OutQuad, EaseType.InOutQuad, 1000); // stop
        yield return new WaitForMillisecondFrames(1000);
        yield return MovementPattern(new MoveVector(-2f, m_MoveVector.direction), EaseType.InQuad, EaseType.InOutQuad, 1500); // speed to 2f
        yield return MovementPattern(new MoveVector(m_MoveVector.speed, 240f + 180f), EaseType.Linear, EaseType.InOutQuad, 2500); // direction to 240f
        yield return new WaitForMillisecondFrames(6500);
        m_EnemyDeath.RemoveEnemy();
    }

    private IEnumerator MovementPattern(MoveVector target_moveVector, EaseType speedEase, EaseType directionEase, int duration) {
        MoveVector init_moveVector = m_MoveVector;
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int) speedEase].Evaluate((float) (i+1) / frame);
            float t_dir = AC_Ease.ac_ease[(int) directionEase].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_moveVector.speed, target_moveVector.speed, t_spd);
            m_MoveVector.direction = Mathf.Lerp(init_moveVector.direction, target_moveVector.direction, t_dir);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }


    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.375f) { // 체력 37.5% 이하
                ToNextPhase();
            }
        }
        
        m_CustomDirection[0] += 200f / Application.targetFrameRate * Time.timeScale;
    }

    private IEnumerator Phase1()
    {
        StartPattern("1A", new BulletPattern_EnemyMiddleBoss2_1A(this));
        yield break;
    }

    public void ToNextPhase() {
        if (m_Phase == 2)
            return;
        m_Phase = 2;
        if (_currentPhase != null)
            StopCoroutine(_currentPhase);
        
        StopAllPatterns();

        if (m_MainTurret != null)
            m_MainTurret.m_EnemyDeath.KillEnemy();
        if (m_SubTurret[0] != null)
            m_SubTurret[0].m_EnemyDeath.KillEnemy();
        if (m_SubTurret[1] != null)
            m_SubTurret[1].m_EnemyDeath.KillEnemy();
        
        EnableInteractable();

        StartPattern("2A", new BulletPattern_EnemyMiddleBoss2_2A(this));
        StartPattern("2B", new BulletPattern_EnemyMiddleBoss2_2B(this));
        
        //m_Collider2D[0].gameObject.SetActive(true);
        BulletManager.SetBulletFreeState(500);
    }


    protected override IEnumerator DyingEffect() { // 파괴 과정
        BulletManager.BulletsToGems(2000);
        m_MoveVector.speed = 0f;
        m_Phase = -1;

        if (m_MovementPattern != null) {
            StopCoroutine(m_MovementPattern);
        }
        
        yield break;
    }

    public void OnBossKilled() {
        SystemManager.OnMiddleBossClear();
    }

    public void OnEndBossDeathAnimation() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}
