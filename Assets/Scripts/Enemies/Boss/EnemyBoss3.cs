using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss3 : EnemyUnit, IHasAppearance, IEnemyBossMain
{
    public EnemyBoss3Turret[] m_Turret = new EnemyBoss3Turret[2];
    public EnemyBoss3Part m_Part;
    public Transform[] m_FirePosition = new Transform[3];
    public EnemyBoss3Barrel[] m_EnemyBoss3Barrel = new EnemyBoss3Barrel[2];
    public GameObject[] m_PartOnDeath = new GameObject[2];

    private int m_Phase;
    
    private Vector3 m_TargetPosition;
    private Vector3 m_DefaultScale;
    private const int APPEARANCE_TIME = 1200;
    private byte m_DirectionState;
    private float m_Direction;
    private int m_RotateDirection;
    private bool m_InPattern = false;
    private float m_MaxRotation = 11f;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2, m_CurrentPattern3;

    void Start()
    {
        m_TargetPosition = new Vector2(0f, -4.2f);
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
            if (transform.position.x > m_TargetPosition.x + 0.7f) {
                m_MoveVector.direction = Random.Range(-105f, -75f);
            }
            if (transform.position.x < m_TargetPosition.x - 0.7f) {
                m_MoveVector.direction = Random.Range(75f, 105f);
            }
            if (transform.position.y > m_TargetPosition.y + 0.2f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
            if (transform.position.y < m_TargetPosition.y - 0.2f) {
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
                m_Direction += m_MaxRotation * m_RotateDirection / Application.targetFrameRate * Time.timeScale;
                break;
        }

        m_Part.m_CurrentAngle = m_CurrentAngle;
        
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
        yield break;
    }

    public void OnAppearanceComplete() {
        float random_direction = Random.Range(75f, 105f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.5f, random_direction);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        EnableInteractableAll();
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
            m_SystemManager.EraseBullets(1000);
            
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
                m_EnemyHealth.DisableInvincibility(duration);
                NextPhaseExplosion(duration);
            }
        }
    }

    private void NextPhaseExplosion(int duration) {
        StartCoroutine(NextPhaseExplosionEffect1(duration));
        StartCoroutine(NextPhaseExplosionEffect2(duration));
    }

    private IEnumerator NextPhaseExplosionEffect1(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(350, 500);
            m_SystemManager.ShakeCamera(0.3f);
            ExplosionEffect(1, 1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator NextPhaseExplosionEffect2(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(200, 400);
            ExplosionEffect(2, 2, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator Phase1() { // 페이즈 1 패턴 =================
        int side;
        yield return new WaitForMillisecondFrames(1000);
        while(m_Phase == 1) {
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] fire_delay = { 0.18f, 0.13f, 0.1f };

        m_RotateDirection = RandomValue();
        
        m_InPattern = true;
        m_DirectionState = 1;

        for (int i = 0; i < 3; i++) {
            pos = m_FirePosition[0].position;
            m_Direction = GetAngleToTarget(pos, m_PlayerManager.GetPlayerPosition()) - 45f*m_RotateDirection;
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int duration = 0;
        int[] fire_delay = { 180, 130, 100 };

        while (duration < 1000) {
            pos = m_FirePosition[0].position;
            duration += fire_delay[m_SystemManager.GetDifficulty()];
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
            yield return new WaitForMillisecondFrames(fire_delay[m_SystemManager.GetDifficulty()]);
        }
        yield break;
    }

    private IEnumerator Pattern1A2(int level) {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int duration = 0;
        int[] fire_delay = { 300, 220, 170 };

        while (duration < 1000) {
            pos = m_FirePosition[0].position;
            duration += fire_delay[m_SystemManager.GetDifficulty()];
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
            yield return new WaitForMillisecondFrames(fire_delay[m_SystemManager.GetDifficulty()]);
        }
        yield break;
    }

    private IEnumerator Pattern1B() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int duration;
        int[] fire_delay = { 1000, 500, 330 };

        m_InPattern = true;
        yield return new WaitForMillisecondFrames(1500);

        for (int j = 0; j < 4; j++) {
            duration = 0;
            while (duration < 1600) {
                duration += fire_delay[m_SystemManager.GetDifficulty()];
                pos = m_FirePosition[0].position;
                for (int i = 0; i <= j; i++) {
                    CreateBullet(3, pos, 7f + i*0.6f, GetAngleToTarget(pos, m_PlayerPosition), accel);
                    if (m_SystemManager.GetDifficulty() == 0) {
                        break;
                    }
                }
                yield return new WaitForMillisecondFrames(fire_delay[m_SystemManager.GetDifficulty()]);
            }
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1C1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int[] fire_delay = { 210, 120, 70 };
        int duration = 0;
        
        m_Direction = Random.Range(0f, 360f);
        m_DirectionState = 2;
        m_RotateDirection = RandomValue();

        while (true) {
            duration += fire_delay[m_SystemManager.GetDifficulty()];
            pos = m_FirePosition[0].position;
            CreateBulletsSector(2, pos, 7.4f, m_Direction, accel, 8, 45f);
            CreateBulletsSector(5, pos, 7.4f, -m_Direction, accel, 8, 45f);
            yield return new WaitForMillisecondFrames(fire_delay[m_SystemManager.GetDifficulty()]);
        }
    }

    private IEnumerator Pattern1C2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        
        m_RotateDirection = RandomValue();
        yield return new WaitForMillisecondFrames(1000);
        
        if (m_SystemManager.GetDifficulty() == 0) {
            while (true) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForMillisecondFrames(2400);
            }
        }
        else if (m_SystemManager.GetDifficulty() == 1) {
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        EnemyBulletAccel new_accel = new EnemyBulletAccel(7.3f, 1500);
        
        m_RotateDirection = RandomValue();
        
        pos1 = m_FirePosition[1].position;
        pos2 = m_FirePosition[2].position;

        if (m_SystemManager.GetDifficulty() == 0) {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletType.CREATE, Random.Range(0, 100),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2Int(200, 300));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletType.CREATE, Random.Range(0, 100),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2Int(200, 300));
        }
        else if (m_SystemManager.GetDifficulty() == 1) {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletType.CREATE, Random.Range(0, 100),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletType.CREATE, Random.Range(0, 100),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
        }
        else {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletType.CREATE, Random.Range(0, 100),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletType.CREATE, Random.Range(0, 100),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
            CreateBullet(3, pos1, 10f, 40f, accel, BulletType.CREATE, Random.Range(0, 100),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
            CreateBullet(3, pos2, 10f, -40f, accel, BulletType.CREATE, Random.Range(0, 100),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2Int(100, 150));
        }
        
        StartCoroutine(m_EnemyBoss3Barrel[0].ShootAnimation());
        StartCoroutine(m_EnemyBoss3Barrel[1].ShootAnimation());
        yield break;
    }

    private IEnumerator Pattern2A() {
        Vector3 pos;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.5f, 1200);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0);
        int[] fire_delay = { 6, 2, 1 };

        m_InPattern = true;
        //m_DirectionState = 3;

        while (true) {
            pos = m_FirePosition[0].position;
            CreateBullet(4, pos, Random.Range(8f, 9f), Random.Range(0f, 360f), accel1, BulletType.ERASE_AND_CREATE, 1200,
            0, 3.2f, BulletDirection.FIXED, Random.Range(0f, 360f), accel2, 2, Random.Range(0f, 360f)); // PL
            CreateBullet(4, pos, Random.Range(8f, 9f), Random.Range(0f, 360f), accel1, BulletType.ERASE_AND_CREATE, 1200,
            0, 3.2f, BulletDirection.FIXED, Random.Range(0f, 360f), accel2, 2, Random.Range(0f, 360f)); // PL
            CreateBullet(4, pos, Random.Range(9f, 10f), Random.Range(0f, 360f), accel1, BulletType.ERASE_AND_CREATE, 1200,
            2, 2.1f, BulletDirection.FIXED, Random.Range(0f, 360f), accel2, 2, Random.Range(0f, 360f)); // PS
            CreateBullet(4, pos, Random.Range(9f, 10f), Random.Range(0f, 360f), accel1, BulletType.ERASE_AND_CREATE, 1200,
            2, 2.1f, BulletDirection.FIXED, Random.Range(0f, 360f), accel2, 2, Random.Range(0f, 360f)); // PS
            yield return new WaitForFrames(fire_delay[m_SystemManager.GetDifficulty()]);
        }
    }

    private IEnumerator Pattern2B1() {
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int total_duration = 1400, duration, fire_delay = 80;
        
        m_DirectionState = 4;
        m_RotateDirection = RandomValue();
        m_Direction = - m_RotateDirection * m_MaxRotation * 0.7f;

        while (true) {
            for (int i = 0; i < 3; i++) {
                duration = 0;
                while (duration < total_duration) {
                    pos1 = m_FirePosition[1].position;
                    pos2 = m_FirePosition[2].position;

                    if (m_SystemManager.GetDifficulty() == 0) {
                        CreateBulletsSector(0, pos1, 8f, m_Direction - 20f, accel, 4, 50f);
                        CreateBulletsSector(0, pos2, 8f, m_Direction + 20f, accel, 4, 50f);
                    }
                    else if (m_SystemManager.GetDifficulty() == 1) {
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
        EnemyBulletAccel accel = new EnemyBulletAccel(4.2f, 900);
        int[] fire_delay = { 100, 64, 50 };
        float target_angle;
        
        while (true) {
            pos = m_FirePosition[0].position;
            target_angle = GetAngleToTarget(pos, m_PlayerManager.GetPlayerPosition());
            CreateBulletsSector(4, pos, 8f, target_angle + Random.Range(-40f, 40f), accel, 2, 8f);
            yield return new WaitForMillisecondFrames(fire_delay[m_SystemManager.GetDifficulty()]);
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
        m_SystemManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(1f, 0f);
        ExplosionEffect(2, -1, new Vector2(0f, -0.5f), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
        ExplosionEffect(2, -1, new Vector2(0f, 0.6f), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(0.4f, 0.7f), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(0.4f, 0.2f), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(-0.4f, 0.7f), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(-0.4f, 0.2f), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
        
        yield return new WaitForMillisecondFrames(600);

        StartCoroutine(DeathExplosion1(2800));
        StartCoroutine(DeathExplosion2(2800));
        StartCoroutine(DeathExplosion3(2800));

        yield return new WaitForMillisecondFrames(1000);
        ExplosionEffect(0, 0, new Vector3(0f, -0.8f, Depth.EXPLOSION), new MoveVector(0.7f, Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector3(-0.25f, -0.4f, Depth.EXPLOSION), new MoveVector(0.7f, Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector3(0.25f, -0.4f, Depth.EXPLOSION), new MoveVector(0.7f, Random.Range(0f, 360f)));
        m_PartOnDeath[0].SetActive(false);

        yield return new WaitForMillisecondFrames(800);
        ExplosionEffect(0, 0, new Vector3(-0.45f, 0.5f, Depth.EXPLOSION), new MoveVector(1.2f, 180f + Random.Range(80f, 100f)));
        ExplosionEffect(0, -1, new Vector3(0.45f, 0.5f, Depth.EXPLOSION), new MoveVector(1.2f, Random.Range(80f, 100f)));
        ExplosionEffect(1, -1, new Vector3(-0.5f, 0.24f, Depth.EXPLOSION), new MoveVector(0.7f, Random.Range(-10f, 10f)));
        ExplosionEffect(1, -1, new Vector3(0.5f, 0.24f, Depth.EXPLOSION), new MoveVector(0.7f, Random.Range(-10f, 10f)));
        ExplosionEffect(1, -1, new Vector3(-0.4f, 0.6f, Depth.EXPLOSION));
        ExplosionEffect(1, -1, new Vector3(0.4f, 0.6f, Depth.EXPLOSION));
        m_PartOnDeath[1].SetActive(false);

        yield return new WaitForMillisecondFrames(1200);
        ExplosionEffect(2, 4, new Vector3(0f, 0.64f, Depth.EXPLOSION), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f))); // 최종 파괴
        ExplosionEffect(1, -1, new Vector3(0f, -0.12f, Depth.EXPLOSION), new MoveVector(1.8f, Random.Range(-30f, -10f)));
        ExplosionEffect(1, -1, new Vector3(0f, -0.12f, Depth.EXPLOSION), new MoveVector(1.8f, Random.Range(10f, 30f)));
        ExplosionEffect(1, -1, new Vector3(0.2f, 0.75f, Depth.EXPLOSION), new MoveVector(1.8f, Random.Range(110f, 120f)));
        ExplosionEffect(2, -1, new Vector3(0.2f, 0.2f, Depth.EXPLOSION), new MoveVector(1.8f, Random.Range(60f, 70f)));
        ExplosionEffect(1, -1, new Vector3(-0.2f, 0.75f, Depth.EXPLOSION), new MoveVector(1.8f, Random.Range(-110f, -120f)));
        ExplosionEffect(2, -1, new Vector3(-0.2f, 0.2f, Depth.EXPLOSION), new MoveVector(1.8f, Random.Range(-70f, -60f)));
        ExplosionEffect(0, -1, new Vector3(0.2f, 0.18f, Depth.EXPLOSION), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
        ExplosionEffect(0, -1, new Vector3(-0.2f, 0.18f, Depth.EXPLOSION), new MoveVector(Random.Range(0.75f, 1.25f), Random.Range(0f, 360f)));
        
        m_EnemyDeath.OnDeath();
        yield break;
    }

    public void OnBossDying() {
        m_SystemManager.BossClear();
    }

    public void OnBossDeath() {
        m_SystemManager.StartStageClearCoroutine();
        m_SystemManager.ScreenEffect(1);
        m_SystemManager.ShakeCamera(1f);
    }

    private IEnumerator DeathExplosion1(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(500, 700);
            ExplosionEffect(2, 3, new Vector3(Random.Range(-0.64f, 0.64f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-0.64f, 0.64f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(350, 500);
            ExplosionEffect(1, 2, new Vector3(Random.Range(-0.64f, 0.64f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            ExplosionEffect(1, -1, new Vector3(Random.Range(-0.64f, 0.64f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion3(int duration) {
        int timer = 0, t_add = 0;
        while (timer < duration) {
            t_add = Random.Range(200, 400);
            ExplosionEffect(0, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            ExplosionEffect(0, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }
}
