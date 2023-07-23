using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss5 : EnemyUnit, IHasAppearance, IEnemyBossMain
{
    public GameObject m_Core;
    public GameObject m_AllWings;
    public GameObject m_WingsForAppearance;
    public EnemyBoss5_Wing[] m_EnemyBoss5Wings = new EnemyBoss5_Wing[3];
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;
    public Animator m_WingAnimator;
    
    private int _pattern1A_fireDelay1;
    private int _pattern1A_fireDelay2;

    private int m_Phase;
    private float m_Direction;
    private readonly Vector3 TARGET_POSITION = new (0f, -4f);
    private const int APPEARANCE_TIME = 10000;
    private float m_WingsAngle;
    private int m_MoveDirection;
    private float m_MoveSpeed, m_DefaultSpeed = 0.2f;
    private float m_TrackPos;
    private readonly MeshRenderer[] _wingMeshRenderers = new MeshRenderer[3];
    private readonly int _openedBoolAnimation = Animator.StringToHash("Opened");

    private IEnumerator m_CurrentPhase;
    private IEnumerator[] m_CurrentPattern = new IEnumerator[4];

    void Start()
    {
        m_WingsAngle = Random.Range(0f, 360f);
       
        for (int i = 0; i < _wingMeshRenderers.Length; i++) {
            _wingMeshRenderers[i] = m_EnemyBoss5Wings[i].GetComponentInChildren<MeshRenderer>();
            _wingMeshRenderers[i].gameObject.SetActive(false);
        }

        DisableInteractableAll();
        
        /*
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(TARGET_POSITION.y, APPEARANCE_TIME).SetEase(Ease.Linear));*/

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDeath;
    }

    public IEnumerator AppearanceSequence() {
        float init_position_y = transform.position.y;
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_posy = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            float position_y = Mathf.Lerp(init_position_y, TARGET_POSITION.y, t_posy);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    public void OnAppearanceComplete() {
        float random_direction = 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.05f, random_direction);
        m_MoveDirection = Random.Range(0, 2)*2 - 1;
        ToNextPhase();
        StartCoroutine(InitMaterial());

        EnableInteractableAll();
        
        SystemManager.OnBossStart();
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase == -1) {
            m_MoveVector.speed += 0.72f / Application.targetFrameRate * Time.timeScale;
        }
        else if (m_Phase > 0) {
            if (transform.position.x >= TARGET_POSITION.x + 0.7f) {
                m_MoveDirection = -1;
            }
            else if (transform.position.x <= TARGET_POSITION.x - 0.7f) {
                m_MoveDirection = 1;
            }
            else if (transform.position.y >= TARGET_POSITION.y + 0.2f) {
                m_MoveVector.direction = 0f;
            }
            else if (transform.position.y <= TARGET_POSITION.y - 0.2f) {
                m_MoveVector.direction = 180f;
            }

            if (m_MoveSpeed < m_DefaultSpeed && m_MoveDirection == 1) {
                m_MoveSpeed += 0.13f / Application.targetFrameRate * Time.timeScale;
            }
            else if (m_MoveSpeed > m_DefaultSpeed && m_MoveDirection == -1) {
                m_MoveSpeed -= 0.13f / Application.targetFrameRate * Time.timeScale;
            }
            else {
                m_MoveSpeed = m_DefaultSpeed*m_MoveDirection;
            }

            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x + m_MoveSpeed / Application.targetFrameRate * Time.timeScale, pos.y, Depth.ENEMY);
        }

        m_Direction += 91f / Application.targetFrameRate * Time.timeScale;
        if (m_Direction > 360f)
            m_Direction -= 360f;
        else if (m_Direction < 0f)
            m_Direction += 360f;
        
        RotateWings();
    }

    private void RotateWings() {
        m_WingsAngle += 30f / Application.targetFrameRate * Time.timeScale;
        if (m_Core != null)
            m_Core.transform.localRotation = Quaternion.Euler(0f, 0f, m_Core.transform.rotation.eulerAngles.z - 10f / Application.targetFrameRate * Time.timeScale);
        if (m_AllWings != null)
            m_AllWings.transform.localRotation = Quaternion.Euler(0f, 0f, m_WingsAngle);
        
        if (m_WingsAngle > 360f)
            m_WingsAngle -= 360f;
        else if (m_WingsAngle < 0f)
            m_WingsAngle += 360f;
    }

    private void SetWingOpenState(bool state) {
        m_WingAnimator.SetBool(_openedBoolAnimation, state);
        /*
        Quaternion[] init_localRotation = new Quaternion[m_EnemyBoss5Wings.Length];
        
        for (int i = 0; i < m_EnemyBoss5Wings.Length; ++i) {
            m_FirePositionsWing[i].localPosition = new Vector3(0f, 4.5f, -3.3f);
            init_localRotation[i] = m_EnemyBoss5Wings[i].transform.localRotation;
        }
        
        int frame = 1500 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_rot = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);

            try {
                for (int j = 0; j < m_EnemyBoss5Wings.Length; ++j) {
                    m_EnemyBoss5Wings[j].transform.localRotation = Quaternion.Lerp(init_localRotation[j], Quaternion.Euler(30f, 0f, 0f), t_rot);
                }
            }
            catch {
                break;
            }
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;*/
    }

    private IEnumerator InitMaterial() {
        yield return new WaitForMillisecondFrames(400);
        for (int i = 0; i < _wingMeshRenderers.Length; i++) {
            _wingMeshRenderers[i].material.SetColor("_EmissionColor", Color.white);
        }
        m_WingsForAppearance.SetActive(false);
        for (int i = 0; i < _wingMeshRenderers.Length; i++) {
            _wingMeshRenderers[i].gameObject.SetActive(true);
        }
        yield break;
    }

    public void ToNextPhase() {
        m_Phase++;
        StopAllPatterns();

        if (m_Phase == 1) {
            m_Phase = 1;
            m_CurrentPhase = Phase1();
            StartCoroutine(m_CurrentPhase);
        }
        else if (m_Phase == 2) {
            NextPhaseExplosion();
            BulletManager.SetBulletFreeState(2000);
            SetWingOpenState(true);

            if (m_CurrentPhase != null)
                StopCoroutine(m_CurrentPhase);
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
        }
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1500);
        
        var difficulty = (int)SystemManager.Difficulty;
        int[,] fireDelay1 = {{ 600, 360, 250 }, { 2400, 2000, 2000 }};
        int[,] fireDelay2 = {{ 2400, 2000, 2000 }, { 600, 400, 250 }};
        var initDelay1 = fireDelay1[0, difficulty]; // start value
        var targetDelay1 = fireDelay1[1, difficulty]; // target value
        var initDelay2 = fireDelay2[0, difficulty]; // start value
        var targetDelay2 = fireDelay2[1, difficulty]; // target value
        
        StartCoroutine(Pattern1_A0(11000, initDelay1, targetDelay1, initDelay2, targetDelay2));
        StartPattern("1A1", new BulletPattern_EnemyBoss5_1A1(this, () => _pattern1A_fireDelay1, targetDelay1));
        StartPattern("1A2", new BulletPattern_EnemyBoss5_1A2(this, () => _pattern1A_fireDelay2, targetDelay2));
        yield return new WaitForMillisecondFrames(15000);

        while (m_Phase == 1)
        {
            StartPattern("1B1", new BulletPattern_EnemyBoss5_1B1(this));
            StartPattern("1B2", new BulletPattern_EnemyBoss5_1B2(this, GetAngleToTarget));
            yield return new WaitForMillisecondFrames(10000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(2500);

            StartPattern("1C1", new BulletPattern_EnemyBoss5_1C1(this));
            yield return new WaitForMillisecondFrames(3000);
            StartPattern("1C2", new BulletPattern_EnemyBoss5_1C2(this));
            yield return new WaitForMillisecondFrames(7000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            var initDir1 = GetAngleToTarget(m_FirePosition[1].position, PlayerManager.GetPlayerPosition());
            StartPattern("1D1", new BulletPattern_EnemyBoss5_1D1(this, 0, 1400, initDir1, 2f));
            var initDir2 = GetAngleToTarget(m_FirePosition[2].position, PlayerManager.GetPlayerPosition());
            StartPattern("1D1", new BulletPattern_EnemyBoss5_1D1(this, 0, 1100, initDir2, -1f));
            var initDir3 = GetAngleToTarget(m_FirePosition[3].position, PlayerManager.GetPlayerPosition());
            StartPattern("1D1", new BulletPattern_EnemyBoss5_1D1(this, 0, 800, initDir3, -3f));
            yield return new WaitForMillisecondFrames(3000);
            StartPattern("1D2", new BulletPattern_EnemyBoss5_1D2(this));
            yield return new WaitForMillisecondFrames(11000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
        }
    }

    private IEnumerator Pattern1_A0(int duration, float initDelay1, float targetDelay1, float initDelay2, float targetDelay2)
    {
        var frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            var t_delay = AC_Ease.ac_ease[(int)EaseType.InOutQuad].Evaluate((float) (i+1) / frame);

            var fire_delay_1 = Mathf.Lerp(initDelay1, targetDelay1, t_delay);
            var fire_delay_2 = Mathf.Lerp(initDelay2, targetDelay2, t_delay);
            _pattern1A_fireDelay1 = (int) fire_delay_1;
            _pattern1A_fireDelay2 = (int) fire_delay_2;
            yield return new WaitForMillisecondFrames(0);
        }
    }

    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(3000);
        m_CurrentPattern[0] = Pattern2_A1();
        StartCoroutine(m_CurrentPattern[0]);
        m_CurrentPattern[1] = Pattern2_A2();
        StartCoroutine(m_CurrentPattern[1]);
        yield return new WaitForMillisecondFrames(8000);
        StopAllPatterns();
        yield return new WaitForMillisecondFrames(3000);

        while (m_Phase == 2) {
            m_CurrentPattern[0] = Pattern2_B1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern2_B2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForMillisecondFrames(15000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
            
            m_CurrentPattern[0] = Pattern2_C();
            StartCoroutine(m_CurrentPattern[0]);
            yield return new WaitForMillisecondFrames(8000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
        }
    }

    private IEnumerator Pattern2_A1() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        int[] period = { 900, 350, 250 };
        int number = Random.Range(0, 2);

        while (true) {
            for (int i = 0; i < 3; i++) {
                //pos = m_FirePositionsWing[i].position;
                pos = Vector3.zero;
                if (i == 0) {
                    if (160f <= m_WingsAngle && m_WingsAngle < 200f) {
                        continue;
                    }
                }
                else if (i == 1) {
                    if (40f <= m_WingsAngle && m_WingsAngle < 80f) {
                        continue;
                    }
                }
                else {
                    if (280f <= m_WingsAngle && m_WingsAngle < 320f) {
                        continue;
                    }
                }

                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    CreateBulletsSector(5, pos, 5.5f, Random.Range(-5f, 5f), accel, 4 - number, 25f);
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    CreateBulletsSector(5, pos, 5.7f, Random.Range(-5f, 5f), accel, 5 - number, 15f);
                }
                else {
                    CreateBulletsSector(5, pos, 6f, Random.Range(-5f, 5f), accel, 5 - number, 15f);
                }
                number = 1 - number;
            }
            yield return new WaitForMillisecondFrames(period[(int) SystemManager.Difficulty]);
        }
    }

    private IEnumerator Pattern2_A2() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        float interval = Random.Range(0f, 260f);

        while (true) {
            //pos = m_FirePosition.position;
            pos = Vector3.zero;
            CreateBulletsSector(4, pos, 8f, -180f, accel, 2, interval);
            interval += 6.1f;
            if (interval > 260f) {
                interval -= 260f;
            }
            yield return new WaitForMillisecondFrames(40);
        }
    }

    private IEnumerator Pattern2_B1() {
        Vector3 pos;
        BulletAccel accel1 = new BulletAccel(5f, 800);
        BulletAccel accel2 = new BulletAccel(6f, 800);
        BulletAccel accel3 = new BulletAccel(7f, 800);
        int[] period = { 320, 150, 100 };

        while (true) {
            for (int i = 0; i < 3; i++) {
                //pos = m_FirePositionsWing[i].position;
                pos = Vector3.zero;
                if (i == 0) {
                    if (160f <= m_WingsAngle && m_WingsAngle < 200f) {
                        continue;
                    }
                }
                else if (i == 1) {
                    if (40f <= m_WingsAngle && m_WingsAngle < 80f) {
                        continue;
                    }
                }
                else {
                    if (280f <= m_WingsAngle && m_WingsAngle < 320f) {
                        continue;
                    }
                }

                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    CreateBullet(4, pos, 2f, Random.Range(-18f, 18f), accel1);
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    CreateBullet(4, pos, 2.5f, Random.Range(-18f, 18f), accel2);
                }
                else {
                    CreateBullet(4, pos, 3f, Random.Range(-18f, 18f), accel1);
                }
            }
            yield return new WaitForMillisecondFrames(period[(int) SystemManager.Difficulty]);
        }
    }

    private IEnumerator Pattern2_B2() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        float interval = 350f, dir, timer = 0f;
        float[] min_interval = { 45f, 35f, 30f };
        int[] number = { 52, 54, 55 };
        int rand;

        while (interval > min_interval[(int) SystemManager.Difficulty]) {
            //pos = m_FirePosition.position;
            pos = Vector3.zero;
            CreateBulletsSector(1, pos, 8f, 0f, accel, 2, interval);
            CreateBulletsSector(0, pos, 8f, 0f, accel, 2, interval + 14f);
            CreateBulletsSector(1, pos, 8f, 0f, accel, 2, interval + 21f);
            interval -= 21.1f;
            yield return new WaitForMillisecondFrames(80);
        }
        interval = min_interval[(int) SystemManager.Difficulty];
        rand = 2*Random.Range(0, 2) - 1;
        dir = 0f;
        while (true) {
            //pos = m_FirePosition.position;
            pos = Vector3.zero;
            CreateBulletsSector(1, pos, 8f, dir, accel, 2, interval);
            CreateBulletsSector(0, pos, 8f, dir, accel, 2, interval + 14f);
            CreateBulletsSector(1, pos, 8f, dir, accel, 2, interval + 21f);
            if (timer > 0.8f) {
                for (int i = 0; i < number[(int) SystemManager.Difficulty]; i++)
                    CreateBulletsSector(0, pos, 8f, dir + 180f, accel, 2, 3f + i*6f);
                timer -= 0.8f;
            }
            timer += 0.08f;
            dir += 1.5f*rand;
            if (Mathf.Abs(dir) > 40f) {
                rand *= -1;
            }
            yield return new WaitForMillisecondFrames(80);
        }
    }

    private IEnumerator Pattern2_C() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        int[] period = { 640, 400, 300 };
        float[] dir = { Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f) };

        while (true) {
            for (int i = 0; i < 3; i++) {
                //pos = m_FirePositionsWing[i].position;
                pos = Vector3.zero;
                if (i == 0) {
                    if (160f <= m_WingsAngle && m_WingsAngle < 200f) {
                        continue;
                    }
                }
                else if (i == 1) {
                    if (40f <= m_WingsAngle && m_WingsAngle < 80f) {
                        continue;
                    }
                }
                else {
                    if (280f <= m_WingsAngle && m_WingsAngle < 320f) {
                        continue;
                    }
                }

                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    CreateBulletsSector(1, pos, 5.2f, dir[i] + m_Direction, accel, 15, 24f);
                }
                else {
                    CreateBulletsSector(1, pos, 5.2f, dir[i] + m_Direction, accel, 20, 18f);
                }
            }
            yield return new WaitForMillisecondFrames(period[(int) SystemManager.Difficulty]);
        }
    }

    private void StopAllPatterns() {
        for (int i = 0; i < m_CurrentPattern.Length; i++) {
            if (m_CurrentPattern[i] != null) {
                StopCoroutine(m_CurrentPattern[i]);
            }
        }
    }


    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        StopAllPatterns();
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        for (int i = 0; i < m_EnemyBoss5Wings.Length; i++) {
            m_EnemyBoss5Wings[i].m_EnemyDeath.OnDeath();
        }
        Destroy(m_AllWings);
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0f, 0f);
        
        if (SystemManager.Difficulty < GameDifficulty.Hell) {
            InGameDataManager.Instance.SaveElapsedTime();
        }
        
        yield break;
    }

    public void OnBossDying() {
        SystemManager.OnBossClear();
    }

    public void OnBossDeath() {
        SystemManager.Instance.StartStageClearCoroutine();
        InGameScreenEffectService.WhiteEffect(true);
        MainCamera.ShakeCamera(1.5f);
        
        if (SystemManager.Difficulty == GameDifficulty.Hell)
        {
            Vector3 bossPos = transform.position;
            bossPos.z = Depth.ENEMY;
            StageManager.Action_OnTrueBossStart?.Invoke(bossPos);
        }
    }
}
