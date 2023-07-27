using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4 : EnemyUnit, IHasAppearance, IEnemyBossMain
{
    public EnemyBoss4_SmallTurret[] m_SmallTurrets = new EnemyBoss4_SmallTurret[4];
    public EnemyBoss4_FrontTurret[] m_FrontTurrets = new EnemyBoss4_FrontTurret[2];
    public EnemyBoss4_MainTurret m_MainTurret;
    public EnemyBoss4_SubTurret[] m_SubTurrets = new EnemyBoss4_SubTurret[2];
    public EnemyBoss4_Launcher[] m_Launchers = new EnemyBoss4_Launcher[2];
    public MeshRenderer m_Track;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int m_Phase;
    
    private const int APPEARANCE_TIME = 8000;
    //private int m_MoveDirection;
    //private float m_MoveSpeed, m_DefaultSpeed = 0.005f;
    private float _trackPos;
    private bool _followingBackground;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        m_MoveVector = new MoveVector(0f, 180f); // new MoveVector(-4.5f, 180f);
        m_CustomDirection = new CustomDirection();
        
        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDeath;
    }

    public IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME);

        float init_speed = m_MoveVector.speed;
        float target_speed = BackgroundCamera.GetBackgroundVector().z;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, target_speed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    public void OnAppearanceComplete() {
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
            m_MoveVector.speed = BackgroundCamera.GetBackgroundVector().z; // 배경 속도에 맞추기
        }
    }

    protected override void Update()
    {
        base.Update();
        
        ControlSpeed();

        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.65f) { // 체력 65% 이하
                for (int i = 0; i < m_FrontTurrets.Length; i++) {
                    if (m_FrontTurrets[i] != null)
                        m_FrontTurrets[i].m_EnemyDeath.OnDying();
                }
                BulletManager.SetBulletFreeState(2000);
                NextPhaseExplosion();
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
        Material material = m_Track.material;
        material.SetTextureOffset("_MainTex", new Vector2(_trackPos, 0f));
        _trackPos += m_MoveVector.speed / Application.targetFrameRate * Time.timeScale;
        if (_trackPos > 1f)
            _trackPos--;
    }

    public void ToNextPhase() {
        m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
        m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
        m_Launchers[0].SetMoving(false);
        m_Launchers[1].SetMoving(false);
        m_Phase++;
        
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            if (m_SmallTurrets[i] != null)
                m_SmallTurrets[i].StopAllPatterns();
        }
        for (int i = 0; i < m_SubTurrets.Length; i++) {
            if (m_SubTurrets[i] != null)
                m_SubTurrets[i].StopAllPatterns();
        }
        for (int i = 0; i < m_Launchers.Length; i++) {
            m_Launchers[i].StopAllPatterns();
        }
        m_MainTurret.StopAllPatterns();
        
        StopAllPatterns();
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        if (m_Phase == 2) {
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
        }
        else if (m_Phase == 3) {
            m_CurrentPhase = Phase3();
            StartCoroutine(m_CurrentPhase);
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
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_Target(0f, 150f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_Target(0f, 150f));
            m_FrontTurrets[0].SetRotatePattern(new RotatePattern_Target(0f, 150f));
            m_FrontTurrets[1].SetRotatePattern(new RotatePattern_Target(0f, 150f));
            yield return new WaitForMillisecondFrames(2000);
            foreach (var smallTurret in m_SmallTurrets)
            {
                smallTurret.StopPattern("0");
            }

            m_CurrentPattern1 = Pattern1B1();
            StartCoroutine(m_CurrentPattern1);
            m_CurrentPattern2 = Pattern1B2();
            StartCoroutine(m_CurrentPattern2);
            
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern("1B1", new BulletPattern_EnemyBoss4_MainTurret_1B1(m_MainTurret));
            m_MainTurret.StartPattern("1B2", new BulletPattern_EnemyBoss4_MainTurret_1B2(m_MainTurret));
            yield return new WaitForMillisecondFrames(6000);
            m_MainTurret.StopPattern("1B1");
            m_MainTurret.StopPattern("1B2");
            m_Launchers[0].StopPattern("1B");
            m_Launchers[1].StopPattern("1B");
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_Target(130f, 100f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_Target(130f, 100f));
            m_FrontTurrets[0].SetRotatePattern(new RotatePattern_TargetPlayer());
            m_FrontTurrets[1].SetRotatePattern(new RotatePattern_TargetPlayer());
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(1000);
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_Target(270f, 150f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_Target(90f, 150f));
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
            m_CurrentPattern1 = Pattern1C1();
            StartCoroutine(m_CurrentPattern1);
            m_CurrentPattern2 = Pattern1C2();
            StartCoroutine(m_CurrentPattern2);

            yield return new WaitForMillisecondFrames(5000);
            m_SubTurrets[0].StopPattern("1C");
            m_SubTurrets[1].StopPattern("1C");
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_TargetPlayer(130f, 100f));
            StopAllPatterns();

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

    private IEnumerator Pattern1B1() {
        int[] n = {2, 3, 4};

        for (int i = 0; i < 6; i++) {
            foreach (var subTurret in m_SubTurrets) {
                if (subTurret != null)
                    subTurret.StartPattern("1B", new BulletPattern_EnemyBoss4_SubTurret_1B(subTurret));
            }
            foreach (var frontTurret in m_FrontTurrets) {
                if (frontTurret != null)
                    frontTurret.StartPattern("1B", new BulletPattern_EnemyBoss4_FrontTurret_1B(frontTurret));
            }
            
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                yield return new WaitForMillisecondFrames(3000);
            }
            else {
                yield return new WaitForMillisecondFrames(2000);
            }
        }
    }

    private IEnumerator Pattern1B2() {
        int rand1 = Random.Range(0, 2);
        var rand2 = Random.Range(0, 2) * 2 - 1;

        while (true) {
            m_CustomDirection[0] = Random.Range(0f, 360f);
            m_Launchers[rand1].StartPattern("1B",
                new BulletPattern_EnemyBoss4_Launcher_1B(m_Launchers[rand1], rand2));
            rand2 = (2*Random.Range(0, 2) - 1); // -1 or 1
            float difficulty = (int) SystemManager.Difficulty;
            for (int i = 0; i < 32; i++) {
                m_CustomDirection[0] += (20f + difficulty*5f) * rand2 / Application.targetFrameRate * Time.timeScale;
                yield return new WaitForMillisecondFrames(0);
            }
            rand2 = (2*Random.Range(0, 2) - 1); // -1 or 1
            for (int i = 0; i < 32; i++) {
                m_CustomDirection[0] -= (20f + difficulty*5f) * rand2 / Application.targetFrameRate * Time.timeScale;
                yield return new WaitForMillisecondFrames(0);
            }
            m_Launchers[rand1].StopPattern("1B");
            rand1 = 1 - rand1;
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private IEnumerator Pattern1C1() {
        while (true) {
            var rand = Random.Range(0, 2);
            m_Launchers[rand].StartPattern("1C", new BulletPattern_EnemyBoss4_Launcher_1C(m_Launchers[rand]));
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                yield return new WaitForMillisecondFrames(1800);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                yield return new WaitForMillisecondFrames(1200);
            }
            else {
                yield return new WaitForMillisecondFrames(800);
            }
        }
    }

    private IEnumerator Pattern1C2() {
        int r = Random.Range(0, 2);
        yield return new WaitForMillisecondFrames(1000);

        while (true) {
            if (m_FrontTurrets[r] != null) {
                m_FrontTurrets[r].StartPattern("1C", new BulletPattern_EnemyBoss4_FrontTurret_1C(m_FrontTurrets[r]));
            }
            else if (m_FrontTurrets[1-r] != null) {
                m_FrontTurrets[1-r].StartPattern("1C", new BulletPattern_EnemyBoss4_FrontTurret_1C(m_FrontTurrets[1-r]));
            }
            else {
                yield break;
            }
            
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                yield return new WaitForMillisecondFrames(3000);
            }
            else {
                yield return new WaitForMillisecondFrames(2000);
            }
            r = 1-r;
        }
    }

    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        bool first = true;
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
            m_Launchers[0].SetMoving(true);
            m_Launchers[1].SetMoving(true);
            m_CurrentPattern1 = Pattern2A();
            yield return StartCoroutine(m_CurrentPattern1);
            
            m_Launchers[0].StopPattern("2A");
            m_Launchers[1].StopPattern("2A");
            m_Launchers[0].SetMoving(false);
            m_Launchers[1].SetMoving(false);
            if (m_EnemyHealth.HealthPercent <= 0.25f) { // 체력 25% 이하
                ToNextPhase();
                break;
            }
            yield return new WaitForMillisecondFrames(1500);

            var side = Random.Range(0, 2) * 2 - 1;
            m_SubTurrets[0].SetRotatePattern(new RotatePattern_Target(AngleToPlayer + 70f*side, 180f));
            m_SubTurrets[1].SetRotatePattern(new RotatePattern_Target(AngleToPlayer + 70f*side, 180f));
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
            if (first)
                yield return new WaitForMillisecondFrames(2000);
            first = false;

            m_MainTurret.StartPattern("2C", new BulletPattern_EnemyBoss4_MainTurret_2C(m_MainTurret));
            m_SubTurrets[0].StartPattern("2C", new BulletPattern_EnemyBoss4_SubTurret_2C(m_SubTurrets[0]));
            m_SubTurrets[1].StartPattern("2C", new BulletPattern_EnemyBoss4_SubTurret_2C(m_SubTurrets[1]));
            yield return new WaitForMillisecondFrames(7000);
            m_MainTurret.StopPattern("2C");
            m_SubTurrets[0].StopPattern("2C");
            m_SubTurrets[1].StopPattern("2C");
            if (m_EnemyHealth.HealthPercent <= 0.25f) { // 체력 25% 이하
                ToNextPhase();
                break;
            }
            yield return new WaitForMillisecondFrames(2000);
        }
    }

    private IEnumerator Pattern2A()
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            int[] repeatNum = { 1, 1, 2, 2, 2 };
            foreach (var num in repeatNum)
            {
                for (var i = 0; i < num; ++i)
                {
                    m_MainTurret.StartPattern("2A", new BulletPattern_EnemyBoss4_MainTurret_2A(m_MainTurret));
                    yield return new WaitForMillisecondFrames(500);
                }
                yield return new WaitForMillisecondFrames(1000);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            int[] repeatNum = { 1, 2, 2, 3, 3 };
            foreach (var num in repeatNum)
            {
                for (var i = 0; i < num; ++i)
                {
                    m_MainTurret.StartPattern("2A", new BulletPattern_EnemyBoss4_MainTurret_2A(m_MainTurret));
                    yield return new WaitForMillisecondFrames(400);
                }
                yield return new WaitForMillisecondFrames(900);
            }
        }
        else {
            int[] repeatNum = { 1, 2, 3, 3, 3 };
            foreach (var num in repeatNum)
            {
                for (var i = 0; i < num; ++i)
                {
                    m_MainTurret.StartPattern("2A", new BulletPattern_EnemyBoss4_MainTurret_2A(m_MainTurret));
                    yield return new WaitForMillisecondFrames(400);
                }
                yield return new WaitForMillisecondFrames(800);
            }
        }
    }

    private IEnumerator Phase3() { // 페이즈3 패턴 ============================
        var rand = Random.Range(0, 2) * 2 - 1;
        m_SubTurrets[0].SetRotatePattern(new RotatePattern_Target(0f, 150f));
        m_SubTurrets[1].SetRotatePattern(new RotatePattern_Target(0f, 150f));
        yield return new WaitForMillisecondFrames(2500);

        while (m_Phase == 3) {
            m_MainTurret.SetRotatePattern(new RotatePattern_Target(45f * rand, 80f));
            m_SubTurrets[(rand + 1) / 2].StartPattern("3A", new BulletPattern_EnemyBoss4_SubTurret_3A(m_SubTurrets[(rand + 1) / 2]));
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern("3A", new BulletPattern_EnemyBoss4_MainTurret_3A(m_MainTurret));
            yield return new WaitForMillisecondFrames(1000);
            rand *= -1;
            m_MainTurret.SetRotatePattern(new RotatePattern_Target(45f * rand, 80f));
            m_SubTurrets[(rand + 1) / 2].StartPattern("3A", new BulletPattern_EnemyBoss4_SubTurret_3A(m_SubTurrets[(rand + 1) / 2]));
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern("3A", new BulletPattern_EnemyBoss4_MainTurret_3A(m_MainTurret));
            yield return new WaitForMillisecondFrames(1000);
            rand *= -1;
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
                smallTurret.m_EnemyDeath.OnDying();
        }
        BulletManager.BulletsToGems(2000);
        
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
