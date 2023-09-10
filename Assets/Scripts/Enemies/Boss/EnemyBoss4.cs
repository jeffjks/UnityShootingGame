using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4 : EnemyUnit, IEnemyBossMain, IHasPhase
{
    public EnemyBoss4_SmallTurret[] m_SmallTurrets = new EnemyBoss4_SmallTurret[4];
    public EnemyBoss4_FrontTurret[] m_FrontTurrets = new EnemyBoss4_FrontTurret[2];
    public EnemyBoss4_MainTurret m_MainTurret;
    public EnemyBoss4_SubTurret[] m_SubTurrets = new EnemyBoss4_SubTurret[2];
    public EnemyBoss4_Launcher[] m_Launchers = new EnemyBoss4_Launcher[2];
    public MeshRenderer m_Track;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;
    public Animator m_MissileLauncherAnimator;

    private int m_Phase;

    private readonly int _missileLauncherMoving = Animator.StringToHash("Moving");
    private const int APPEARANCE_TIME = 8000;
    //private int m_MoveDirection;
    //private float m_MoveSpeed, m_DefaultSpeed = 0.005f;
    private float _trackPos;
    private bool _followingBackground;
    private EnemyUnit[] _childEnemyUnits;
    private Material _trackMaterial;

    private IEnumerator m_CurrentPhase;

    private void Start()
    {
        m_MoveVector = new MoveVector(0f, 180f); // new MoveVector(-4.5f, 180f);
        m_CustomDirection = new CustomDirection();
        
        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnKilled += OnBossKilled;
        m_EnemyDeath.Action_OnEndDeathAnimation += OnEndBossDeathAnimation;
        m_EnemyDeath.Action_OnRemoved += OnEndBossDeathAnimation;

        _trackMaterial = m_Track.material;
        _childEnemyUnits = GetComponentsInChildren<EnemyUnit>();
    }

    private IEnumerator AppearanceSequence() {
        if (DebugOption.SceneMode > 0)
            yield return new WaitForMillisecondFrames(1500);
        else
            yield return new WaitForMillisecondFrames(APPEARANCE_TIME);

        float init_speed = m_MoveVector.speed;
        float target_speed = BackgroundCamera.GetBackgroundCameraMoveVector().z;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, target_speed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        _followingBackground = true;
        
        EnableInteractableAll();
        
        SystemManager.OnBossStart();
    }

    private void ControlSpeed() {
        if (m_Phase == -1) { // OnDeath
            if (m_MoveVector.speed > 0f) {
                m_MoveVector.speed -= 0.5f / Application.targetFrameRate * Time.timeScale;
            }
            else {
                m_MoveVector.speed = 0f;
            }
            return;
        }
        if (_followingBackground) {
            m_MoveVector.speed = BackgroundCamera.GetBackgroundCameraMoveVector().z; // 배경 속도에 맞추기
        }
    }

    protected override void Update()
    {
        base.Update();
        
        ControlSpeed();

        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.65f) { // 체력 65% 이하
                ToNextPhase();
            }
        }
        else if (m_Phase == 2) {
            m_CustomDirection[0] += 80f / Application.targetFrameRate * Time.timeScale;
        }

        RunTracks();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private void RunTracks() {
        _trackMaterial.SetTextureOffset("_BaseMap", new Vector2(_trackPos, 0f));
        _trackPos += m_MoveVector.speed / Application.targetFrameRate * Time.timeScale;
        _trackPos = Mathf.Repeat(_trackPos, 1f);
    }

    public void ToNextPhase() {
        m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
        m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
        m_MissileLauncherAnimator.SetBool(_missileLauncherMoving, false);

        StopAllSubUnitPattern();
        StopAllPatterns();
        
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        if (m_Phase == 1) {
            foreach (var frontTurret in m_FrontTurrets) {
                if (frontTurret != null)
                    frontTurret.m_EnemyDeath.KillEnemy();
            }
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
            BulletManager.SetBulletFreeState(2000);
            NextPhaseExplosion();
            m_Phase++;
        }
        else if (m_Phase == 2) {
            m_CurrentPhase = Phase3();
            StartCoroutine(m_CurrentPhase);
            m_Phase++;
        }
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1500);
        
        foreach (var smallTurret in m_SmallTurrets)
        {
            smallTurret.StartPattern("0", new BulletPattern_EnemyBoss4_SmallTurret_0(smallTurret));
        }
        yield return new WaitForMillisecondFrames(1000);

        StartPattern("1A1", new BulletPattern_EnemyBoss4_1A1(this, ExecuteFrontTurretPattern_1A));
        yield return StartPattern("1A2", new BulletPattern_EnemyBoss4_1A2(this, ExecuteSubTurretPattern_1A));
        
        StopAllPatterns();
        var rand = 1;

        while (m_Phase == 1) {
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetAngle(0f, 150f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetAngle(0f, 150f));
            if (m_FrontTurrets[0] != null)
                m_FrontTurrets[0].SetRotatePattern(new RotatePattern_TargetAngle(0f, 150f));
            if (m_FrontTurrets[1] != null)
                m_FrontTurrets[1].SetRotatePattern(new RotatePattern_TargetAngle(0f, 150f));
            yield return new WaitForMillisecondFrames(2000);
            foreach (var smallTurret in m_SmallTurrets)
            {
                smallTurret.StopPattern("0");
            }
            
            foreach (var subTurret in m_SubTurrets) {
                if (subTurret != null)
                    subTurret.StartPattern("1B", new BulletPattern_EnemyBoss4_SubTurret_1B(subTurret));
            }
            foreach (var frontTurret in m_FrontTurrets) {
                if (frontTurret != null)
                    frontTurret.StartPattern("1B", new BulletPattern_EnemyBoss4_FrontTurret_1B(frontTurret));
            }
            
            var rand1B = Random.Range(0, 2);
            m_Launchers[0].StartPattern("1B", new BulletPattern_EnemyBoss4_Launcher_1B(m_Launchers[0], rand1B));
            m_Launchers[1].StartPattern("1B", new BulletPattern_EnemyBoss4_Launcher_1B(m_Launchers[1], 1 - rand1B));
            
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern("1B1", new BulletPattern_EnemyBoss4_MainTurret_1B1(m_MainTurret));
            m_MainTurret.StartPattern("1B2", new BulletPattern_EnemyBoss4_MainTurret_1B2(m_MainTurret));
            yield return new WaitForMillisecondFrames(6000);
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetAngle(130f, 100f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetAngle(130f, 100f));
            if (m_FrontTurrets[0] != null)
                m_FrontTurrets[0].SetRotatePattern(new RotatePattern_TargetPlayer());
            if (m_FrontTurrets[1] != null)
                m_FrontTurrets[1].SetRotatePattern(new RotatePattern_TargetPlayer());
            StopAllSubUnitPattern();
            StopAllPatterns();
            
            yield return new WaitForMillisecondFrames(1000);
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetAngle(270f, 150f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetAngle(90f, 150f));
            yield return new WaitForMillisecondFrames(1000);
            
            foreach (var smallTurret in m_SmallTurrets)
            {
                smallTurret.StartPattern("0", new BulletPattern_EnemyBoss4_SmallTurret_0(smallTurret));
            }
            yield return new WaitForMillisecondFrames(1000);
            
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_RotateAround(140f * rand));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_RotateAround(140f * rand));
            m_SubTurrets[0].StartPattern("1C", new BulletPattern_EnemyBoss4_SubTurret_1C(m_SubTurrets[0]));
            m_SubTurrets[1].StartPattern("1C", new BulletPattern_EnemyBoss4_SubTurret_1C(m_SubTurrets[1]));
            StartPattern("1C1", new BulletPattern_EnemyBoss4_1C1(this, m_Launchers));
            StartPattern("1C2", new BulletPattern_EnemyBoss4_1C2(this, m_FrontTurrets));

            yield return new WaitForMillisecondFrames(6400);
            StopAllSubUnitPattern();
            StopAllPatterns();
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));

            yield return new WaitForMillisecondFrames(2000);
            StartPattern("1A2", new BulletPattern_EnemyBoss4_1A2(this, ExecuteFrontTurretPattern_1A));
            yield return StartPattern("1D2", new BulletPattern_EnemyBoss4_1D2(this, ExecuteSubTurretPattern_1D));

            StopAllPatterns();
            rand *= -1;
        }
    }

    private void ExecuteFrontTurretPattern_1A(int side)
    {
        if (m_FrontTurrets[side] != null) {
            m_FrontTurrets[side].StartPattern("1A", new BulletPattern_EnemyBoss4_FrontTurret_1A(m_FrontTurrets[side]));
        }
        else if (m_FrontTurrets[1-side] != null) {
            m_FrontTurrets[1-side].StartPattern("1A", new BulletPattern_EnemyBoss4_FrontTurret_1A(m_FrontTurrets[1-side]));
        }
    }

    private void ExecuteSubTurretPattern_1A(int index)
    {
        m_SubTurrets[index].StartPattern("1A", new BulletPattern_EnemyBoss4_SubTurret_1A(m_SubTurrets[index]));
    }

    private void ExecuteSubTurretPattern_1D(int index)
    {
        m_SubTurrets[index].StartPattern("1D", new BulletPattern_EnemyBoss4_SubTurret_1D(m_SubTurrets[index]));
    }

    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        var first = true;
        yield return new WaitForMillisecondFrames(3000);
        
        foreach (var smallTurret in m_SmallTurrets)
        {
            smallTurret.StartPattern("0", new BulletPattern_EnemyBoss4_SmallTurret_0(smallTurret));
        }
        yield return new WaitForMillisecondFrames(1000);

        while (m_Phase == 2) {
            var rand = Random.Range(0, 2) * 2 - 1;
            m_Launchers[0].StartPattern("2A", new BulletPattern_EnemyBoss4_Launcher_2A(m_Launchers[0], rand));
            m_Launchers[1].StartPattern("2A", new BulletPattern_EnemyBoss4_Launcher_2A(m_Launchers[1], rand));
            m_MissileLauncherAnimator.SetBool(_missileLauncherMoving, true);
            yield return StartPattern("2A", new BulletPattern_EnemyBoss4_2A(this, m_MainTurret));
            
            m_Launchers[0].StopPattern("2A");
            m_Launchers[1].StopPattern("2A");
            m_MissileLauncherAnimator.SetBool(_missileLauncherMoving, false);
            if (m_EnemyHealth.HealthPercent <= 0.25f) { // 체력 25% 이하
                ToNextPhase();
                break;
            }
            yield return new WaitForMillisecondFrames(1500);

            var side = Random.Range(0, 2) * 2 - 1;
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetPlayer(m_SubTurrets[0].AngleToPlayer).SetOffsetAngle(70f*side));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetPlayer(m_SubTurrets[1].AngleToPlayer).SetOffsetAngle(70f*side));
            yield return new WaitForMillisecondFrames(1000);
            m_SubTurrets[0].StartPattern("2B", new BulletPattern_EnemyBoss4_SubTurret_2B(m_SubTurrets[0], side));
            m_SubTurrets[1].StartPattern("2B", new BulletPattern_EnemyBoss4_SubTurret_2B(m_SubTurrets[1], side));
            yield return new WaitForMillisecondFrames(1000);
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
            if (m_EnemyHealth.HealthPercent <= 0.25f) { // 체력 25% 이하
                ToNextPhase();
                break;
            }
            
            yield return new WaitForMillisecondFrames(first ? 2000 : 1000);
            first = false;

            m_MainTurret.StartPattern("2C", new BulletPattern_EnemyBoss4_MainTurret_2C(m_MainTurret));
            m_SubTurrets[0].StartPattern("2C", new BulletPattern_EnemyBoss4_SubTurret_2C(m_SubTurrets[0]));
            m_SubTurrets[1].StartPattern("2C", new BulletPattern_EnemyBoss4_SubTurret_2C(m_SubTurrets[1]));
            yield return new WaitForMillisecondFrames(7000);
            StopAllSubUnitPattern();
            if (m_EnemyHealth.HealthPercent <= 0.25f) { // 체력 25% 이하
                ToNextPhase();
                break;
            }
            yield return new WaitForMillisecondFrames(2000);
        }
    }

    private IEnumerator Phase3() { // 페이즈3 패턴 ============================
        var rand = Random.Range(0, 2) * 2 - 1;
        m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetAngle(0f, 150f));
        m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetAngle(0f, 150f));
        yield return new WaitForMillisecondFrames(2500);

        while (m_Phase == 3) {
            m_MainTurret.SetRotatePattern(new RotatePattern_TargetAngle(45f * rand, 80f));
            m_SubTurrets[(rand + 1) / 2].StartPattern("3A", new BulletPattern_EnemyBoss4_SubTurret_3A(m_SubTurrets[(rand + 1) / 2]));
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern("3A", new BulletPattern_EnemyBoss4_MainTurret_3A(m_MainTurret));
            yield return new WaitForMillisecondFrames(1000);
            rand *= -1;
            m_MainTurret.SetRotatePattern(new RotatePattern_TargetAngle(45f * rand, 80f));
            m_SubTurrets[(rand + 1) / 2].StartPattern("3A", new BulletPattern_EnemyBoss4_SubTurret_3A(m_SubTurrets[(rand + 1) / 2]));
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern("3A", new BulletPattern_EnemyBoss4_MainTurret_3A(m_MainTurret));
            yield return new WaitForMillisecondFrames(1000);
            rand *= -1;
        }
    }

    private void StopAllSubUnitPattern()
    {
        foreach (var childEnemyUnit in _childEnemyUnits)
        {
            childEnemyUnit.StopAllPatterns();
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        StopAllPatterns();
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        foreach (var smallTurret in m_SmallTurrets)
        {
            if (smallTurret != null)
                smallTurret.m_EnemyDeath.KillEnemy();
        }
        BulletManager.BulletsToGems(2000);
        
        yield return new WaitForMillisecondFrames(1000);
        MainCamera.ShakeCamera(0.5f);
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
