using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2 : EnemyUnit, IHasAppearance, IEnemyBossMain
{
    public EnemyUnit[] m_Part1Turrets = new EnemyUnit[3];
    public EnemyUnit[] m_Part2Turrets = new EnemyUnit[4];
    public EnemyUnit[] m_Part3Turrets = new EnemyUnit[4];
    
    private int m_Phase;
    
    private Vector3 TARGET_POSITION;
    private const int APPEARANCE_TIME = 11000;
    private const int NEXT_PHASE_DELAY = 4000;

    private IEnumerator m_CurrentPhase;

    void Start()
    {
        RotateUnit(180f);

        TARGET_POSITION = transform.position;

        StartCoroutine(AppearanceSequence());
        
        DisableInteractableAll();

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDeath;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase > 0) {
            if (transform.position.x >= TARGET_POSITION.x + 1f) {
                m_MoveVector.direction = Random.Range(-110f, -70f);
            }
            else if (transform.position.x <= TARGET_POSITION.x - 1f) {
                m_MoveVector.direction = Random.Range(70f, 110f);
            }
            else if (transform.position.z >= TARGET_POSITION.z + 0.3f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
            else if (transform.position.z <= TARGET_POSITION.z - 0.3f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
        }

        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.625f) { // 체력 62.5% 이하
                for (int i = 0; i < m_Part1Turrets.Length; i++) {
                    m_Part1Turrets[i].m_EnemyDeath.OnDying();
                }
                BulletManager.SetBulletFreeState(2000);
                ToNextPhase(NEXT_PHASE_DELAY);
            }
        }
        else if (m_Phase == 2) {
            if (m_EnemyHealth.HealthPercent <= 0.25f) { // 체력 25% 이하
                for (int i = 0; i < m_Part2Turrets.Length; i++) {
                    m_Part2Turrets[i].m_EnemyDeath.OnDying();
                }
                BulletManager.SetBulletFreeState(2000);
                ToNextPhase(NEXT_PHASE_DELAY);
            }
        }
    }

    public IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME);
        OnAppearanceComplete();
        yield break;
    }

    public void OnAppearanceComplete() {
        float random_direction = Random.Range(70f, 110f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.5f, random_direction);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        for (int i = 0; i < m_Part1Turrets.Length; i++) {
            m_Part1Turrets[i].EnableInteractable();
        }
        
        SystemManager.OnBossStart();
    }

    public void ToNextPhase(int duration) {
        if (m_Phase == -1)
            return;

        m_Phase++;
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        if (m_Phase == 2) { // Phase 1 to 2
            for (int i = 0; i < m_Part2Turrets.Length; i++) {
                m_Part2Turrets[i].DisableInteractable(duration);
            }
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
            BackgroundCamera.MoveBackgroundCamera(true, 13f, duration);
        }
        else if (m_Phase == 3) { // Phase 2 to 3
            for (int i = 0; i < m_Part3Turrets.Length; i++) {
                m_Part3Turrets[i].DisableInteractable(duration);
            }
            DisableInteractable(duration);

            m_CurrentPhase = Phase3();
            StartCoroutine(m_CurrentPhase);
            BackgroundCamera.MoveBackgroundCamera(true, -7.5f, duration);
        }
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        m_Part1Turrets[1].StartPattern("0", new BulletPattern_EnemyBoss2_Part1_Turret2_0(m_Part1Turrets[1]));
        m_Part1Turrets[2].StartPattern("0", new BulletPattern_EnemyBoss2_Part1_Turret2_0(m_Part1Turrets[2]));
        yield return new WaitForMillisecondFrames(1000);
        
        while (m_Phase == 1) {
            yield return Phase1_PatternA();
            
            var side = Random.Range(0, 2) * 2 - 1;
            yield return Phase1_PatternB(side);

            yield return Phase1_PatternC(side);
        }
    }

    private IEnumerator Phase1_PatternA()
    {
        var side = Random.Range(0, 2) * 2 - 1;
        m_Part1Turrets[0].StartPattern("1A", new BulletPattern_EnemyBoss2_Part1_Turret1_1A(m_Part1Turrets[1]));
        m_Part1Turrets[1].StartPattern("1A", new BulletPattern_EnemyBoss2_Part1_Turret2_1A(m_Part1Turrets[1], side));
        m_Part1Turrets[2].StartPattern("1A", new BulletPattern_EnemyBoss2_Part1_Turret2_1A(m_Part1Turrets[2], -side));
        yield return new WaitWhile(() => m_Part1Turrets[0].IsExecutingPattern);
        
        m_Part1Turrets[1].StopPattern("1A");
        m_Part1Turrets[2].StopPattern("1A");
    }

    private IEnumerator Phase1_PatternB(int side) {
        //m_Part1Turrets[1].PrepareRotate(0, side[random_value]);
        //m_Part1Turrets[2].PrepareRotate(0, side[random_value]);
        m_Part1Turrets[1].StartPattern("1B", new BulletPattern_EnemyBoss2_Part1_Turret2_1B(m_Part1Turrets[1], side));
        m_Part1Turrets[2].StartPattern("1B", new BulletPattern_EnemyBoss2_Part1_Turret2_1B(m_Part1Turrets[2], -side));
        yield return new WaitForMillisecondFrames(1200);

        m_Part1Turrets[0].StartPattern("1B", new BulletPattern_EnemyBoss2_Part1_Turret1_1B(m_Part1Turrets[0]));
        //m_Part1Turrets[1].StartPattern(2);
        //m_Part1Turrets[2].StartPattern(2);
        yield return new WaitForMillisecondFrames(6000);
        m_Part1Turrets[0].StopPattern("1B");
        m_Part1Turrets[1].StopPattern("1B");
        m_Part1Turrets[2].StopPattern("1B");
    }

    private IEnumerator Phase1_PatternC(int side) {
        m_Part1Turrets[1].StartPattern("1C", new BulletPattern_EnemyBoss2_Part1_Turret2_1C(m_Part1Turrets[1], side));
        m_Part1Turrets[2].StartPattern("1C", new BulletPattern_EnemyBoss2_Part1_Turret2_1C(m_Part1Turrets[2], -side));
        yield return new WaitForMillisecondFrames(4000);
    }

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        //m_Part2Turrets[2].StartPattern(0);
        //m_Part2Turrets[3].StartPattern(0);
        yield return new WaitForMillisecondFrames(5000);
        while (m_Phase == 2) {
            yield return Pattern2A();
            yield return new WaitForMillisecondFrames(1500);

            yield return Pattern2B();
            yield return new WaitForMillisecondFrames(1000);
        }
    }

    private IEnumerator Pattern2A() {
        m_Part2Turrets[1].StartPattern("2A", new BulletPattern_EnemyBoss2_Part2_Turret2_2A(m_Part2Turrets[1]));
        if (PlayerManager.GetPlayerPosition().x < 0f) {
            m_Part2Turrets[2].StartPattern("2A", new BulletPattern_EnemyBoss2_Part2_Turret3_2A(m_Part2Turrets[2], 1));
            m_Part2Turrets[3].StartPattern("2A", new BulletPattern_EnemyBoss2_Part2_Turret3_2A(m_Part2Turrets[3], 0));
        }
        else {
            m_Part2Turrets[2].StartPattern("2A", new BulletPattern_EnemyBoss2_Part2_Turret3_2A(m_Part2Turrets[2], 0));
            m_Part2Turrets[3].StartPattern("2A", new BulletPattern_EnemyBoss2_Part2_Turret3_2A(m_Part2Turrets[3], 1));
        }
        yield return new WaitWhile(() => m_Part2Turrets[1].IsExecutingPattern);
    }

    private IEnumerator Pattern2B() {
        m_Part2Turrets[0].StartPattern("2B", new BulletPattern_EnemyBoss2_Part2_Turret1_2B(m_Part2Turrets[0]));
        yield return new WaitWhile(() => m_Part2Turrets[0].IsExecutingPattern);
    }

    private IEnumerator Phase3() { // 페이즈3 패턴 ============================
        yield return new WaitForMillisecondFrames(5000);
        m_Part3Turrets[0].StartPattern("3A", new BulletPattern_EnemyBoss2_Part3_Turret1_3A(m_Part3Turrets[0], 
            value => ((EnemyBoss2_Part3_Turret1) m_Part3Turrets[0]).Side = value));
        m_Part3Turrets[1].StartPattern("3A", new BulletPattern_EnemyBoss2_Part3_Turret2_3A(m_Part3Turrets[1]));
        m_Part3Turrets[2].StartPattern("3A", new BulletPattern_EnemyBoss2_Part3_Turret2_3A(m_Part3Turrets[2]));
        m_Part3Turrets[3].StartPattern("3A", new BulletPattern_EnemyBoss2_Part3_Turret3_3A(m_Part3Turrets[3]));
        while (m_EnemyHealth.CurrentHealth >= 7000)
            yield return new WaitForMillisecondFrames(0);
        
        m_Part3Turrets[0].StopPattern("3A");
        yield return new WaitForMillisecondFrames(500);
        
        m_Part3Turrets[0].StartPattern("3B", new BulletPattern_EnemyBoss2_Part3_Turret1_3B(m_Part3Turrets[0], 
            value => ((EnemyBoss2_Part3_Turret1) m_Part3Turrets[0]).Side = value));
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0f, 0f);

        for (int i = 0; i < m_Part3Turrets.Length; i++) {
            m_Part3Turrets[i].m_EnemyDeath.OnDying();
        }
        
        yield break;
    }

    public void OnBossDying() {
        SystemManager.OnBossClear();
    }

    public void OnBossDeath() {
        SystemManager.Instance.StartStageClearCoroutine();
        InGameScreenEffectService.WhiteEffect(true);
        MainCamera.ShakeCamera(1f);
    }
}
