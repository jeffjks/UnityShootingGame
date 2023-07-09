using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossFinal : EnemyUnit, IHasAppearance, IEnemyBossMain
{
    public GameObject m_BombBarrier;
    public Transform m_Core;

    private int m_Phase;
    private float[] m_Direction = new float[2], m_DirectionDelta = new float[2];
    private int m_DirectionSide = 1;
    private float m_BulletSpeed;
    private readonly Vector3 TARGET_POSITION = new (0f, -3.8f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 1600;
    private bool m_InPattern = false;

    private IEnumerator m_CurrentPhase;
    private IEnumerator[] m_CurrentPattern = new IEnumerator[5];
    private Vector3 m_RotateAxis = new Vector2(1f, 1f);
    private float m_RotateAngle, m_RotateAxisAngle = 45f;
    private const float ROTATE_SPEED = 120f;
    private const float ROTATE_AXIS_SPEED = 180f;

    void Start()
    {
        //m_RotateAxisSide = 2*Random.Range(0, 2) - 1;
        
        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDeath;
    }

    public IEnumerator AppearanceSequence() {
        Vector3 init_position = transform.position;
        Vector3 init_scale = transform.localScale;
        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
            float t_scale = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_position, TARGET_POSITION, t_pos);
            transform.localScale = Vector3.Lerp(init_scale, new Vector3(1f, 1f, 1f), t_pos);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    public void OnAppearanceComplete() {
        float[] random_direction = { 70f, 110f, -70f, -110f };
        m_MoveVector = new MoveVector(1f, random_direction[Random.Range(0, 4)]);

        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        StageManager.IsTrueBossEnabled = false;
        
        PlayerInvincibility.Instance.Action_OnInvincibilityChanged += SetBombBarrier;
        SetBombBarrier(PlayerInvincibility.IsInvincible);

        EnableInteractableAll();
        
        SystemManager.OnBossStart();
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.m_HealthPercent <= 0.50f) { // 체력 50% 이하
                ToNextPhase();
            }
        }

        if (m_Phase > 0) {
            if (transform.position.x > TARGET_POSITION.x + 1.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
                transform.position = new Vector3(TARGET_POSITION.x + 1.5f, transform.position.y, transform.position.z);
            }
            if (transform.position.x < TARGET_POSITION.x - 1.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
                transform.position = new Vector3(TARGET_POSITION.x - 1.5f, transform.position.y, transform.position.z);
            }
            if (transform.position.y > TARGET_POSITION.y + 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                transform.position = new Vector3(transform.position.x, TARGET_POSITION.y + 0.4f, transform.position.z);
            }
            if (transform.position.y < TARGET_POSITION.y - 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                transform.position = new Vector3(transform.position.x, TARGET_POSITION.y - 0.4f, transform.position.z);
            }
        }
        
        Rotate();

        for (int i = 0; i < m_Direction.Length; i++) {
            m_Direction[i] += m_DirectionDelta[i] / Application.targetFrameRate * Time.timeScale;
            if (m_Direction[i] > 360f)
                m_Direction[i] -= 360f;
            else if (m_Direction[i] < 0f)
                m_Direction[i] += 360f;
        }
    }

    private void Rotate() {
        Vector3 temp_rotate_axis;
        m_RotateAngle += ROTATE_SPEED / Application.targetFrameRate * Time.timeScale;
        temp_rotate_axis = Quaternion.AngleAxis(m_RotateAngle, Vector3.up) * m_RotateAxis;
        m_RotateAxis = new Vector2(Mathf.Cos(Mathf.Deg2Rad*m_RotateAxisAngle), Mathf.Sin(Mathf.Deg2Rad*m_RotateAxisAngle));

        //m_RotateAxisAngle += 25f / Application.targetFrameRate * Time.timeScale;
        /*
        m_RotateAxisAngle += 25f / Application.targetFrameRate * Time.timeScale * m_RotateAxisSide;

        if (m_RotateAxisAngle > 70) {
            m_RotateAxisSide = -1;
        }
        else if (m_RotateAxisAngle < 20) {
            m_RotateAxisSide = 1;
        }*/

        m_Core.RotateAround(transform.position, temp_rotate_axis, -ROTATE_AXIS_SPEED / Application.targetFrameRate * Time.timeScale);

        if (m_RotateAngle > 360f)
            m_RotateAngle -= 360f;
        else if (m_RotateAngle < 0f)
            m_RotateAngle += 360f;
    }

    private void SetBombBarrier(bool state) {
        if (DebugOption.InvincibleMod)
            return;
        if (state)
        {
            m_EnemyHealth.SetInvincibility();
        }
        else
        {
            m_EnemyHealth.DisableInvincibility();
        }
        m_BombBarrier.SetActive(state);
    }

    public void ToNextPhase() {
        m_Phase++;
        StopAllPatterns();
        BulletManager.SetBulletFreeState(2000);

        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(3000);
        m_CurrentPattern[0] = Pattern1_A1();
        StartCoroutine(m_CurrentPattern[0]);
        m_CurrentPattern[1] = Pattern1_A2();
        StartCoroutine(m_CurrentPattern[1]);

        while(m_InPattern) {
            yield return new WaitForMillisecondFrames(0);
        }
        StopAllPatterns();
        yield return new WaitForMillisecondFrames(3000);

        while (m_Phase == 1) {
            m_CurrentPattern[0] = Pattern1_B1();
            StartCoroutine(m_CurrentPattern[0]);
            yield return new WaitForMillisecondFrames(3000);
            m_CurrentPattern[1] = Pattern1_B2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForMillisecondFrames(4000);
            m_CurrentPattern[1] = Pattern1_B2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForMillisecondFrames(1500);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
            
            m_CurrentPattern[0] = Pattern1_C1();
            StartCoroutine(m_CurrentPattern[0]);
            yield return new WaitForMillisecondFrames(2000);
            m_CurrentPattern[1] = Pattern1_C2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForMillisecondFrames(8000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            m_CurrentPattern[0] = Pattern1_D1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_D2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForMillisecondFrames(14000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            m_CurrentPattern[0] = Pattern1_E1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_E2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForMillisecondFrames(8000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            m_DirectionSide *= -1;
        }
        yield break;
    }

    private IEnumerator Pattern1_A1() {
        float dir;
        BulletAccel accel = new BulletAccel(0f, 0);

        while (true) {
            dir = GetAngleToTarget(transform.position, PlayerManager.GetPlayerPosition());
            CreateBulletsSector(2, transform.position, 5.4f, dir - 40f, accel, 6, 6f);
            CreateBulletsSector(2, transform.position, 4.8f, dir, accel, 6, 6f);
            CreateBulletsSector(2, transform.position, 5.4f, dir + 40f, accel, 6, 6f);

            CreateBulletsSector(0, transform.position, 4f, dir, accel, 6, 4f);
            yield return new WaitForMillisecondFrames(1600);
        }
    }

    private IEnumerator Pattern1_A2() {
        float dir = Random.Range(0f, 360f);
        BulletAccel accel = new BulletAccel(0f, 0);
        int timer, t_add = 110;
        float random_distribution = 2f;
        int bulletNum1 = 20, bulletNum2 = 25;

        m_InPattern = true;

        timer = 0;
        while (timer < 4000) {
            CreateBulletsSector(3, transform.position, 7f, dir, accel, bulletNum1, 360f/((float) bulletNum1));
            dir += Random.Range(180f/((float) bulletNum1) - random_distribution, 180f/((float) bulletNum1) + random_distribution);
            yield return new WaitForFrames(8);
            CreateBulletsSector(3, transform.position, 7f, dir, accel, bulletNum1, 360f/((float) bulletNum1));
            dir += Random.Range(180f/((float) bulletNum1) - random_distribution, 180f/((float) bulletNum1) + random_distribution);
            yield return new WaitForFrames(8);
            timer += t_add * 2;
        }

        yield return new WaitForMillisecondFrames(500);
        dir = Random.Range(0f, 360f);

        timer = 0;
        while (timer < 3500) {
            CreateBulletsSector(5, transform.position, 8.1f, dir, accel, bulletNum2, 360f/((float) bulletNum2));
            dir += Random.Range(180f/((float) bulletNum2) - random_distribution, 180f/((float) bulletNum2) + random_distribution);
            yield return new WaitForFrames(7);
            CreateBulletsSector(5, transform.position, 8.1f, dir, accel, bulletNum2, 360f/((float) bulletNum2));
            dir += Random.Range(180f/((float) bulletNum2) - random_distribution, 180f/((float) bulletNum2) + random_distribution);
            yield return new WaitForFrames(7);
            timer += t_add * 2;
        }

        m_InPattern = false;
        yield break;
    }



    private IEnumerator Pattern1_B1() {
        BulletAccel accel1 = new BulletAccel(3f, 1000), accel2 = new BulletAccel(6f, 400);
        float dir;

        while (true) {
            dir = GetAngleToTarget(transform.position, PlayerManager.GetPlayerPosition());
            for (int i = 0; i < 3; i++) {
                CreateBullet(2, transform.position, 6f, dir + Random.Range(-45f, 45f), accel1, BulletSpawnType.Create, 600,
                5, 3f, BulletPivot.Current, 0f, accel2);
            }
            yield return new WaitForFrames(5);
        }
    }

    private IEnumerator Pattern1_B2() {
        BulletAccel accel1 = new BulletAccel(0.1f, 1000), accel2;

        for (int i = 0; i < 10; i++) {
            accel2 = new BulletAccel(7f+i*0.86f, 400);
            CreateBulletsSector(0, transform.position, 10f, 0f, accel1, 4, 90f, BulletSpawnType.EraseAndCreate, 500,
            0, 0.1f, BulletPivot.Player, 0f, accel2);
            yield return new WaitForFrames(4);
        }
        yield break;
    }



    private IEnumerator Pattern1_C1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        m_Direction[0] = Random.Range(0f, 360f);
        m_DirectionDelta[0] = 29f;

        while (true) {
            float dir = m_Direction[0]*m_DirectionSide;
            CreateBulletsSector(3, transform.position, 7.2f, dir, accel, 10, 36f);
            CreateBulletsSector(3, transform.position, 7.2f, dir - 11f, accel, 10, 36f);
            yield return new WaitForFrames(9);
        }
    }

    private IEnumerator Pattern1_C2() {
        BulletAccel accel = new BulletAccel(0f, 0);
        m_Direction[1] = Random.Range(0f, 360f);
        m_DirectionDelta[1] = 67f;

        while (true) {
            float dir = m_Direction[1]*m_DirectionSide;
            CreateBulletsSector(0, transform.position, 9f, dir, accel, 9, 40f);
            yield return new WaitForFrames(7);
        }
    }



    private IEnumerator Pattern1_D1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        m_Direction[0] = Random.Range(0f, 360f);
        m_Direction[1] = Random.Range(0f, 360f);
        m_DirectionDelta[0] = 23f*m_DirectionSide;
        m_DirectionDelta[1] = 43f;

        while (true) {
            CreateBulletsSector(3, transform.position, 5.3f, m_Direction[0], accel, 4, 90f, BulletSpawnType.EraseAndCreate, 600,
            4, 6.4f, BulletPivot.Fixed, m_Direction[1], accel, 6, 60f);
            yield return new WaitForFrames(9);
        }
    }

    private IEnumerator Pattern1_D2() {
        /*
        yield return new WaitForMillisecondFrames(5000);
        DOTween.To(()=>m_DirectionDelta[1], x=>m_DirectionDelta[1] = x, -43f, 2.7f).SetEase(Ease.Linear);
        yield return new WaitForMillisecondFrames(4000);
        DOTween.To(()=>m_DirectionDelta[1], x=>m_DirectionDelta[1] = x, 43f, 2.7f).SetEase(Ease.Linear);*/

        float init_direction;
        int frame;
        yield return new WaitForMillisecondFrames(5000);

        init_direction = m_DirectionDelta[1];
        frame = 2700 * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_dir = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            m_DirectionDelta[1] = Mathf.Lerp(init_direction, -43f, t_dir);
            yield return new WaitForMillisecondFrames(0);
        }

        yield return new WaitForMillisecondFrames(1300);

        init_direction = m_DirectionDelta[1];
        frame = 2700 * Application.targetFrameRate / 1000;
        for (int i = 0; i < frame; ++i) {
            float t_dir = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            m_DirectionDelta[1] = Mathf.Lerp(init_direction, 43f, t_dir);
            yield return new WaitForMillisecondFrames(0);
        }

        yield break;
    }



    private IEnumerator Pattern1_E1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        m_Direction[0] = Random.Range(0f, 360f);
        m_DirectionDelta[0] = -79f*m_DirectionSide;

        while (true) {
            for (int i = 0; i < 6; i++)
                CreateBulletsSector(1, transform.position, 4f+i*0.9f, m_Direction[0], accel, 4, 90f);
            yield return new WaitForFrames(7);
        }
    }

    private IEnumerator Pattern1_E2() {
        BulletAccel accel = new BulletAccel(0f, 0);
        float dir;

        while (true) {
            dir = GetAngleToTarget(transform.position, PlayerManager.GetPlayerPosition());
            CreateBulletsSector(0, transform.position, 7.5f, dir, accel, 10, 36f, BulletSpawnType.EraseAndCreate, 700,
            4, 6.4f, BulletPivot.Player, 0f, accel);
            yield return new WaitForMillisecondFrames(1000);
        }
    }

    
    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(2000);
        m_CurrentPattern[0] = Pattern2_A();
        StartCoroutine(m_CurrentPattern[0]);

        while(m_InPattern) {
            yield return new WaitForMillisecondFrames(0);
        }
        StopAllPatterns();
        yield return new WaitForMillisecondFrames(5000);
        m_CurrentPattern[0] = PatternFinal1_1();
        StartCoroutine(m_CurrentPattern[0]);
        yield return new WaitForMillisecondFrames(4000);
        m_CurrentPattern[4] = PatternFinal2();
        StartCoroutine(m_CurrentPattern[4]);
        yield break;
    }



    private IEnumerator Pattern2_A() {
        BulletAccel accel1 = new BulletAccel(1f, 1000), accel2 = new BulletAccel(9.2f, 500), accel3 = new BulletAccel(0f, 0);
        m_DirectionDelta[0] = 31f;
        m_InPattern = true;

        m_Direction[0] = Random.Range(0f, 360f);
        for (int i = 0; i < 3; i++) {
            CreateBulletsSector(0, transform.position, 8f, m_Direction[0], accel1, 30, 12f, BulletSpawnType.EraseAndCreate, 1000,
            0, 1f, BulletPivot.Current, 30f, accel2);
            yield return new WaitForFrames(7);
        }
        yield return new WaitForMillisecondFrames(400);

        m_Direction[0] = Random.Range(0f, 360f);
        for (int i = 0; i < 5; i++) {
            CreateBulletsSector(3, transform.position, 8f, -m_Direction[0], accel1, 30, 12f, BulletSpawnType.EraseAndCreate, 1000,
            3, 1f, BulletPivot.Current, -60f, accel2);
            yield return new WaitForFrames(7);
        }
        yield return new WaitForMillisecondFrames(400);

        m_Direction[0] = Random.Range(0f, 360f);
        for (int i = 0; i < 7; i++) {
            CreateBulletsSector(0, transform.position, 8f, -m_Direction[0], accel1, 30, 12f, BulletSpawnType.EraseAndCreate, 1000,
            0, 1f, BulletPivot.Current, 90f, accel2);
            yield return new WaitForFrames(7);
        }
        yield return new WaitForMillisecondFrames(1400);

        m_Direction[0] = Random.Range(0f, 360f);
        float dir = 0f;
        for (int i = 0; i < 9; i++) {
            CreateBulletsSector(3, transform.position, 10f, dir + Random.Range(3.5f, 6.5f), accel3, 36, 10f);
            //CreateBulletsSector(3, transform.position, 8f, -m_Direction[0], accel1, 30, 12f, BulletSpawnType.EraseAndCreate, 1000,
            //3, 1f, BulletPivot.Current, -140f, accel2);
            yield return new WaitForFrames(10);
        }
        m_InPattern = false;
    }


    /*
    분홍탄 = Direction[0]
    청탄 = Direction[1]
    */

    private IEnumerator PatternFinal1_1() {
        int bullet_delay = 1100;
        float max_speed = 10f, min_speed = 5.6f;
        BulletAccel accel1 = new BulletAccel(0.1f, bullet_delay), accel2 = new BulletAccel(0f, 0);
        m_Direction[0] = Random.Range(0f, 360f);
        m_Direction[1] = Random.Range(0f, 360f);
        m_DirectionDelta[0] = 109f;
        m_DirectionDelta[1] = 73f;
        m_BulletSpeed = max_speed;

        //m_CurrentPattern[1] = PatternFinal1_2(m_DirectionDelta[0]);
        //StartCoroutine(m_CurrentPattern[1]);

        m_CurrentPattern[2] = PatternFinal1_3(m_DirectionDelta[1]);
        StartCoroutine(m_CurrentPattern[2]);

        m_CurrentPattern[3] = PatternFinal1_4(min_speed, max_speed);
        StartCoroutine(m_CurrentPattern[3]);

        while (true) {
            CreateBulletsSector(0, transform.position, m_BulletSpeed, m_Direction[0], accel1, 3, 120f, BulletSpawnType.EraseAndCreate, bullet_delay,
            3, 7.5f, BulletPivot.Fixed, m_Direction[1], accel2, 2, 48f);
            CreateBulletsSector(0, transform.position, m_BulletSpeed, m_Direction[0], accel1, 3, 120f, BulletSpawnType.EraseAndCreate, bullet_delay,
            5, 5.4f, BulletPivot.Fixed, m_Direction[1] + 180f, accel2);
            yield return new WaitForFrames(4); // 62ms
        }
    }

    private IEnumerator PatternFinal1_2(float max_rotate_speed) { // Unused
        yield return new WaitForMillisecondFrames(2400);
        while (true) {
            float init_direction;
            int frame;
            yield return new WaitForMillisecondFrames(Random.Range(8000, 10000) - 2400);

            init_direction = m_DirectionDelta[0];
            frame = 1200 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_dir = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
                
                m_DirectionDelta[0] = Mathf.Lerp(init_direction, -max_rotate_speed, t_dir);
                yield return new WaitForMillisecondFrames(0);
            }
            yield return new WaitForMillisecondFrames(Random.Range(8000, 10000) - 2400);

            init_direction = m_DirectionDelta[0];
            frame = 1200 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_dir = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);
                
                m_DirectionDelta[0] = Mathf.Lerp(init_direction, max_rotate_speed, t_dir);
                yield return new WaitForMillisecondFrames(0);
            }
        }
    }

    private IEnumerator PatternFinal1_3(float max_rotate_speed) {
        yield return new WaitForMillisecondFrames(1700);
        while (true) {
            float init_direction;
            int frame;
            yield return new WaitForMillisecondFrames(Random.Range(7000, 9000) - 1700);

            init_direction = m_DirectionDelta[1];
            frame = 700 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_dir = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame);
                
                m_DirectionDelta[1] = Mathf.Lerp(init_direction, -max_rotate_speed, t_dir);
                yield return new WaitForMillisecondFrames(0);
            }
            yield return new WaitForMillisecondFrames(Random.Range(7000, 9000) - 1700);

            init_direction = m_DirectionDelta[1];
            frame = 700 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_dir = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
                
                m_DirectionDelta[1] = Mathf.Lerp(init_direction, max_rotate_speed, t_dir);
                yield return new WaitForMillisecondFrames(0);
            }
        }
    }

    private IEnumerator PatternFinal1_4(float min_speed, float max_speed) {
        yield return new WaitForMillisecondFrames(2000);
        while (true) {
            float init_speed;
            int frame;
            yield return new WaitForMillisecondFrames(Random.Range(5000, 8000) - 2000);

            init_speed = m_BulletSpeed;
            frame = 2000 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_spd = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
                
                m_BulletSpeed = Mathf.Lerp(init_speed, min_speed, t_spd);
                yield return new WaitForMillisecondFrames(0);
            }
            yield return new WaitForMillisecondFrames(Random.Range(5000, 8000) - 2000);

            init_speed = m_BulletSpeed;
            frame = 2000 * Application.targetFrameRate / 1000;
            for (int i = 0; i < frame; ++i) {
                float t_spd = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
                
                m_BulletSpeed = Mathf.Lerp(init_speed, max_speed, t_spd);
                yield return new WaitForMillisecondFrames(0);
            }
        }
    }

    private IEnumerator PatternFinal2() {
        BulletAccel accel = new BulletAccel(0f, 0);
        float dir = GetAngleToTarget(transform.position, PlayerManager.GetPlayerPosition());
        while (true) {
            CreateBulletsSector(1, transform.position, 5f, dir + Random.Range(-2f, 2f), accel, 26, 13.8461f);
            yield return new WaitForMillisecondFrames(400);
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
        BulletManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0.7f, 0f);
        
        InGameDataManager.Instance.SaveElapsedTime();
        
        yield break;
    }

    public void OnBossDying() {
        SystemManager.OnBossClear();
    }

    public void OnBossDeath() {
        SystemManager.Instance.StartStageClearCoroutine();
        InGameScreenEffectService.WhiteEffect(true);
        MainCamera.ShakeCamera(2f);
    }
}
