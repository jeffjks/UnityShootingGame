using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss5 : EnemyUnit
{
    public GameObject m_Core, m_Wings, m_WingsForAppearance;
    public MeshRenderer[] m_WingMeshRenderers = new MeshRenderer[3];
    public Transform m_FirePosition;
    public Transform[] m_FirePositionsWing = new Transform[3];

    private sbyte m_Phase;
    private float m_Direction;
    private Vector3 m_TargetPosition;
    private float m_AppearanceTime = 10f, m_WingsAngle;
    private int m_MoveDirection;
    private float m_MoveSpeed, m_DefaultSpeed = 0.2f;
    private float m_TrackPos;

    private IEnumerator m_CurrentPhase;
    private IEnumerator[] m_CurrentPattern = new IEnumerator[4];

    void Start()
    {
        DisableAttackable(m_AppearanceTime);
        m_TargetPosition = new Vector2(0f, -4f);
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(m_TargetPosition.y, m_AppearanceTime).SetEase(Ease.Linear));

        Invoke("OnAppearanceComplete", m_AppearanceTime);
        
        for (int i = 0; i < m_WingMeshRenderers.Length; i++) {
            m_WingMeshRenderers[i].gameObject.SetActive(false);
        }
    }

    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase == -1) {
            m_MoveVector.speed += 0.72f*Time.deltaTime;
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
                m_MoveSpeed += 0.13f*Time.deltaTime;
            }
            else if (m_MoveSpeed > m_DefaultSpeed && m_MoveDirection == -1) {
                m_MoveSpeed -= 0.13f*Time.deltaTime;
            }
            else {
                m_MoveSpeed = m_DefaultSpeed*m_MoveDirection;
            }

            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x + m_MoveSpeed*Time.deltaTime, pos.y, Depth.ENEMY);
        }

        m_Direction += 91f*Time.deltaTime;
        if (m_Direction > 360f)
            m_Direction -= 360f;
        else if (m_Direction < 0f)
            m_Direction += 360f;
        
        RotateWings();
        base.Update();
    }

    private void RotateWings() {
        m_WingsAngle += 30f*Time.deltaTime;
        if (m_Core != null)
            m_Core.transform.localRotation = Quaternion.Euler(0f, 0f, m_Core.transform.rotation.eulerAngles.z - 10f*Time.deltaTime);
        if (m_Wings != null)
            m_Wings.transform.localRotation = Quaternion.Euler(0f, 0f, m_WingsAngle);
        
        if (m_WingsAngle > 360f)
            m_WingsAngle -= 360f;
        else if (m_WingsAngle < 0f)
            m_WingsAngle += 360f;
    }

    private void OpenWings() {
        for (int i = 0; i < m_WingMeshRenderers.Length; i++) {
            m_WingMeshRenderers[i].transform.DOLocalRotateQuaternion(Quaternion.Euler(30f, 0f, 0f), 1.5f).SetEase(Ease.InOutQuad);
            m_FirePositionsWing[i].localPosition = new Vector3(0f, 4.5f, -3.3f);
        }
    }

    private void OnAppearanceComplete() {
        float random_direction = 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.05f, random_direction);
        m_MoveDirection = Random.Range(0, 2)*2 - 1;
        ToNextPhase();
        Invoke("InitMaterial", 0.4f);
    }

    private void InitMaterial() {
        for (int i = 0; i < m_WingMeshRenderers.Length; i++) {
            m_WingMeshRenderers[i].material.SetColor("_EmissionColor", Color.white);
        }
        m_WingsForAppearance.SetActive(false);
        for (int i = 0; i < m_WingMeshRenderers.Length; i++) {
            m_WingMeshRenderers[i].gameObject.SetActive(true);
        }
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
            m_SystemManager.EraseBullets(2f);
            OpenWings();

            if (m_CurrentPhase != null)
                StopCoroutine(m_CurrentPhase);
            m_CurrentPhase = Phase2();
            StartCoroutine(m_CurrentPhase);
        }
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForSeconds(1.5f);
        m_CurrentPattern[0] = Pattern1_A1();
        StartCoroutine(m_CurrentPattern[0]);
        m_CurrentPattern[1] = Pattern1_A2();
        StartCoroutine(m_CurrentPattern[1]);
        yield return new WaitForSeconds(15f);

        while (m_Phase == 1) {
            m_CurrentPattern[0] = Pattern1_B1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_B2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForSeconds(10f);
            StopAllPatterns();
            yield return new WaitForSeconds(2.5f);

            m_CurrentPattern[0] = Pattern1_C1();
            StartCoroutine(m_CurrentPattern[0]);
            yield return new WaitForSeconds(3f);
            m_CurrentPattern[1] = Pattern1_C2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForSeconds(7f);
            StopAllPatterns();
            yield return new WaitForSeconds(3f);

            m_CurrentPattern[0] = Pattern1_D1(0, 1.5f, 2f);
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_D1(1, 2.5f, -1f);
            StartCoroutine(m_CurrentPattern[1]);
            m_CurrentPattern[2] = Pattern1_D1(2, 1f, -3f);
            StartCoroutine(m_CurrentPattern[2]);
            yield return new WaitForSeconds(3f);
            m_CurrentPattern[3] = Pattern1_D2();
            StartCoroutine(m_CurrentPattern[3]);
            yield return new WaitForSeconds(11f);
            StopAllPatterns();
            yield return new WaitForSeconds(3f);
        }
        yield break;
    }

    private IEnumerator Pattern1_A1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float[] fire_delay_start = { 0.6f, 0.36f, 0.25f };
        float[] fire_delay_end = { 2.4f, 2f, 2f };
        float fire_delay = fire_delay_start[m_SystemManager.m_Difficulty];
        float rand = Random.Range(0f, 360f);
        DOTween.To(()=>fire_delay, x=>fire_delay = x, fire_delay_end[m_SystemManager.m_Difficulty], 11f).SetEase(Ease.InOutQuad);

        while (fire_delay < fire_delay_end[m_SystemManager.m_Difficulty]) {
            pos = m_FirePosition.position;
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(3, pos, 6.7f, rand, accel, 18, 20f);
                rand += Random.Range(8.4375f, 14.0625f);
                yield return new WaitForSeconds(fire_delay);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(3, pos, 7f, rand, accel, 24, 15f);
                rand += Random.Range(6.75f, 11.25f);
                yield return new WaitForSeconds(fire_delay);
            }
            else {
                CreateBulletsSector(3, pos, 7f, rand, accel, 30, 12f);
                rand += Random.Range(4.5f, 7.5f);
                yield return new WaitForSeconds(fire_delay);
            }
        }
        yield break;
    }

    private IEnumerator Pattern1_A2() {
        Vector3 pos;
        float dir;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f), accel2 = new EnemyBulletAccel(4f, 0.6f);
        float[] fire_delay_start = { 2.4f, 2f, 2f };
        float[] fire_delay_end = { 0.6f, 0.4f, 0.25f };
        float fire_delay = fire_delay_start[m_SystemManager.m_Difficulty];
        DOTween.To(()=>fire_delay, x=>fire_delay = x, fire_delay_end[m_SystemManager.m_Difficulty], 11f).SetEase(Ease.InOutQuad);

        while (fire_delay > fire_delay_end[m_SystemManager.m_Difficulty]) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition) + Random.Range(-6f, 6f);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(1, pos, 5f, dir, accel, 6, 20f, BulletType.ERASE_AND_CREATE, 0.6f,
                1, 2f, BulletDirection.CURRENT, 0f, accel2, 2, 20f + Random.Range(0f, 24f));
                yield return new WaitForSeconds(fire_delay);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(1, pos, 5.3f, dir, accel, 10, 12f, BulletType.ERASE_AND_CREATE, 0.6f,
                1, 2f, BulletDirection.CURRENT, 0f, accel2, 2, 20f + Random.Range(0f, 24f));
                yield return new WaitForSeconds(fire_delay);
            }
            else {
                CreateBulletsSector(1, pos, 5.6f, dir, accel, 12, 10f, BulletType.ERASE_AND_CREATE, 0.6f,
                1, 2f, BulletDirection.CURRENT, 0f, accel2, 2, 20f + Random.Range(0f, 24f));
                yield return new WaitForSeconds(fire_delay);
            }
        }
        yield break;
    }

    private IEnumerator Pattern1_B1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float dir;

        while (true) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition) + Random.Range(-5f, 5f);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(1, pos, 5.2f, dir - 2f, accel, 7, 19f);
                CreateBulletsSector(1, pos, 5.4f, dir, accel, 7, 19f);
                CreateBulletsSector(1, pos, 5.2f, dir + 2f, accel, 7, 19f);
                yield return new WaitForSeconds(1f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(1, pos, 5.4f, dir - 2f, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.6f, dir, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.4f, dir + 2f, accel, 9, 14f);
                yield return new WaitForSeconds(0.65f);
            }
            else {
                CreateBulletsSector(1, pos, 5.2f, dir - 4f, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.4f, dir - 2f, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.6f, dir, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.4f, dir + 2f, accel, 9, 14f);
                CreateBulletsSector(1, pos, 5.2f, dir + 4f, accel, 9, 14f);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator Pattern1_B2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float dir;

        while (true) {
            Vector3 center_point = transform.position + new Vector3(0f, 5f, 0f);
            for (int i = 0; i < m_FirePositionsWing.Length; i++) {
                pos = m_FirePositionsWing[i].position;
                dir = GetAngleToTarget(center_point, pos);
                CreateBullet(4, pos, 6f, dir, accel);
            }
            if (m_SystemManager.m_Difficulty == 0) {
                yield return new WaitForSeconds(0.4f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                yield return new WaitForSeconds(0.21f);
            }
            else {
                yield return new WaitForSeconds(0.17f);
            }
        }
    }

    private IEnumerator Pattern1_C1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float dir;

        while (true) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition);
            if (m_SystemManager.m_Difficulty == 0) {
                dir += Random.Range(-10f, 10f);
                CreateBulletsSector(3, pos, 4f, dir, accel, 7, 17f);
                CreateBulletsSector(5, pos, 5.6f, dir, accel, 7, 13f);
                yield return new WaitForSeconds(1.2f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                dir += Random.Range(-8f, 8f);
                CreateBulletsSector(3, pos, 4f, dir, accel, 8, 14f);
                CreateBulletsSector(5, pos, 5.6f, dir, accel, 8, 10f);
                yield return new WaitForSeconds(0.6f);
            }
            else {
                dir += Random.Range(-6f, 6f);
                CreateBulletsSector(3, pos, 4f, dir, accel, 9, 12f);
                CreateBulletsSector(5, pos, 5.6f, dir, accel, 9, 8f);
                yield return new WaitForSeconds(0.4f);
            }
        }
    }

    private IEnumerator Pattern1_C2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float dir;

        while (true) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition);
            if (m_SystemManager.m_Difficulty < 2) {
                break;
            }
            if (m_SystemManager.m_Difficulty == 2) {
                CreateBulletsSector(4, pos, 6f, dir, accel, 9, 10f);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private IEnumerator Pattern1_D1(int fire_position, float duration, float dir_delta) {
        Vector3 pos = m_FirePositionsWing[fire_position].position;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float dir = GetAngleToTarget(pos, m_PlayerPosition);
        float timer = duration, time = 0f;
        float[] time_add = {0.12f, 0.12f, 0.1f};
        float[] period = {1.2f, 0.7f, 0.7f};

        if (m_SystemManager.m_Difficulty == 0) {
            timer *= 0.5f;
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            timer *= 0.8f;
        }

        while (true) {
            while (time < timer) {
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
                yield return new WaitForSeconds(time_add[m_SystemManager.m_Difficulty]);
            }
            time = 0f;
            yield return new WaitForSeconds(period[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern1_D2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float dir;

        while (true) {
            pos = m_FirePosition.position;
            dir = GetAngleToTarget(pos, m_PlayerPosition);
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBullet(5, pos, 3.6f, dir, accel);
                yield return new WaitForSeconds(1.5f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBullet(5, pos, 3.6f, dir, accel);
                yield return new WaitForSeconds(0.6f);
            }
            else {
                for (int i = 0; i < 4; i++) {
                    CreateBullet(5, pos, 3.6f, dir, accel);
                    yield return new WaitForSeconds(0.2f);
                }
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForSeconds(3f);
        m_CurrentPattern[0] = Pattern2_A1();
        StartCoroutine(m_CurrentPattern[0]);
        m_CurrentPattern[1] = Pattern2_A2();
        StartCoroutine(m_CurrentPattern[1]);
        yield return new WaitForSeconds(8f);
        StopAllPatterns();
        yield return new WaitForSeconds(3f);

        while (m_Phase == 2) {
            m_CurrentPattern[0] = Pattern2_B1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern2_B2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForSeconds(15f);
            StopAllPatterns();
            yield return new WaitForSeconds(3f);
            
            m_CurrentPattern[0] = Pattern2_C();
            StartCoroutine(m_CurrentPattern[0]);
            yield return new WaitForSeconds(8f);
            StopAllPatterns();
            yield return new WaitForSeconds(3f);
        }
        yield break;
    }

    private IEnumerator Pattern2_A1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float[] period = { 0.9f, 0.35f, 0.25f };
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
            yield return new WaitForSeconds(period[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern2_A2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float interval = Random.Range(0f, 260f);

        while (true) {
            pos = m_FirePosition.position;
            CreateBulletsSector(4, pos, 8f, -180f, accel, 2, interval);
            interval += 6.1f;
            if (interval > 260f) {
                interval -= 260f;
            }
            yield return new WaitForSeconds(0.04f);
        }
    }

    private IEnumerator Pattern2_B1() {
        Vector3 pos;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(5f, 0.8f);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(6f, 0.8f);
        EnemyBulletAccel accel3 = new EnemyBulletAccel(7f, 0.8f);
        float[] period = { 0.32f, 0.15f, 0.1f };

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
            yield return new WaitForSeconds(period[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern2_B2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float interval = 350f, dir, timer = 0f;
        float[] min_interval = { 45f, 35f, 30f };
        int rand;

        while (interval > min_interval[m_SystemManager.m_Difficulty]) {
            pos = m_FirePosition.position;
            CreateBulletsSector(1, pos, 8f, 0f, accel, 2, interval);
            CreateBulletsSector(0, pos, 8f, 0f, accel, 2, interval + 14f);
            CreateBulletsSector(1, pos, 8f, 0f, accel, 2, interval + 21f);
            interval -= 21.1f;
            yield return new WaitForSeconds(0.08f);
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
                for (int i = 0; i < 55; i++)
                    CreateBulletsSector(0, pos, 10f, dir + 180f, accel, 2, 3f + i*6f);
                timer -= 0.8f;
            }
            timer += 0.08f;
            dir += 1.5f*rand;
            if (Mathf.Abs(dir) > 40f) {
                rand *= -1;
            }
            yield return new WaitForSeconds(0.08f);
        }
    }

    private IEnumerator Pattern2_C() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float[] period = { 0.64f, 0.4f, 0.3f };
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
            yield return new WaitForSeconds(period[m_SystemManager.m_Difficulty]);
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
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(0f, 0f);

        StartCoroutine(DeathExplosion1(3.2f));
        StartCoroutine(DeathExplosion2(3.2f));
        
        Vector2 random_pos;
        yield return new WaitForSeconds(0.8f);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, 2, random_pos);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, -1, random_pos);
        yield return new WaitForSeconds(0.8f);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, 2, random_pos);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, -1, random_pos);
        yield return new WaitForSeconds(1.2f);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, 2, random_pos);
        random_pos = Random.insideUnitCircle * 3f;
        ExplosionEffect(2, -1, random_pos);
        yield return new WaitForSeconds(1.2f);
        
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

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.25f, 0.35f);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, 0, random_pos, new MoveVector(Random.Range(2f, 3.5f), Random.Range(0f, 360f)));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(0, -1, random_pos, new MoveVector(Random.Range(2f, 3.5f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.4f, 0.6f);
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, 1, random_pos, new MoveVector(Random.Range(1f, 2f), Random.Range(0f, 360f)));
            random_pos = Random.insideUnitCircle * 3f;
            ExplosionEffect(1, -1, random_pos, new MoveVector(Random.Range(1f, 2f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }
}
