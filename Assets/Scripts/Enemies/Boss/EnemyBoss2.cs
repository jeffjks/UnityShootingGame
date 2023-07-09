using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2 : EnemyUnit, IHasAppearance, IEnemyBossMain
{
    public EnemyUnit[] m_Part1_Turrets = new EnemyUnit[3];
    public EnemyUnit[] m_Part2_Turrets = new EnemyUnit[4];
    public EnemyUnit[] m_Part3_Turrets = new EnemyUnit[4];
    
    private int m_Phase;
    public int Phase {
        get { return m_Phase; }
        set {
            m_Phase = value;
            (m_Part1_Turrets[0] as EnemyBoss2Turret0_1).m_Phase = value;
            (m_Part1_Turrets[1] as EnemyBoss2Turret0_1).m_Phase = value;
        }
    }
    
    private Vector3 TARGET_POSITION;
    private const int APPEARANCE_TIME = 11000;
    private const int NEXT_PHASE_DELAY = 4000;

    private bool m_InPattern = false;

    private IEnumerator m_CurrentPhase, m_CurrentPattern;

    void Start()
    {
        CurrentAngle = 180f;
        RotateImmediately(CurrentAngle);

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
            if (m_EnemyHealth.m_HealthPercent <= 0.625f) { // 체력 62.5% 이하
                for (int i = 0; i < m_Part1_Turrets.Length; i++) {
                    m_Part1_Turrets[i].m_EnemyDeath.OnDying();
                }
                BulletManager.SetBulletFreeState(2000);
                ToNextPhase(NEXT_PHASE_DELAY);
            }
        }
        else if (m_Phase == 2) {
            if (m_EnemyHealth.m_HealthPercent <= 0.25f) { // 체력 25% 이하
                for (int i = 0; i < m_Part2_Turrets.Length; i++) {
                    m_Part2_Turrets[i].m_EnemyDeath.OnDying();
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

        for (int i = 0; i < m_Part1_Turrets.Length; i++) {
            m_Part1_Turrets[i].EnableInteractable();
        }
        
        SystemManager.OnBossStart();
    }

    public void ToNextPhase(int duration) {
        if (m_Phase == -1)
            return;

        m_Phase++;
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        if (m_Phase == 2) { // Phase 1 to 2
            for (int i = 0; i < m_Part2_Turrets.Length; i++) {
                m_Part2_Turrets[i].DisableInteractable(duration);
            }
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
            BackgroundCamera.MoveBackgroundCamera(true, 13f, duration);
        }
        else if (m_Phase == 3) { // Phase 2 to 3
            for (int i = 0; i < m_Part3_Turrets.Length; i++) {
                m_Part3_Turrets[i].DisableInteractable(duration);
            }
            DisableInteractable(duration);

            m_CurrentPhase = Phase3();
            StartCoroutine(m_CurrentPhase);
            BackgroundCamera.MoveBackgroundCamera(true, -7.5f, duration);
        }
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StartPattern(0);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StartPattern(0);
        yield return new WaitForMillisecondFrames(1000);
        int[] side = {-1, 1};
        int random_value;
        while (m_Phase == 1) {
            random_value = Random.Range(0, 2);
            m_CurrentPattern = Pattern1A();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).PrepareRotate(0, side[random_value]);
            ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).PrepareRotate(0, side[random_value]);
            random_value = Random.Range(0, 2);
            yield return new WaitForMillisecondFrames(1200);
                
            m_CurrentPattern = Pattern1B();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            
            ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).PrepareRotate(0, side[random_value]);
            ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).PrepareRotate(0, side[1 - random_value]);
            yield return new WaitForMillisecondFrames(1500);

            m_CurrentPattern = Pattern1C();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            yield return new WaitForMillisecondFrames(2000);
        }
        yield break;
    }

    private IEnumerator Pattern1A() {
        m_InPattern = true;
        
        ((EnemyBoss2Turret0_0) m_Part1_Turrets[0]).StartPattern(1);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StartPattern(1);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StartPattern(1);
        while(((EnemyBoss2Turret0_0)m_Part1_Turrets[0]).m_InPattern) {
            yield return new WaitForMillisecondFrames(0);
        }
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StopPattern();
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StopPattern();
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1B() {
        m_InPattern = true;
        ((EnemyBoss2Turret0_0) m_Part1_Turrets[0]).StartPattern(2);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StartPattern(2);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StartPattern(2);
        yield return new WaitForMillisecondFrames(6000);
        ((EnemyBoss2Turret0_0) m_Part1_Turrets[0]).StopPattern();
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StopPattern();
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StopPattern();

        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1C() {
        m_InPattern = true;
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[1]).StartPattern(3);
        ((EnemyBoss2Turret0_1) m_Part1_Turrets[2]).StartPattern(3);
        yield return new WaitForMillisecondFrames(500);

        m_InPattern = false;
        yield break;
    }

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        ((EnemyBoss2Turret1_2) m_Part2_Turrets[2]).StartPattern(0);
        ((EnemyBoss2Turret1_2) m_Part2_Turrets[3]).StartPattern(0);
        yield return new WaitForMillisecondFrames(5000);
        while (m_Phase == 2) {
            m_CurrentPattern = Pattern2A();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            yield return new WaitForMillisecondFrames(1500);

            m_CurrentPattern = Pattern2B();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            yield return new WaitForMillisecondFrames(1000);
        }
        yield break;
    }

    private IEnumerator Pattern2A() {
        m_InPattern = true;
        ((EnemyBoss2Turret1_1) m_Part2_Turrets[1]).StartPattern(1);
        if (PlayerManager.GetPlayerPosition().x < 0f) {
            ((EnemyBoss2Turret1_2) m_Part2_Turrets[2]).StartPattern(1, true);
           ((EnemyBoss2Turret1_2) m_Part2_Turrets[3]).StartPattern(1, false);
        }
        else {
            ((EnemyBoss2Turret1_2) m_Part2_Turrets[2]).StartPattern(1, false);
           ((EnemyBoss2Turret1_2) m_Part2_Turrets[3]).StartPattern(1, true);
        }
        while(((EnemyBoss2Turret1_1) m_Part2_Turrets[1]).m_InPattern) {
            yield return new WaitForMillisecondFrames(0);
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern2B() {
        m_InPattern = true;
        ((EnemyBoss2Turret1_0) m_Part2_Turrets[0]).StartPattern(1);
        while(((EnemyBoss2Turret1_0) m_Part2_Turrets[0]).m_InPattern) {
            yield return new WaitForMillisecondFrames(0);
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Phase3() { // 페이즈3 패턴 ============================
        yield return new WaitForMillisecondFrames(5000);
        ((EnemyBoss2Turret2_0) m_Part3_Turrets[0]).StartPattern(1);
        ((EnemyBoss2Turret2_1) m_Part3_Turrets[1]).StartPattern();
        ((EnemyBoss2Turret2_2) m_Part3_Turrets[2]).StartPattern();
        ((EnemyBoss2Turret2_2) m_Part3_Turrets[3]).StartPattern();
        while (m_EnemyHealth.CurrentHealth >= 7000)
            yield return new WaitForMillisecondFrames(0);
        ((EnemyBoss2Turret2_0) m_Part3_Turrets[0]).StopPattern();
        yield return new WaitForMillisecondFrames(500);
        ((EnemyBoss2Turret2_0) m_Part3_Turrets[0]).StartPattern(2);
        yield break;
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0f, 0f);

        for (int i = 0; i < m_Part3_Turrets.Length; i++) {
            m_Part3_Turrets[i].m_EnemyDeath.OnDying();
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
