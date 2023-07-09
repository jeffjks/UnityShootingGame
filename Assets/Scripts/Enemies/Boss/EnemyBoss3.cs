using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss3 : EnemyUnit, IHasAppearance, IEnemyBossMain
{
    public EnemyBoss3Turret[] m_Turret = new EnemyBoss3Turret[2];
    public EnemyBoss3Part m_Part;
    public EnemyBoss3Barrel[] m_EnemyBoss3Barrel = new EnemyBoss3Barrel[2];
    public GameObject[] m_PartOnDeath = new GameObject[2];
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int m_Phase;
    
    private readonly Vector3 TARGET_POSITION = new (0f, -4.2f, Depth.ENEMY);
    private Vector3 m_DefaultScale;
    private const int APPEARANCE_TIME = 1200;
    private const float MAX_ROTATION = 11f;
    private byte m_DirectionState;
    private float m_Direction;
    private int m_RotateDirection;
    private bool m_InPattern = false;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2, m_CurrentPattern3;

    void Start()
    {
        m_DefaultScale = transform.localScale;
        transform.localScale = new Vector3(2f, 2f, 2f);
        m_MoveVector = new MoveVector(1f, -125f);
        RotateImmediately(m_MoveVector.direction);

        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDeath;
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.m_HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase > 0) {
            if (transform.position.x > TARGET_POSITION.x + 0.7f) {
                m_MoveVector.direction = Random.Range(-105f, -75f);
            }
            if (transform.position.x < TARGET_POSITION.x - 0.7f) {
                m_MoveVector.direction = Random.Range(75f, 105f);
            }
            if (transform.position.y > TARGET_POSITION.y + 0.2f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
            if (transform.position.y < TARGET_POSITION.y - 0.2f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
        }

        switch(m_DirectionState) {
            case 1:
                m_Direction += 90f * m_RotateDirection / Application.targetFrameRate * Time.timeScale;
                break;
            case 2:
                m_Direction += 71f * m_RotateDirection / Application.targetFrameRate * Time.timeScale;
                break;
            case 3:
                m_Direction += 19f * m_RotateDirection / Application.targetFrameRate * Time.timeScale;
                break;
            case 4:
                m_Direction += MAX_ROTATION * m_RotateDirection / Application.targetFrameRate * Time.timeScale;
                break;
        }

        m_Part.CurrentAngle = CurrentAngle;
        
        if (m_Direction > 360f)
            m_Direction -= 360f;
        else if (m_Direction < 0f)
            m_Direction += 360f;
    }

    public IEnumerator AppearanceSequence() {
        int frame;

        float init_speed = m_MoveVector.speed;
        frame = 2000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 15f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }

        yield return new WaitForMillisecondFrames(1000);

        transform.localScale = m_DefaultScale;
        transform.position = new Vector3(0f, 4.3f, Depth.ENEMY);
        m_MoveVector = new MoveVector(0f, 0f);
        RotateImmediately(m_MoveVector.direction);

        float init_position_y = transform.position.y;
        frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_posy = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            float position_y = Mathf.Lerp(init_position_y, -4.5f, t_posy);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    public void OnAppearanceComplete() {
        float random_direction = Random.Range(75f, 105f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.5f, random_direction);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        EnableInteractableAll();
        
        SystemManager.OnBossStart();
    }

    private int RandomValue() {
        int random_value = Random.Range(0, 2);
        if (random_value == 0)
            random_value = -1;
        return random_value;
    }

    public void ToNextPhase() {
        int duration = 2000;
        m_Phase++;
        if (m_Phase >= 2) {
            BulletManager.SetBulletFreeState(1000);
            
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            if (m_CurrentPattern3 != null)
                StopCoroutine(m_CurrentPattern3);
            if (m_CurrentPhase != null)
                StopCoroutine(m_CurrentPhase);
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();

            if (m_Phase == 2) {
                m_CurrentPhase = Phase2();
                StartCoroutine(m_CurrentPhase);
                m_Part.m_EnemyDeath.OnDying();
                m_EnemyHealth.SetInvincibility(duration);
                NextPhaseExplosion();
            }
        }
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private IEnumerator Phase1() { // 페이즈 1 패턴 =================
        int side;
        yield return new WaitForMillisecondFrames(1000);
        while (m_Phase == 1) {
            m_CurrentPattern1 = Pattern1A();
            StartCoroutine(m_CurrentPattern1);
            while (m_InPattern)
                yield return new WaitForFrames(0);
            yield return new WaitForMillisecondFrames(500);
            
            side = RandomValue();
            
            m_Turret[0].StartPattern(1, side);
            m_Turret[1].StartPattern(1, side);
            m_CurrentPattern1 = Pattern1B();
            StartCoroutine(m_CurrentPattern1);
            while (m_InPattern)
                yield return new WaitForFrames(0);
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            yield return new WaitForMillisecondFrames(2000);

            m_CurrentPattern1 = Pattern1C1();
            m_CurrentPattern2 = Pattern1C2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(5000);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForMillisecondFrames(700);
            m_CurrentPattern1 = Pattern1D();
            StartCoroutine(m_CurrentPattern1);
            yield return new WaitForMillisecondFrames(2500);
        }
        yield break;
    }

    private IEnumerator Phase2() { // 페이즈 2 패턴 =================
        int random_value;
        yield return new WaitForMillisecondFrames(4000);

        while (m_Phase == 2) {
            m_CurrentPattern1 = Pattern2A();
            StartCoroutine(m_CurrentPattern1);
            yield return new WaitForMillisecondFrames(5000);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            yield return new WaitForMillisecondFrames(5000);

            m_CurrentPattern1 = Pattern2B1();
            StartCoroutine(m_CurrentPattern1);
            
            for (int i = 0; i < 1; ++i) { // Repeat Once
                random_value = Random.Range(0, 2);
                
                m_Turret[0].StartPattern(2, random_value);
                m_Turret[1].StartPattern(2, 1 - random_value);

                yield return new WaitForMillisecondFrames(4000);
                m_Turret[0].StopPattern();
                m_Turret[1].StopPattern();
                m_CurrentPattern2 = Pattern2B2();
                StartCoroutine(m_CurrentPattern2);
                yield return new WaitForMillisecondFrames(2000);
                if (m_CurrentPattern2 != null)
                    StopCoroutine(m_CurrentPattern2);
                yield return new WaitForMillisecondFrames(1000);
            }
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            yield return new WaitForMillisecondFrames(1200);
        }
        yield break;
    }

    private IEnumerator Pattern1A() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        float[] fire_delay = { 0.18f, 0.13f, 0.1f };

        m_RotateDirection = RandomValue();
        
        m_InPattern = true;
        m_DirectionState = 1;

        for (int i = 0; i < 3; i++) {
            pos = m_FirePosition[0].position;
            m_Direction = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition()) - 45f*m_RotateDirection;
            m_CurrentPattern2 = Pattern1A1(i);
            m_CurrentPattern3 = Pattern1A2(i);
            StartCoroutine(m_CurrentPattern2);
            StartCoroutine(m_CurrentPattern3);
            yield return new WaitForMillisecondFrames(1000);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            if (m_CurrentPattern3 != null)
                StopCoroutine(m_CurrentPattern3);
            m_RotateDirection *= -1;
        }
        
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1A1(int level) {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        int duration = 0;
        int[] fire_delay = { 180, 130, 100 };

        while (duration < 1000) {
            pos = m_FirePosition[0].position;
            duration += fire_delay[(int) SystemManager.Difficulty];
            switch(level) {
                case 0:
                    CreateBullet(1, pos, 7.4f, m_Direction, accel);
                    break;
                case 1:
                    CreateBulletsSector(1, pos, 7.4f, m_Direction, accel, 2, 24f);
                    break;
                case 2:
                    CreateBulletsSector(1, pos, 7.4f, m_Direction, accel, 3, 24f);
                    break;
                default:
                    break;
            }
            yield return new WaitForMillisecondFrames(fire_delay[(int) SystemManager.Difficulty]);
        }
        yield break;
    }

    private IEnumerator Pattern1A2(int level) {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        int duration = 0;
        int[] fire_delay = { 300, 220, 170 };

        while (duration < 1000) {
            pos = m_FirePosition[0].position;
            duration += fire_delay[(int) SystemManager.Difficulty];
            switch(level) {
                case 0:
                    CreateBulletsSector(3, pos, 6f, m_Direction, accel, 3, 2.25f);
                    break;
                case 1:
                    CreateBulletsSector(3, pos, 6f, m_Direction - 14f, accel, 3, 2.25f);
                    CreateBulletsSector(3, pos, 6f, m_Direction + 14f, accel, 3, 2.25f);
                    break;
                case 2:
                    CreateBulletsSector(3, pos, 6f, m_Direction - 28f, accel, 3, 2.25f);
                    CreateBulletsSector(3, pos, 6f, m_Direction, accel, 3, 2.25f);
                    CreateBulletsSector(3, pos, 6f, m_Direction + 28f, accel, 3, 2.25f);
                    break;
                default:
                    break;
            }
            yield return new WaitForMillisecondFrames(fire_delay[(int) SystemManager.Difficulty]);
        }
        yield break;
    }

    private IEnumerator Pattern1B() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        int duration;
        int[] fire_delay = { 1000, 500, 330 };

        m_InPattern = true;
        yield return new WaitForMillisecondFrames(1500);

        for (int j = 0; j < 4; j++) {
            duration = 0;
            while (duration < 1600) {
                duration += fire_delay[(int) SystemManager.Difficulty];
                pos = m_FirePosition[0].position;
                for (int i = 0; i <= j; i++) {
                    CreateBullet(3, pos, 7f + i*0.6f, GetAngleToTarget(pos, PlayerManager.GetPlayerPosition()), accel);
                    if (SystemManager.Difficulty == GameDifficulty.Normal) {
                        break;
                    }
                }
                yield return new WaitForMillisecondFrames(fire_delay[(int) SystemManager.Difficulty]);
            }
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1C1() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        int[] fire_delay = { 210, 120, 70 };
        int duration = 0;
        
        m_Direction = Random.Range(0f, 360f);
        m_DirectionState = 2;
        m_RotateDirection = RandomValue();

        while (true) {
            duration += fire_delay[(int) SystemManager.Difficulty];
            pos = m_FirePosition[0].position;
            CreateBulletsSector(2, pos, 7.4f, m_Direction, accel, 8, 45f);
            CreateBulletsSector(5, pos, 7.4f, -m_Direction, accel, 8, 45f);
            yield return new WaitForMillisecondFrames(fire_delay[(int) SystemManager.Difficulty]);
        }
    }

    private IEnumerator Pattern1C2() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        
        m_RotateDirection = RandomValue();
        yield return new WaitForMillisecondFrames(1000);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while (true) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForMillisecondFrames(2400);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while (true) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7f, Random.Range(0f, 360f), accel, 30, 12f);
                yield return new WaitForMillisecondFrames(220);
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7f, Random.Range(0f, 360f), accel, 36, 10f);
                yield return new WaitForMillisecondFrames(2000);
            }
        }
        else {
            while (true) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7f, Random.Range(0f, 360f), accel, 30, 12f);
                yield return new WaitForMillisecondFrames(220);
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7f, Random.Range(0f, 360f), accel, 36, 10f);
                yield return new WaitForMillisecondFrames(220);
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7f, Random.Range(0f, 360f), accel, 30, 12f);
                yield return new WaitForMillisecondFrames(1500);
            }
        }
    }

    private IEnumerator Pattern1D() {
        Vector3 pos1, pos2;
        BulletAccel accel = new BulletAccel(0f, 0);
        BulletAccel new_accel = new BulletAccel(7.3f, 1500);
        
        m_RotateDirection = RandomValue();
        
        pos1 = m_FirePosition[1].position;
        pos2 = m_FirePosition[2].position;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletSpawnType.Create, Random.Range(0, 100),
            1, 0.1f, BulletPivot.Current, 0f, new_accel, 2, 180f, new Vector2Int(200, 300));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletSpawnType.Create, Random.Range(0, 100),
            1, 0.1f, BulletPivot.Current, 0f, new_accel, 2, 180f, new Vector2Int(200, 300));
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletSpawnType.Create, Random.Range(0, 100),
            1, 0.1f, BulletPivot.Current, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletSpawnType.Create, Random.Range(0, 100),
            1, 0.1f, BulletPivot.Current, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
        }
        else {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletSpawnType.Create, Random.Range(0, 100),
            1, 0.1f, BulletPivot.Current, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletSpawnType.Create, Random.Range(0, 100),
            1, 0.1f, BulletPivot.Current, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
            CreateBullet(3, pos1, 10f, 40f, accel, BulletSpawnType.Create, Random.Range(0, 100),
            1, 0.1f, BulletPivot.Current, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
            CreateBullet(3, pos2, 10f, -40f, accel, BulletSpawnType.Create, Random.Range(0, 100),
            1, 0.1f, BulletPivot.Current, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
        }
        
        StartCoroutine(m_EnemyBoss3Barrel[0].ShootAnimation());
        StartCoroutine(m_EnemyBoss3Barrel[1].ShootAnimation());
        yield break;
    }

    private IEnumerator Pattern2A() {
        Vector3 pos;
        BulletAccel accel1 = new BulletAccel(0.5f, 1200);
        BulletAccel accel2 = new BulletAccel(0f, 0);
        int[] fire_delay = { 6, 2, 1 };

        m_InPattern = true;
        //m_DirectionState = 3;

        while (true) {
            pos = m_FirePosition[0].position;
            CreateBullet(4, pos, Random.Range(8f, 9f), Random.Range(0f, 360f), accel1, BulletSpawnType.EraseAndCreate, 1200,
            0, 3.2f, BulletPivot.Fixed, Random.Range(0f, 360f), accel2, 2, Random.Range(0f, 360f)); // PL
            CreateBullet(4, pos, Random.Range(8f, 9f), Random.Range(0f, 360f), accel1, BulletSpawnType.EraseAndCreate, 1200,
            0, 3.2f, BulletPivot.Fixed, Random.Range(0f, 360f), accel2, 2, Random.Range(0f, 360f)); // PL
            CreateBullet(4, pos, Random.Range(9f, 10f), Random.Range(0f, 360f), accel1, BulletSpawnType.EraseAndCreate, 1200,
            2, 2.1f, BulletPivot.Fixed, Random.Range(0f, 360f), accel2, 2, Random.Range(0f, 360f)); // PS
            CreateBullet(4, pos, Random.Range(9f, 10f), Random.Range(0f, 360f), accel1, BulletSpawnType.EraseAndCreate, 1200,
            2, 2.1f, BulletPivot.Fixed, Random.Range(0f, 360f), accel2, 2, Random.Range(0f, 360f)); // PS
            yield return new WaitForFrames(fire_delay[(int) SystemManager.Difficulty]);
        }
    }

    private IEnumerator Pattern2B1() {
        Vector3 pos1, pos2;
        BulletAccel accel = new BulletAccel(0f, 0);
        int total_duration = 1400, duration, fire_delay = 80;
        
        m_DirectionState = 4;
        m_RotateDirection = RandomValue();
        m_Direction = - m_RotateDirection * MAX_ROTATION * 0.7f;

        while (true) {
            for (int i = 0; i < 3; i++) {
                duration = 0;
                while (duration < total_duration) {
                    pos1 = m_FirePosition[1].position;
                    pos2 = m_FirePosition[2].position;

                    if (SystemManager.Difficulty == GameDifficulty.Normal) {
                        CreateBulletsSector(0, pos1, 8f, m_Direction - 20f, accel, 4, 50f);
                        CreateBulletsSector(0, pos2, 8f, m_Direction + 20f, accel, 4, 50f);
                    }
                    else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                        CreateBulletsSector(0, pos1, 8.3f, m_Direction, accel, 10, 24f);
                        CreateBulletsSector(0, pos2, 8.3f, m_Direction, accel, 10, 24f);
                    }
                    else {
                        CreateBulletsSector(0, pos1, 8.3f, m_Direction, accel, 12, 20f);
                        CreateBulletsSector(0, pos2, 8.3f, m_Direction, accel, 12, 20f);
                    }
                    duration += fire_delay;
                    yield return new WaitForMillisecondFrames(fire_delay);
                }
                m_RotateDirection *= -1;
            }
            yield return new WaitForMillisecondFrames(600);
        }
    }

    private IEnumerator Pattern2B2() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(4.2f, 900);
        int[] fire_delay = { 100, 64, 50 };
        float target_angle;
        
        while (true) {
            pos = m_FirePosition[0].position;
            target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
            CreateBulletsSector(4, pos, 8f, target_angle + Random.Range(-40f, 40f), accel, 2, 8f);
            yield return new WaitForMillisecondFrames(fire_delay[(int) SystemManager.Difficulty]);
        }
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
        if (m_CurrentPattern3 != null)
            StopCoroutine(m_CurrentPattern3);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        for (int i = 0; i < m_Turret.Length; i++) {
            if (m_Turret[i] != null)
                m_Turret[i].m_EnemyDeath.OnDying();
        }
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(1f, 0f);
        
        yield return new WaitForMillisecondFrames(1600);
        m_PartOnDeath[0].SetActive(false);
        yield return new WaitForMillisecondFrames(800);
        m_PartOnDeath[1].SetActive(false);
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
