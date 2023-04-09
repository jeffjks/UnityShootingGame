using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4 : EnemyBoss
{
    public EnemyBoss4SmallTurret[] m_SmallTurrets = new EnemyBoss4SmallTurret[4];
    public EnemyBoss4FrontTurret[] m_FrontTurrets = new EnemyBoss4FrontTurret[2];
    public EnemyBoss4MainTurret m_MainTurret;
    public EnemyBoss4SubTurret[] m_SubTurrets = new EnemyBoss4SubTurret[2];
    public EnemyBoss4Launcher[] m_Launchers = new EnemyBoss4Launcher[2];
    public MeshRenderer m_Track;

    [HideInInspector] public int m_Phase;
    [HideInInspector] public float m_Direction;
    
    private Vector3 m_TargetPosition;
    private const int APPEARNCE_TIME = 8000;
    private bool m_InPattern = false;
    //private int m_MoveDirection;
    //private float m_MoveSpeed, m_DefaultSpeed = 0.005f;
    private float m_TrackPos;
    private bool m_FollowingBackground = false;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        m_TargetPosition = transform.position;
        m_MoveVector = new MoveVector(0f, 180f); // new MoveVector(-4.5f, 180f);
        
        DisableAttackable();
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            m_SmallTurrets[i].DisableAttackable();
        }
        for (int i = 0; i < m_FrontTurrets.Length; i++) {
            m_FrontTurrets[i].DisableAttackable();
        }

        StartCoroutine(AppearanceSequence());
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARNCE_TIME);

        float init_speed = m_MoveVector.speed;
        float target_speed = m_SystemManager.m_StageManager.m_BackgroundVector.z;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, target_speed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
        yield break;
    }

    private void OnAppearanceComplete() {
        int random_speed = Random.Range(0, 2);
        //m_MoveDirection = random_speed*2 - 1;
        //m_MoveSpeed = m_DefaultSpeed*m_MoveDirection;
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        m_FollowingBackground = true;
        
        EnableAttackable();
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            m_SmallTurrets[i].EnableAttackable();
        }
        for (int i = 0; i < m_FrontTurrets.Length; i++) {
            m_FrontTurrets[i].EnableAttackable();
        }
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
        if (m_FollowingBackground) {
            m_MoveVector.speed = m_SystemManager.m_StageManager.m_BackgroundVector.z; // 배경 속도에 맞추기
        }
    }

    protected override void Update()
    {
        ControlSpeed();

        /*
        transform.position = new Vector3(transform.position.x + m_MoveSpeed / Application.targetFrameRate * Time.timeScale * 60f, transform.position.y, transform.position.z);
        if (m_MoveVector.speed < 0f) {
            m_MoveVector.speed += 1f / Application.targetFrameRate * Time.timeScale;
        }
        else {
            m_MoveVector.speed = 0f;
        }
        
        if (m_Phase > 0) {
            if (transform.position.x >= m_TargetPosition.x + 0.7f) {
                m_MoveDirection = -1;
            }
            else if (transform.position.x <= m_TargetPosition.x - 0.7f) {
                m_MoveDirection = 1;
            }
            if (m_MoveDirection == 1) {
                if (m_MoveSpeed < m_DefaultSpeed) {
                    m_MoveSpeed += m_MoveDirection*0.01f / Application.targetFrameRate * Time.timeScale;
                }
            }
            else if (m_MoveDirection == -1) {
                if (m_MoveSpeed > -m_DefaultSpeed) {
                    m_MoveSpeed += m_MoveDirection*0.01f / Application.targetFrameRate * Time.timeScale;
                }
            }
        }*/

        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 65 / 100) { // 체력 65% 이하
                for (int i = 0; i < m_FrontTurrets.Length; i++) {
                    if (m_FrontTurrets[i] != null)
                        m_FrontTurrets[i].OnDeath();
                }
                m_SystemManager.EraseBullets(2000);
                StartCoroutine(NextPhaseExplosion1(2000));
                StartCoroutine(NextPhaseExplosion2(2000));
                
                for (int i = 0; i < m_SmallTurrets.Length; i++) {
                    if (m_SmallTurrets[i] != null)
                        m_SmallTurrets[i].StopPattern();
                }
                for (int i = 0; i < m_SubTurrets.Length; i++) {
                    if (m_SubTurrets[i] != null)
                        m_SubTurrets[i].StopPattern();
                }
                for (int i = 0; i < m_Launchers.Length; i++) {
                    m_Launchers[i].StopPattern();
                }
                m_MainTurret.StopPattern();
                ToNextPhase();
            }
        }
        else if (m_Phase == 2) {
            m_Direction += 80f / Application.targetFrameRate * Time.timeScale;
        }

        if (m_Direction > 360f)
            m_Direction -= 360f;
        else if (m_Direction < 0f)
            m_Direction += 360f;

        RunTracks();
        base.Update();
    }

    private void RunTracks() {
        Material material = m_Track.material;
        material.SetTextureOffset("_MainTex", new Vector2(m_TrackPos, 0f));
        m_TrackPos += m_MoveVector.speed / Application.targetFrameRate * Time.timeScale;
        if (m_TrackPos > 1f)
            m_TrackPos--;
    }

    public void ToNextPhase() {
        m_SubTurrets[0].m_RotatePattern = 10;
        m_SubTurrets[1].m_RotatePattern = 10;
        m_Launchers[0].SetMoving(false);
        m_Launchers[1].SetMoving(false);
        m_Phase++;
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
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            if (m_SmallTurrets[i] != null)
                m_SmallTurrets[i].StartPattern(1);
        }
        yield return new WaitForMillisecondFrames(1000);
        m_InPattern = true;
        m_CurrentPattern1 = Pattern1A1();
        StartCoroutine(m_CurrentPattern1);
        m_CurrentPattern2 = Pattern1A2();
        StartCoroutine(m_CurrentPattern2);
        while (m_InPattern)
            yield return new WaitForMillisecondFrames(0);

        StopAllPatterns();

        while(m_Phase == 1) {
            m_SubTurrets[0].m_RotatePattern = 20;
            m_SubTurrets[1].m_RotatePattern = 20;
            m_FrontTurrets[0].m_RotatePattern = 20;
            m_FrontTurrets[1].m_RotatePattern = 20;
            yield return new WaitForMillisecondFrames(2000);
            for (int i = 0; i < m_SmallTurrets.Length; i++) {
                if (m_SmallTurrets[i] != null)
                    m_SmallTurrets[i].StopPattern();
            }

            m_CurrentPattern1 = Pattern1B1();
            StartCoroutine(m_CurrentPattern1);
            m_CurrentPattern2 = Pattern1B2();
            StartCoroutine(m_CurrentPattern2);
            
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern(1);
            yield return new WaitForMillisecondFrames(6000);
            m_MainTurret.StopPattern();
            m_Launchers[0].StopPattern();
            m_Launchers[1].StopPattern();
            m_SubTurrets[0].m_RotatePattern = 10;
            m_SubTurrets[1].m_RotatePattern = 10;
            m_FrontTurrets[0].m_RotatePattern = 10;
            m_FrontTurrets[1].m_RotatePattern = 10;
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(1000);
            m_SubTurrets[0].m_RotatePattern = 31;
            m_SubTurrets[1].m_RotatePattern = 32;
            yield return new WaitForMillisecondFrames(1000);
            for (int i = 0; i < m_SmallTurrets.Length; i++) {
                if (m_SmallTurrets[i] != null)
                    m_SmallTurrets[i].StartPattern(1);
            }
            yield return new WaitForMillisecondFrames(1000);

            m_SubTurrets[0].m_RotatePattern = 30;
            m_SubTurrets[1].m_RotatePattern = 30;
            m_SubTurrets[0].StartPattern(3);
            m_SubTurrets[1].StartPattern(3);
            m_CurrentPattern1 = Pattern1C1();
            StartCoroutine(m_CurrentPattern1);
            m_CurrentPattern2 = Pattern1C2();
            StartCoroutine(m_CurrentPattern2);

            yield return new WaitForMillisecondFrames(5000);
            m_SubTurrets[0].StopPattern();
            m_SubTurrets[1].StopPattern();
            m_SubTurrets[0].m_RotatePattern = 10;
            m_SubTurrets[1].m_RotatePattern = 10;
            StopAllPatterns();

            yield return new WaitForMillisecondFrames(2000);
            m_InPattern = true;
            m_CurrentPattern1 = Pattern1D();
            StartCoroutine(m_CurrentPattern1);
            m_CurrentPattern2 = Pattern1A2();
            StartCoroutine(m_CurrentPattern2);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);

            StopAllPatterns();
        }
        yield break;
    }

    private IEnumerator Pattern1A1() {
        int r = Random.Range(0, 2);
        int[] n = {6, 12, 12};
        for (int i = 0; i < n[m_SystemManager.GetDifficulty()]; i++) {
            m_SubTurrets[r].StartPattern(1);
            
            if (m_SystemManager.GetDifficulty() == 0) {
                yield return new WaitForMillisecondFrames(1000);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                yield return new WaitForMillisecondFrames(500);
            }
            else {
                yield return new WaitForMillisecondFrames(500);
            }
            r = 1-r;
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1A2() {
        int r = Random.Range(0, 2);
        yield return new WaitForMillisecondFrames(1000);

        while (true) {
            if (m_FrontTurrets[r] != null) {
                m_FrontTurrets[r].StartPattern(1);
            }
            else if (m_FrontTurrets[1-r] != null) {
                m_FrontTurrets[1-r].StartPattern(1);
            }
            else {
                yield break;
            }
            
            if (m_SystemManager.GetDifficulty() <= 1) {
                yield return new WaitForMillisecondFrames(3000);
            }
            else {
                yield return new WaitForMillisecondFrames(2000);
            }
            r = 1-r;
        }
    }

    private IEnumerator Pattern1B1() {
        int[] n = {2, 3, 4};

        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < m_SubTurrets.Length; j++) {
                if (m_SubTurrets[j] != null)
                    m_SubTurrets[j].StartPattern(2);
            }
            for (int j = 0; j < m_FrontTurrets.Length; j++) {
                if (m_FrontTurrets[j] != null)
                    m_FrontTurrets[j].StartPattern(2);
            }
            
            if (m_SystemManager.GetDifficulty() == 0) {
                yield return new WaitForMillisecondFrames(3000);
            }
            else {
                yield return new WaitForMillisecondFrames(2000);
            }
        }
    }

    private IEnumerator Pattern1B2() {
        int rand1 = Random.Range(0, 2), rand2;

        while (true) {
            m_Direction = Random.Range(0f, 360f);
            m_Launchers[rand1].StartPattern(1);
            rand2 = (2*Random.Range(0, 2) - 1); // -1 or 1
            for (int i = 0; i < 32; i++) {
                m_Direction += (20f + m_SystemManager.GetDifficulty()*5f) * rand2 / Application.targetFrameRate * Time.timeScale;
                yield return new WaitForMillisecondFrames(0);
            }
            rand2 = (2*Random.Range(0, 2) - 1); // -1 or 1
            for (int i = 0; i < 32; i++) {
                m_Direction -= (20f + m_SystemManager.GetDifficulty()*5f) * rand2 / Application.targetFrameRate * Time.timeScale;
                yield return new WaitForMillisecondFrames(0);
            }
            m_Launchers[rand1].StopPattern();
            rand1 = 1 - rand1;
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private IEnumerator Pattern1C1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(4.8f, 500);

        while (true) {
            int rand = Random.Range(0, 2);
            m_Launchers[rand].StartPattern(2);
            if (m_SystemManager.GetDifficulty() == 0) {
                yield return new WaitForMillisecondFrames(1800);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
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
                m_FrontTurrets[r].StartPattern(3);
            }
            else if (m_FrontTurrets[1-r] != null) {
                m_FrontTurrets[1-r].StartPattern(3);
            }
            else {
                yield break;
            }
            
            if (m_SystemManager.GetDifficulty() == 0) {
                yield return new WaitForMillisecondFrames(3000);
            }
            else {
                yield return new WaitForMillisecondFrames(2000);
            }
            r = 1-r;
        }
    }

    private IEnumerator Pattern1D() {
        int r = Random.Range(0, 2);
        int[] n = {6, 12, 12};
        for (int i = 0; i < n[m_SystemManager.GetDifficulty()]; i++) {
            m_SubTurrets[r].StartPattern(4);
            
            if (m_SystemManager.GetDifficulty() == 0) {
                yield return new WaitForMillisecondFrames(800);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                yield return new WaitForMillisecondFrames(400);
            }
            else {
                yield return new WaitForMillisecondFrames(320);
            }
            r = 1-r;
        }
        m_InPattern = false;
        yield break;
    }

    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        int rand1 = Random.Range(0, 2), rand2;
        bool first = true;
        yield return new WaitForMillisecondFrames(3000);
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            if (m_SmallTurrets[i] != null)
                m_SmallTurrets[i].StartPattern(1);
        }
        yield return new WaitForMillisecondFrames(1000);

        while (m_Phase == 2) {
            m_InPattern = true;
            m_Launchers[0].StartPattern(3, rand1);
            m_Launchers[1].StartPattern(3, rand1);
            m_Launchers[0].SetMoving(true);
            m_Launchers[1].SetMoving(true);
            m_CurrentPattern1 = Pattern2A();
            StartCoroutine(m_CurrentPattern1);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            rand1 = 1 - rand1;
            m_Launchers[0].StopPattern();
            m_Launchers[1].StopPattern();
            m_Launchers[0].SetMoving(false);
            m_Launchers[1].SetMoving(false);
            if (m_Health <= m_MaxHealth / 4) { // 체력 25% 이하
                ToNextPhase();
                break;
            }
            yield return new WaitForMillisecondFrames(1500);

            rand2 = Random.Range(0, 2);
            m_SubTurrets[0].m_RotatePattern = (byte) (41 + rand2);
            m_SubTurrets[1].m_RotatePattern = (byte) (41 + rand2);
            yield return new WaitForMillisecondFrames(1000);
            m_SubTurrets[0].StartPattern(5);
            m_SubTurrets[1].StartPattern(5);
            yield return new WaitForMillisecondFrames(1000);
            m_SubTurrets[0].m_RotatePattern = 10;
            m_SubTurrets[1].m_RotatePattern = 10;
            if (m_Health <= m_MaxHealth / 4) { // 체력 25% 이하
                ToNextPhase();
                break;
            }
            if (first)
                yield return new WaitForMillisecondFrames(2000);
            first = false;

            m_MainTurret.StartPattern(3);
            m_SubTurrets[0].StartPattern(6);
            m_SubTurrets[1].StartPattern(6);
            yield return new WaitForMillisecondFrames(7000);
            m_MainTurret.StopPattern();
            m_SubTurrets[0].StopPattern();
            m_SubTurrets[1].StopPattern();
            if (m_Health <= m_MaxHealth / 4) { // 체력 25% 이하
                ToNextPhase();
                break;
            }
            yield return new WaitForMillisecondFrames(2000);
        }
    }

    private IEnumerator Pattern2A() {
        Vector3[] pos = new Vector3[2];
        if (m_SystemManager.GetDifficulty() == 0) {
            for (int i = 0; i < 1; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(500);
            }
            yield return new WaitForMillisecondFrames(1000);
            for (int i = 0; i < 1; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(500);
            }
            yield return new WaitForMillisecondFrames(1000);
            for (int i = 0; i < 2; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(500);
            }
            yield return new WaitForMillisecondFrames(1000);
            for (int i = 0; i < 2; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(500);
            }
            yield return new WaitForMillisecondFrames(1000);
            for (int i = 0; i < 2; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(500);
            }
        }
        else if (m_SystemManager.GetDifficulty() == 1) {
            for (int i = 0; i < 1; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
            yield return new WaitForMillisecondFrames(900);
            for (int i = 0; i < 2; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
            yield return new WaitForMillisecondFrames(900);
            for (int i = 0; i < 2; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
            yield return new WaitForMillisecondFrames(900);
            for (int i = 0; i < 3; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
            yield return new WaitForMillisecondFrames(900);
            for (int i = 0; i < 3; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
        }
        else {
            for (int i = 0; i < 1; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
            yield return new WaitForMillisecondFrames(800);
            for (int i = 0; i < 2; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
            yield return new WaitForMillisecondFrames(800);
            for (int i = 0; i < 3; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
            yield return new WaitForMillisecondFrames(800);
            for (int i = 0; i < 3; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
            yield return new WaitForMillisecondFrames(800);
            for (int i = 0; i < 3; i++) {
                m_MainTurret.StartPattern(2);
                yield return new WaitForMillisecondFrames(400);
            }
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Phase3() { // 페이즈3 패턴 ============================
        int rand = Random.Range(0, 2);
        m_SubTurrets[0].m_RotatePattern = 20;
        m_SubTurrets[1].m_RotatePattern = 20;
        yield return new WaitForMillisecondFrames(2500);

        while (m_Phase == 3) {
            m_MainTurret.m_RotatePattern = (byte) (21 + rand);
            m_SubTurrets[rand].StartPattern(7);
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern(4);
            yield return new WaitForMillisecondFrames(1000);
            rand = 1 - rand;
            m_MainTurret.m_RotatePattern = (byte) (21 + rand);
            m_SubTurrets[rand].StartPattern(7);
            yield return new WaitForMillisecondFrames(2000);
            m_MainTurret.StartPattern(4);
            yield return new WaitForMillisecondFrames(1000);
            rand = 1 - rand;
        }
    }

    private void StopAllPatterns() {
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
    }


    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_Phase = -1;
        StopAllPatterns();
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        for (int i = 0; i < m_SmallTurrets.Length; i++) {
            if (m_SmallTurrets[i] != null)
                m_SmallTurrets[i].OnDeath();
        }
        m_SystemManager.BulletsToGems(2000);

        StartCoroutine(DeathExplosion1(3200));
        StartCoroutine(DeathExplosion2(3200));
        yield return new WaitForMillisecondFrames(800);
        ExplosionEffect(3, 2, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        yield return new WaitForMillisecondFrames(800);
        ExplosionEffect(3, 2, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        yield return new WaitForMillisecondFrames(800);
        ExplosionEffect(3, 2, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        ExplosionEffect(3, -1, new Vector3(Random.Range(-3.5f, 3.5f), 3f, Random.Range(-4.5f, 3.5f)));
        yield return new WaitForMillisecondFrames(800);
        
        ExplosionEffect(4, 3, new Vector3(0f, 2f, 0f)); // 최종 파괴
        ExplosionEffect(3, -1, new Vector3(-2f, 3f, -2f), new MoveVector(1.6f, -45f));
        ExplosionEffect(3, -1, new Vector3(2f, 3f, -2f), new MoveVector(1.6f, 45f));
        ExplosionEffect(3, -1, new Vector3(2f, 3f, 2f), new MoveVector(1.6f, 135f));
        ExplosionEffect(3, -1, new Vector3(-2f, 3f, 2f), new MoveVector(1.6f, -135f));
        ExplosionEffect(4, -1, new Vector3(-3.6f, 3f, -3.6f));
        ExplosionEffect(4, -1, new Vector3(3.6f, 3f, -3.6f));
        ExplosionEffect(4, -1, new Vector3(3.6f, 3f, 3.6f));
        ExplosionEffect(4, -1, new Vector3(-3.6f, 3f, 3.6f));
        ExplosionEffect(3, -1, new Vector3(-3f, 3f, 0f), new MoveVector(1.2f, -90f));
        ExplosionEffect(3, -1, new Vector3(0f, 3f, -3f), new MoveVector(1.2f, 0f));
        ExplosionEffect(3, -1, new Vector3(3f, 3f, 0f), new MoveVector(1.2f, 90f));
        ExplosionEffect(3, -1, new Vector3(0f, 3f, 3f), new MoveVector(1.2f, 180f));
        m_SystemManager.ScreenEffect(1);
        m_SystemManager.ShakeCamera(1f);
        
        CreateItems();
        BossDestroyed();
        yield break;
    }

    private IEnumerator NextPhaseExplosion1(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(250, 350);
            ExplosionEffect(0, 0, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)), new MoveVector(1.5f, Random.Range(0f, 360f)));
            ExplosionEffect(0, -1, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)), new MoveVector(1.5f, Random.Range(0f, 360f)));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator NextPhaseExplosion2(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(400, 600);
            ExplosionEffect(1, 4, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)), new MoveVector(1.5f, Random.Range(0f, 360f)));
            ExplosionEffect(1, -1, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)), new MoveVector(1.5f, Random.Range(0f, 360f)));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion1(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(250, 350);
            ExplosionEffect(1, 0, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(200, 600);
            ExplosionEffect(0, 1, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)), new MoveVector(2f, Random.Range(0f, 360f)));
            ExplosionEffect(0, -1, new Vector3(Random.Range(-4f, 4f), 3f, Random.Range(-5.4f, 4f)), new MoveVector(2f, Random.Range(0f, 360f)));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }
}
