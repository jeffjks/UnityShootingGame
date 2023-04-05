using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss5 : EnemyUnit
{
    public GameObject m_Core, m_Wings, m_WingsForAppearance;
    public MeshRenderer[] m_WingMeshRenderers = new MeshRenderer[3];
    public Transform m_FirePosition;
    public Transform[] m_FirePositionsWing = new Transform[3];

    private int m_Phase;
    private float m_Direction;
    private Vector3 m_TargetPosition;
    private const int APPEARNCE_TIME = 10000;
    private float m_WingsAngle;
    private int m_MoveDirection;
    private float m_MoveSpeed, m_DefaultSpeed = 0.2f;
    private float m_TrackPos;

    private int[,] m_FireDelay1 = {{ 600, 360, 250 }, { 2400, 2000, 2000 }};
    private int[,] m_FireDelay2 = {{ 2400, 2000, 2000 }, { 600, 400, 250 }};
    private int m_CurrentFireDelay1, m_CurrentFireDelay2;

    private IEnumerator m_CurrentPhase;
    private IEnumerator[] m_CurrentPattern = new IEnumerator[4];

    void Start()
    {
        m_TargetPosition = new Vector2(0f, -4f);
        m_WingsAngle = Random.Range(0f, 360f);
        for (int i = 0; i < m_WingMeshRenderers.Length; i++) {
            m_WingMeshRenderers[i].gameObject.SetActive(false);
        }

        DisableAttackable();
        
        /*
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(m_TargetPosition.y, APPEARNCE_TIME).SetEase(Ease.Linear));*/

        StartCoroutine(AppearanceSequence());
    }

    private IEnumerator AppearanceSequence() {
        float init_position_y = transform.position.y;
        int frame = APPEARNCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_posy = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);

            float position_y = Mathf.Lerp(init_position_y, m_TargetPosition.y, t_posy);
            transform.position = new Vector3(transform.position.x, position_y, transform.position.z);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
        yield break;
    }

    private void OnAppearanceComplete() {
        float random_direction = 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.05f, random_direction);
        m_MoveDirection = Random.Range(0, 2)*2 - 1;
        ToNextPhase();
        StartCoroutine(InitMaterial());

        EnableAttackable();
    }

    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 4 / 10) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase == -1) {
            m_MoveVector.speed += 0.72f / Application.targetFrameRate * Time.timeScale;
        }
        else if (m_Phase > 0) {
            if (transform.position.x >= m_TargetPosition.x + 0.7f) {
                m_MoveDirection = -1;
            }
            else if (transform.position.x <= m_TargetPosition.x - 0.7f) {
                m_MoveDirection = 1;
            }
            else if (transform.position.y >= m_TargetPosition.y + 0.2f) {
                m_MoveVector.direction = 0f;
            }
            else if (transform.position.y <= m_TargetPosition.y - 0.2f) {
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
        base.Update();
    }

    private void RotateWings() {
        m_WingsAngle += 30f / Application.targetFrameRate * Time.timeScale;
        if (m_Core != null)
            m_Core.transform.localRotation = Quaternion.Euler(0f, 0f, m_Core.transform.rotation.eulerAngles.z - 10f / Application.targetFrameRate * Time.timeScale);
        if (m_Wings != null)
            m_Wings.transform.localRotation = Quaternion.Euler(0f, 0f, m_WingsAngle);
        
        if (m_WingsAngle > 360f)
            m_WingsAngle -= 360f;
        else if (m_WingsAngle < 0f)
            m_WingsAngle += 360f;
    }

    private IEnumerator OpenWings() {
        Quaternion[] init_rotation = new Quaternion[m_WingMeshRenderers.Length];
        for (int i = 0; i < m_WingMeshRenderers.Length; ++i) {
            m_FirePositionsWing[i].localPosition = new Vector3(0f, 4.5f, -3.3f);
            init_rotation[i] = m_WingMeshRenderers[i].transform.rotation;
        }
        
        int frame = 1500 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_rot = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame);

            for (int j = 0; j < m_WingMeshRenderers.Length; ++j) {
                m_WingMeshRenderers[j].transform.rotation = Quaternion.Lerp(init_rotation[j], Quaternion.Euler(30f, 0f, 0f), t_rot);
            }
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator InitMaterial() {
        yield return new WaitForMillisecondFrames(400);
        for (int i = 0; i < m_WingMeshRenderers.Length; i++) {
            m_WingMeshRenderers[i].material.SetColor("_EmissionColor", Color.white);
        }
        m_WingsForAppearance.SetActive(false);
        for (int i = 0; i < m_WingMeshRenderers.Length; i++) {
            m_WingMeshRenderers[i].gameObject.SetActive(true);
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
            StartCoroutine(NextPhaseExplosion());
            m_SystemManager.EraseBullets(2000);
            StartCoroutine(OpenWings());

            if (m_CurrentPhase != null)
                StopCoroutine(m_CurrentPhase);
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
        }
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1500);
        StartCoroutine(Pattern1_A0(11000));
        m_CurrentPattern[0] = Pattern1_A1();
        StartCoroutine(m_CurrentPattern[0]);
        m_CurrentPattern[1] = Pattern1_A2();
        StartCoroutine(m_CurrentPattern[1]);
        yield return new WaitForMillisecondFrames(15000);

        while (m_Phase == 1) {
            m_CurrentPattern[0] = Pattern1_B1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_B2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForMillisecondFrames(10000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(2500);

            m_CurrentPattern[0] = Pattern1_C1();
            StartCoroutine(m_CurrentPattern[0]);
            yield return new WaitForMillisecondFrames(3000);
            m_CurrentPattern[1] = Pattern1_C2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForMillisecondFrames(7000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);

            m_CurrentPattern[0] = Pattern1_D1(0, 1600, 2f);
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_D1(1, 1200, -1f);
            StartCoroutine(m_CurrentPattern[1]);
            m_CurrentPattern[2] = Pattern1_D1(2, 800, -3f);
            StartCoroutine(m_CurrentPattern[2]);
            yield return new WaitForMillisecondFrames(3000);
            m_CurrentPattern[3] = Pattern1_D2();
            StartCoroutine(m_CurrentPattern[3]);
            yield return new WaitForMillisecondFrames(11000);
            StopAllPatterns();
            yield return new WaitForMillisecondFrames(3000);
        }
        yield break;
    }

    private IEnumerator Pattern1_A0(int duration) {
        int init_fire_delay_1 = m_FireDelay1[0, m_SystemManager.m_Difficulty]; // start value
        int init_fire_delay_2 = m_FireDelay2[0, m_SystemManager.m_Difficulty]; // start value
        int target_fire_delay_1 = m_FireDelay1[1, m_SystemManager.m_Difficulty]; // target value
        int target_fire_delay_2 = m_FireDelay2[1, m_SystemManager.m_Difficulty]; // target value
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_delay = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame);

            float fire_delay_1 = Mathf.Lerp(init_fire_delay_1, target_fire_delay_1, t_delay);
            float fire_delay_2 = Mathf.Lerp(init_fire_delay_2, target_fire_delay_2, t_delay);
            m_CurrentFireDelay1 = (int) fire_delay_1;
            m_CurrentFireDelay2 = (int) fire_delay_2;
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator Pattern1_A1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        //int[] fire_delay_start = { 600, 360, 250 };
        //int[] fire_delay_end = { 2400, 2000, 2000 };
        //int fire_delay = fire_delay_start[m_SystemManager.m_Difficulty];
        float rand = Random.Range(0f, 360f);
        //DOTween.To(()=>fire_delay, x=>fire_delay = x, fire_delay_end[m_SystemManager.m_Difficulty], 11f).SetEase(Ease.InOutQuad);

        while (m_CurrentFireDelay1 < m_FireDelay1[1,m_SystemManager.m_Difficulty]) {
            pos = m_FirePosition.position;
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(3, pos, 6.7f, rand, accel, 18, 20f);
                rand += Random.Range(8.4375f, 14.0625f);
                yield return new WaitForMillisecondFrames(m_CurrentFireDelay1);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(3, pos, 7f, rand, accel, 24, 15f);
                rand += Random.Range(6.75f, 11.25f);
                yield return new WaitForMillisecondFrames(m_CurrentFireDelay1);
            }
            else {
                CreateBulletsSector(3, pos, 7f, rand, accel, 30, 12f);
                rand += Random.Range(4.5f, 7.5f);
                yield return new WaitForMillisecondFrames(m_CurrentFireDelay1);
            }
        }
        yield break;
    }

    private IEnumerator Pattern1_A2() {
        Vector3 pos;
        float dir;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0), accel2 = new EnemyBulletAccel(4f, 600);
        //int[] fire_delay_start = { 2400, 2000, 2000 };
        //int[] fire_delay_end = { 600, 400, 250 };
        //int fire_delay = fire_delay_start[m_SystemManager.m_Difficulty];
        //DOTween.To(()=>fire_delay, x=>fire_delay = x, fire_delay_end[m_SystemManager.m_Difficulty], 11f).SetEase(Ease.InOutQuad);

        while (m_CurrentFireDelay2 > m_FireDelay2[1,m_SystemManager.m_Difficulty]) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition) + Random.Range(-6f, 6f);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(1, pos, 5f, dir, accel, 6, 20f, BulletType.ERASE_AND_CREATE, 600,
                1, 2f, BulletDirection.CURRENT, 0f, accel2, 2, 20f + Random.Range(0f, 24f));
                yield return new WaitForMillisecondFrames(m_CurrentFireDelay2);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(1, pos, 5.3f, dir, accel, 10, 12f, BulletType.ERASE_AND_CREATE, 600,
                1, 2f, BulletDirection.CURRENT, 0f, accel2, 2, 20f + Random.Range(0f, 24f));
                yield return new WaitForMillisecondFrames(m_CurrentFireDelay2);
            }
            else {
                CreateBulletsSector(1, pos, 5.6f, dir, accel, 12, 10f, BulletType.ERASE_AND_CREATE, 600,
                1, 2f, BulletDirection.CURRENT, 0f, accel2, 2, 20f + Random.Range(0f, 24f));
                yield return new WaitForMillisecondFrames(m_CurrentFireDelay2);
            }
        }
        yield break;
    }

    private IEnumerator Pattern1_B1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float dir;

        while (true) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition) + Random.Range(-5f, 5f);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(1, pos, 5.2f, dir - 2f, accel, 7, 19f);
                CreateBulletsSector(1, pos, 5.4f, dir, accel, 7, 19f);
                CreateBulletsSector(1, pos, 5.2f, dir + 2f, accel, 7, 19f);
                yield return new WaitForMillisecondFrames(1000);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(1, pos, 5.4f, dir - 2f, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.6f, dir, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.4f, dir + 2f, accel, 9, 14f);
                yield return new WaitForMillisecondFrames(650);
            }
            else {
                CreateBulletsSector(1, pos, 5.2f, dir - 4f, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.4f, dir - 2f, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.6f, dir, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.4f, dir + 2f, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.2f, dir + 4f, accel, 9, 14f);
                yield return new WaitForMillisecondFrames(500);
            }
        }
    }

    private IEnumerator Pattern1_B2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float dir;

        while (true) {
            Vector3 center_point = transform.position + new Vector3(0f, 5f, 0f);
            for (int i = 0; i < m_FirePositionsWing.Length; i++) {
                pos = m_FirePositionsWing[i].position;
                dir = GetAngleToTarget(center_point, pos);
                CreateBullet(4, pos, 6f, dir, accel);
            }
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForMillisecondFrames(400);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForMillisecondFrames(210);
            }
            else {
                yield return new WaitForMillisecondFrames(170);
            }
        }
    }

    private IEnumerator Pattern1_C1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float dir;

        while (true) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition);
            if (m_SystemManager.m_Difficulty == 0) {
                dir += Random.Range(-10f, 10f);
                CreateBulletsSector(3, pos, 4f, dir, accel, 7, 17f);
                CreateBulletsSector(5, pos, 5.6f, dir, accel, 7, 13f);
                yield return new WaitForMillisecondFrames(1200);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                dir += Random.Range(-8f, 8f);
                CreateBulletsSector(3, pos, 4f, dir, accel, 8, 14f);
                CreateBulletsSector(5, pos, 5.6f, dir, accel, 8, 10f);
                yield return new WaitForMillisecondFrames(600);
            }
            else {
                dir += Random.Range(-6f, 6f);
                CreateBulletsSector(3, pos, 4f, dir, accel, 9, 12f);
                CreateBulletsSector(5, pos, 5.6f, dir, accel, 9, 8f);
                yield return new WaitForMillisecondFrames(400);
            }
        }
    }

    private IEnumerator Pattern1_C2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float dir;

        while (true) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition);
            if (m_SystemManager.m_Difficulty < 2) {
                break;
            }
            if (m_SystemManager.m_Difficulty == 2) {
                CreateBulletsSector(4, pos, 6f, dir, accel, 9, 10f);
                yield return new WaitForMillisecondFrames(500);
            }
        }
    }

    private IEnumerator Pattern1_D1(int fire_position, int duration, float dir_delta) {
        Vector3 pos = m_FirePositionsWing[fire_position].position;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float dir = GetAngleToTarget(pos, m_PlayerPosition);
        int time = 0;
        int[] time_add = { 120, 120, 100 };
        int[] period = { 1200, 700, 400 };

        int[] duration_scale = { 50, 80, 100 }; // 난이도에 따른 duration 비율 (%)
        duration = duration * duration_scale[m_SystemManager.m_Difficulty] / 10;

        yield return new WaitForMillisecondFrames(Random.Range(0, 1200));

        while (true) {
            while (time < duration) {
                pos = m_FirePositionsWing[fire_position].position;
                if (m_SystemManager.m_Difficulty == 0) {
                    CreateBulletsSector(0, pos, 4.4f, dir, accel, 3, 90f);
                }
                else if (m_SystemManager.m_Difficulty == 1) {
                    CreateBulletsSector(0, pos, 4.4f, dir, accel, 5, 72f);
                }
                else {
                    CreateBulletsSector(0, pos, 4.4f, dir, accel, 6, 60f);
                }
                time += time_add[m_SystemManager.m_Difficulty];
                dir += dir_delta;
                yield return new WaitForMillisecondFrames(time_add[m_SystemManager.m_Difficulty]);
            }
            time = 0;
            yield return new WaitForMillisecondFrames(period[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern1_D2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float dir;

        while (true) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBullet(5, pos, 3.6f, dir, accel);
                yield return new WaitForMillisecondFrames(1500);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBullet(5, pos, 3.6f, dir, accel);
                yield return new WaitForMillisecondFrames(600);
            }
            else {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(5, pos, 3.6f, dir, accel);
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(300);
            }
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
        yield break;
    }

    private IEnumerator Pattern2_A1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int[] period = { 900, 350, 250 };
        int number = Random.Range(0, 2);

        while (true) {
            for (int i = 0; i < 3; i++) {
                pos = m_FirePositionsWing[i].position;
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

                if (m_SystemManager.m_Difficulty == 0) {
                    CreateBulletsSector(5, pos, 5.5f, Random.Range(-5f, 5f), accel, 4 - number, 25f);
                }
                else if (m_SystemManager.m_Difficulty == 1) {
                    CreateBulletsSector(5, pos, 5.7f, Random.Range(-5f, 5f), accel, 5 - number, 15f);
                }
                else {
                    CreateBulletsSector(5, pos, 6f, Random.Range(-5f, 5f), accel, 5 - number, 15f);
                }
                number = 1 - number;
            }
            yield return new WaitForMillisecondFrames(period[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern2_A2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float interval = Random.Range(0f, 260f);

        while (true) {
            pos = m_FirePosition.position;
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
        EnemyBulletAccel accel1 = new EnemyBulletAccel(5f, 800);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(6f, 800);
        EnemyBulletAccel accel3 = new EnemyBulletAccel(7f, 800);
        int[] period = { 320, 150, 100 };

        while (true) {
            for (int i = 0; i < 3; i++) {
                pos = m_FirePositionsWing[i].position;
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

                if (m_SystemManager.m_Difficulty == 0) {
                    CreateBullet(4, pos, 2f, Random.Range(-18f, 18f), accel1);
                }
                else if (m_SystemManager.m_Difficulty == 1) {
                    CreateBullet(4, pos, 2.5f, Random.Range(-18f, 18f), accel2);
                }
                else {
                    CreateBullet(4, pos, 3f, Random.Range(-18f, 18f), accel1);
                }
            }
            yield return new WaitForMillisecondFrames(period[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern2_B2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float interval = 350f, dir, timer = 0f;
        float[] min_interval = { 45f, 35f, 30f };
        int[] number = { 52, 54, 55 };
        int rand;

        while (interval > min_interval[m_SystemManager.m_Difficulty]) {
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 8f, 0f, accel, 2, interval);
            CreateBulletsSector(0, pos, 8f, 0f, accel, 2, interval + 14f);
            CreateBulletsSector(1, pos, 8f, 0f, accel, 2, interval + 21f);
            interval -= 21.1f;
            yield return new WaitForMillisecondFrames(80);
        }
        interval = min_interval[m_SystemManager.m_Difficulty];
        rand = 2*Random.Range(0, 2) - 1;
        dir = 0f;
        while (true) {
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 8f, dir, accel, 2, interval);
            CreateBulletsSector(0, pos, 8f, dir, accel, 2, interval + 14f);
            CreateBulletsSector(1, pos, 8f, dir, accel, 2, interval + 21f);
            if (timer > 0.8f) {
                for (int i = 0; i < number[m_SystemManager.m_Difficulty]; i++)
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int[] period = { 640, 400, 300 };
        float[] dir = { Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f) };

        while (true) {
            for (int i = 0; i < 3; i++) {
                pos = m_FirePositionsWing[i].position;
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

                if (m_SystemManager.m_Difficulty == 0) {
                    CreateBulletsSector(1, pos, 5.2f, dir[i] + m_Direction, accel, 15, 24f);
                }
                else {
                    CreateBulletsSector(1, pos, 5.2f, dir[i] + m_Direction, accel, 20, 18f);
                }
            }
            yield return new WaitForMillisecondFrames(period[m_SystemManager.m_Difficulty]);
        }
    }

    private void StopAllPatterns() {
        for (int i = 0; i < m_CurrentPattern.Length; i++) {
            if (m_CurrentPattern[i] != null) {
                StopCoroutine(m_CurrentPattern[i]);
            }
        }
    }


    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_Phase = -1;
        StopAllPatterns();
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        for (int i = 0; i < m_WingMeshRenderers.Length; i++) {
            ExplosionEffect(2, -1, m_WingMeshRenderers[i].transform.TransformPoint(new Vector3(0f, 0f, 7f)));
            ExplosionEffect(2, -1, m_WingMeshRenderers[i].transform.position);
            ExplosionEffect(2, -1, m_WingMeshRenderers[i].transform.TransformPoint(new Vector3(0f, 0f, -5f)));
        }
        ExplosionEffect(2, -1);
        Destroy(m_Wings);
        m_SystemManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0f, 0f);

        StartCoroutine(DeathExplosion1(3200));
        StartCoroutine(DeathExplosion2(3200));
        
        Vector2 random_pos;
        yield return new WaitForMillisecondFrames(800);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, 2, random_pos);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, -1, random_pos);
        yield return new WaitForMillisecondFrames(800);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, 2, random_pos);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, -1, random_pos);
        yield return new WaitForMillisecondFrames(1200);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, 2, random_pos);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, -1, random_pos);
        yield return new WaitForMillisecondFrames(1200);
        
        ExplosionEffect(2, 3); // 최종 파괴
        ExplosionEffect(2, 2, new Vector2(0f, 0f), new MoveVector(3f, Random.Range(0f, 360f)));
        ExplosionEffect(2, -1, new Vector2(-4f, 0f));
        ExplosionEffect(2, -1, new Vector2(4f, 0f));
        ExplosionEffect(2, -1, new Vector2(-2f, -3.4f));
        ExplosionEffect(2, -1, new Vector2(-2f, 3.4f));
        ExplosionEffect(2, -1, new Vector2(2f, -3.4f));
        ExplosionEffect(2, -1, new Vector2(2f, 3.4f));
        m_SystemManager.ScreenEffect(1);
        m_SystemManager.ShakeCamera(1.5f);
        
        if (m_SystemManager.m_Difficulty == Difficulty.HELL) {
            try {
                ((Stage5Manager) m_SystemManager.m_StageManager).TrueLastBoss(new Vector3(transform.position.x, transform.position.y, Depth.ENEMY)); // True Last Boss
            }
            catch {
                Debug.LogAssertion("Cannot cast StageManager to Stage5Manager!");
            }
        }

        CreateItems();
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator NextPhaseExplosion() {
        ExplosionEffect(1, 2, new Vector2(0f, 0f), new MoveVector(3f, Random.Range(0f, 360f)));
        ExplosionEffect(2, -1, new Vector2(-Random.Range(2.5f, 3.5f), -Random.Range(2.5f, 3.5f)), new MoveVector(3f, Random.Range(0f, 360f)));
        ExplosionEffect(2, -1, new Vector2(-Random.Range(2.5f, 3.5f), Random.Range(2.5f, 3.5f)), new MoveVector(3f, Random.Range(0f, 360f)));
        ExplosionEffect(2, -1, new Vector2(Random.Range(2.5f, 3.5f), -Random.Range(2.5f, 3.5f)), new MoveVector(3f, Random.Range(0f, 360f)));
        ExplosionEffect(2, -1, new Vector2(Random.Range(2.5f, 3.5f), Random.Range(2.5f, 3.5f)), new MoveVector(3f, Random.Range(0f, 360f)));
        yield break;
    }

    private IEnumerator DeathExplosion1(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(250, 350);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, 0, random_pos, new MoveVector(Random.Range(2f, 3.5f), Random.Range(0f, 360f)));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, -1, random_pos, new MoveVector(Random.Range(2f, 3.5f), Random.Range(0f, 360f)));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(int duration) {
        int timer = 0, t_add = 0;
        Vector2 random_pos;
        while (timer < duration) {
            t_add = Random.Range(400, 600);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, 1, random_pos, new MoveVector(Random.Range(1f, 2f), Random.Range(0f, 360f)));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, -1, random_pos, new MoveVector(Random.Range(1f, 2f), Random.Range(0f, 360f)));
            timer += t_add;
            yield return new WaitForMillisecondFrames(t_add);
        }
        yield break;
    }
}
