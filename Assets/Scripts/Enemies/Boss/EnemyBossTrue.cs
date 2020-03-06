using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBossTrue : EnemyUnit
{
    public GameObject m_BombBarrier;
    public Transform m_Core;

    private sbyte m_Phase;
    private float[] m_Direction = new float[2], m_DirectionDelta = new float[2];
    private int m_DirectionSide = 1;
    private float m_BulletSpeed;
    private Vector3 m_TargetPosition;
    private float m_AppearanceTime = 1.6f;
    private bool m_InPattern = false;

    private IEnumerator m_CurrentPhase;
    private IEnumerator[] m_CurrentPattern = new IEnumerator[5];
    private Vector3 m_RotateAxis = new Vector2(1f, 1f);
    private float m_RotateAngle, m_RotateAxisAngle = 45f;
    //private int m_RotateAxisSide;

    void Start()
    {
        DisableAttackable(m_AppearanceTime);
        m_TargetPosition = new Vector3(0f, -3.8f, Depth.ENEMY);
        //m_RotateAxisSide = 2*Random.Range(0, 2) - 1;
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMove(m_TargetPosition, m_AppearanceTime).SetEase(Ease.InOutQuad))
        .Join(transform.DOScale(new Vector3(1f, 1f, 1f), m_AppearanceTime).SetEase(Ease.InQuad));

        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.50f) { // 체력 50% 이하
                ToNextPhase();
            }
        }

        if (m_Phase > 0) {
            if (transform.position.x > m_TargetPosition.x + 1.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
                transform.position = new Vector3(m_TargetPosition.x + 1.5f, transform.position.y, transform.position.z);
            }
            if (transform.position.x < m_TargetPosition.x - 1.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
                transform.position = new Vector3(m_TargetPosition.x - 1.5f, transform.position.y, transform.position.z);
            }
            if (transform.position.y > m_TargetPosition.y + 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                transform.position = new Vector3(transform.position.x, m_TargetPosition.y + 0.4f, transform.position.z);
            }
            if (transform.position.y < m_TargetPosition.y - 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                transform.position = new Vector3(transform.position.x, m_TargetPosition.y - 0.4f, transform.position.z);
            }
        }
        
        Rotate();
        BombBarrier();

        for (int i = 0; i < m_Direction.Length; i++) {
            m_Direction[i] += m_DirectionDelta[i]*Time.deltaTime;
            if (m_Direction[i] > 360f)
                m_Direction[i] -= 360f;
            else if (m_Direction[i] < 0f)
                m_Direction[i] += 360f;
        }
            
        base.Update();
    }

    private void Rotate() {
        Vector3 temp_rotate_axis;
        m_RotateAngle += 180f*Time.deltaTime;
        temp_rotate_axis = Quaternion.AngleAxis(m_RotateAngle, Vector3.up) * m_RotateAxis;
        m_RotateAxis = new Vector2(Mathf.Cos(Mathf.Deg2Rad*m_RotateAxisAngle), Mathf.Sin(Mathf.Deg2Rad*m_RotateAxisAngle));

        //m_RotateAxisAngle += 25f*Time.deltaTime;
        /*
        m_RotateAxisAngle += 25f*Time.deltaTime*m_RotateAxisSide;

        if (m_RotateAxisAngle > 70) {
            m_RotateAxisSide = -1;
        }
        else if (m_RotateAxisAngle < 20) {
            m_RotateAxisSide = 1;
        }*/

        m_Core.RotateAround(transform.position, temp_rotate_axis, -240f*Time.deltaTime);

        if (m_RotateAngle > 360f)
            m_RotateAngle -= 360f;
        else if (m_RotateAngle < 0f)
            m_RotateAngle += 360f;
    }

    private void BombBarrier() {
        if (m_SystemManager.m_InvincibleMod)
            return;
        if (m_Phase > 0) {
            if (m_PlayerManager.m_PlayerController.GetInvincibility()) {
                if (!m_IsUnattackable) {
                    EnableInvincible();
                    m_BombBarrier.SetActive(true);
                }
            }
            else if (m_IsUnattackable) {
                m_IsUnattackable = false;
                m_BombBarrier.SetActive(false);
            }
        }
    }

    private void OnAppearanceComplete() {
        float[] random_direction = { 70f, 110f, -70f, -110f };
        m_MoveVector = new MoveVector(1f, random_direction[Random.Range(0, 4)]);

        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
        m_SystemManager.m_StageManager.SetTrueLastBossState(false);
    }

    public void ToNextPhase() {
        m_Phase++;
        StopAllPatterns();
        m_SystemManager.EraseBullets(2f);

        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        
        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForSeconds(3f);
        m_CurrentPattern[0] = Pattern1_A1();
        StartCoroutine(m_CurrentPattern[0]);
        m_CurrentPattern[1] = Pattern1_A2();
        StartCoroutine(m_CurrentPattern[1]);

        while(m_InPattern) {
            yield return null;
        }
        StopAllPatterns();
        yield return new WaitForSeconds(3f);

        while (m_Phase == 1) {
            m_CurrentPattern[0] = Pattern1_B1();
            StartCoroutine(m_CurrentPattern[0]);
            yield return new WaitForSeconds(3f);
            m_CurrentPattern[1] = Pattern1_B2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForSeconds(4f);
            m_CurrentPattern[1] = Pattern1_B2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForSeconds(1.5f);
            StopAllPatterns();
            yield return new WaitForSeconds(3f);
            
            m_CurrentPattern[0] = Pattern1_C1();
            StartCoroutine(m_CurrentPattern[0]);
            yield return new WaitForSeconds(2f);
            m_CurrentPattern[1] = Pattern1_C2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForSeconds(8f);
            StopAllPatterns();
            yield return new WaitForSeconds(3f);

            m_CurrentPattern[0] = Pattern1_D1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_D2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForSeconds(14f);
            StopAllPatterns();
            yield return new WaitForSeconds(3f);

            m_CurrentPattern[0] = Pattern1_E1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_E2();
            StartCoroutine(m_CurrentPattern[1]);
            yield return new WaitForSeconds(8f);
            StopAllPatterns();
            yield return new WaitForSeconds(3f);

            m_DirectionSide *= -1;
        }
        yield break;
    }

    private IEnumerator Pattern1_A1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float t, t_add = 0.11f;
        m_Direction[0] = GetAngleToTarget(transform.position, m_PlayerPosition);
        m_InPattern = true;

        t = 0f;
        while (t < 2f) {
            CreateBulletsSector(1, transform.position, 8.8f, m_Direction[0], accel, 4, 90f);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }

        t = 0f;
        while (t < 3f) {
            CreateBulletsSector(1, transform.position, 8.8f, m_Direction[0], accel, 8, 45f);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }

        t = 0f;
        while (t < 4f) {
            CreateBulletsSector(1, transform.position, 8.8f, m_Direction[0], accel, 8, 45f);
            CreateBulletsSector(4, transform.position, 8.8f, m_Direction[0] + 22.5f, accel, 8, 45f);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }

        t = 0f;
        while (t < 5f) {
            CreateBulletsSector(1, transform.position, 8.8f, m_Direction[0], accel, 16, 22.5f);
            CreateBulletsSector(4, transform.position, 8.8f, m_Direction[0] + 11.25f, accel, 16, 22.5f);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }

        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1_A2() {
        float delay;

        while (true) {
            m_DirectionDelta[0] = -Random.Range(10f, 50f) * Mathf.Sign(m_DirectionDelta[0]);
            delay = Random.Range(2f, 5f);
            yield return new WaitForSeconds(delay);
        }
    }



    private IEnumerator Pattern1_B1() {
        EnemyBulletAccel accel1 = new EnemyBulletAccel(3f, 1f), accel2 = new EnemyBulletAccel(6f, 0.4f);
        float dir;

        while (true) {
            dir = GetAngleToTarget(transform.position, m_PlayerPosition);
            for (int i = 0; i < 3; i++) {
                CreateBullet(2, transform.position, 6f, dir + Random.Range(-45f, 45f), accel1, BulletType.CREATE, 0.6f,
                5, 3f, BulletDirection.CURRENT, 0f, accel2);
            }
            yield return new WaitForSeconds(0.08f);
        }
    }

    private IEnumerator Pattern1_B2() {
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, 1f), accel2;

        for (int i = 0; i < 10; i++) {
            accel2 = new EnemyBulletAccel(7f+i*0.86f, 0.4f);
            CreateBulletsSector(0, transform.position, 10f, 0f, accel1, 4, 90f, BulletType.ERASE_AND_CREATE, 0.5f,
            0, 0.1f, BulletDirection.PLAYER, 0f, accel2);
            yield return new WaitForSeconds(0.06f);
        }
        yield break;
    }



    private IEnumerator Pattern1_C1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        m_Direction[0] = Random.Range(0f, 360f);
        m_DirectionDelta[0] = 29f;

        while (true) {
            float dir = m_Direction[0]*m_DirectionSide;
            CreateBulletsSector(3, transform.position, 7.2f, dir, accel, 10, 36f);
            CreateBulletsSector(3, transform.position, 7.2f, dir - 11f, accel, 10, 36f);
            yield return new WaitForSeconds(0.14f);
        }
    }

    private IEnumerator Pattern1_C2() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        m_Direction[1] = Random.Range(0f, 360f);
        m_DirectionDelta[1] = 67f;

        while (true) {
            float dir = m_Direction[1]*m_DirectionSide;
            CreateBulletsSector(0, transform.position, 9f, dir, accel, 9, 40f);
            yield return new WaitForSeconds(0.11f);
        }
    }



    private IEnumerator Pattern1_D1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        m_Direction[0] = Random.Range(0f, 360f);
        m_Direction[1] = Random.Range(0f, 360f);
        m_DirectionDelta[0] = 23f*m_DirectionSide;
        m_DirectionDelta[1] = 43f;

        while (true) {
            CreateBulletsSector(3, transform.position, 5.3f, m_Direction[0], accel, 4, 90f, BulletType.ERASE_AND_CREATE, 0.6f,
            4, 6.4f, BulletDirection.FIXED, m_Direction[1], accel, 6, 60f);
            yield return new WaitForSeconds(0.15f);
        }
    }

    private IEnumerator Pattern1_D2() {
        yield return new WaitForSeconds(5f);
        DOTween.To(()=>m_DirectionDelta[1], x=>m_DirectionDelta[1] = x, -43f, 2.7f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(4f);
        DOTween.To(()=>m_DirectionDelta[1], x=>m_DirectionDelta[1] = x, 43f, 2.7f).SetEase(Ease.Linear);
        yield break;
    }



    private IEnumerator Pattern1_E1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        m_Direction[0] = Random.Range(0f, 360f);
        m_DirectionDelta[0] = -79f*m_DirectionSide;

        while (true) {
            for (int i = 0; i < 6; i++)
                CreateBulletsSector(1, transform.position, 4f+i*0.9f, m_Direction[0], accel, 4, 90f);
            yield return new WaitForSeconds(0.12f);
        }
    }

    private IEnumerator Pattern1_E2() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float dir;

        while (true) {
            dir = GetAngleToTarget(transform.position, m_PlayerPosition);
            CreateBulletsSector(0, transform.position, 7.5f, dir, accel, 10, 36f, BulletType.ERASE_AND_CREATE, 0.7f,
            4, 6.4f, BulletDirection.PLAYER, 0f, accel);
            yield return new WaitForSeconds(1f);
        }
    }

    
    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForSeconds(2f);
        m_CurrentPattern[0] = Pattern2_A();
        StartCoroutine(m_CurrentPattern[0]);

        while(m_InPattern) {
            yield return null;
        }
        StopAllPatterns();
        yield return new WaitForSeconds(5f);
        m_CurrentPattern[0] = PatternFinal1_1();
        StartCoroutine(m_CurrentPattern[0]);
        yield return new WaitForSeconds(4f);
        m_CurrentPattern[4] = PatternFinal2();
        StartCoroutine(m_CurrentPattern[4]);
        yield break;
    }



    private IEnumerator Pattern2_A() {
        EnemyBulletAccel accel1 = new EnemyBulletAccel(1f, 1f), accel2 = new EnemyBulletAccel(9.2f, 0.5f), accel3 = new EnemyBulletAccel(0f, 0f);
        m_DirectionDelta[0] = 31f;
        m_InPattern = true;

        m_Direction[0] = Random.Range(0f, 360f);
        for (int i = 0; i < 3; i++) {
            CreateBulletsSector(0, transform.position, 8f, m_Direction[0], accel1, 30, 12f, BulletType.ERASE_AND_CREATE, 1f,
            0, 1f, BulletDirection.CURRENT, 30f, accel2);
            yield return new WaitForSeconds(0.16f);
        }
        yield return new WaitForSeconds(0.4f);

        m_Direction[0] = Random.Range(0f, 360f);
        for (int i = 0; i < 5; i++) {
            CreateBulletsSector(3, transform.position, 8f, -m_Direction[0], accel1, 30, 12f, BulletType.ERASE_AND_CREATE, 1f,
            3, 1f, BulletDirection.CURRENT, -60f, accel2);
            yield return new WaitForSeconds(0.16f);
        }
        yield return new WaitForSeconds(0.4f);

        m_Direction[0] = Random.Range(0f, 360f);
        for (int i = 0; i < 7; i++) {
            CreateBulletsSector(0, transform.position, 8f, -m_Direction[0], accel1, 30, 12f, BulletType.ERASE_AND_CREATE, 1f,
            0, 1f, BulletDirection.CURRENT, 90f, accel2);
            yield return new WaitForSeconds(0.16f);
        }
        yield return new WaitForSeconds(1.4f);

        m_Direction[0] = Random.Range(0f, 360f);
        float dir = 0f;
        for (int i = 0; i < 9; i++) {
            CreateBulletsSector(3, transform.position, 10f, dir + Random.Range(3.5f, 6.5f), accel3, 36, 10f);
            //CreateBulletsSector(3, transform.position, 8f, -m_Direction[0], accel1, 30, 12f, BulletType.ERASE_AND_CREATE, 1f,
            //3, 1f, BulletDirection.CURRENT, -140f, accel2);
            yield return new WaitForSeconds(0.17f);
        }
        m_InPattern = false;
    }



    private IEnumerator PatternFinal1_1() {
        float bullet_delay = 1.1f, max_speed = 10f, min_speed = 5.6f;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, bullet_delay), accel2 = new EnemyBulletAccel(0f, 0f);
        m_Direction[0] = Random.Range(0f, 360f);
        m_Direction[1] = Random.Range(0f, 360f);
        m_DirectionDelta[0] = 109f;
        m_DirectionDelta[1] = 73f;
        m_BulletSpeed = max_speed;

        m_CurrentPattern[1] = PatternFinal1_2(m_DirectionDelta[0]);
        StartCoroutine(m_CurrentPattern[1]);

        m_CurrentPattern[2] = PatternFinal1_3(m_DirectionDelta[1]);
        StartCoroutine(m_CurrentPattern[2]);

        m_CurrentPattern[3] = PatternFinal1_4(min_speed, max_speed);
        StartCoroutine(m_CurrentPattern[3]);

        while (true) {
            CreateBulletsSector(0, transform.position, m_BulletSpeed, m_Direction[0], accel1, 3, 120f, BulletType.ERASE_AND_CREATE, bullet_delay,
            3, 7.5f, BulletDirection.CURRENT, m_Direction[1], accel2, 2, 48f);
            CreateBulletsSector(0, transform.position, m_BulletSpeed, m_Direction[0], accel1, 3, 120f, BulletType.ERASE_AND_CREATE, bullet_delay,
            5, 5.4f, BulletDirection.CURRENT, m_Direction[1] + 180f, accel2);
            yield return new WaitForSeconds(0.062f);
        }
    }

    private IEnumerator PatternFinal1_2(float max_rotate_speed) {
        while (true) {
            yield return new WaitForSeconds(Random.Range(8f, 10f));
            DOTween.To(()=>m_DirectionDelta[0], x=>m_DirectionDelta[0] = x, -max_rotate_speed, 2.4f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(Random.Range(8f, 10f));
            DOTween.To(()=>m_DirectionDelta[0], x=>m_DirectionDelta[0] = x, max_rotate_speed, 2.4f).SetEase(Ease.Linear);
            yield break;
        }
    }

    private IEnumerator PatternFinal1_3(float max_rotate_speed) {
        while (true) {
            yield return new WaitForSeconds(Random.Range(7f, 9f));
            DOTween.To(()=>m_DirectionDelta[1], x=>m_DirectionDelta[1] = x, -max_rotate_speed, 1.7f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(Random.Range(7f, 9f));
            DOTween.To(()=>m_DirectionDelta[1], x=>m_DirectionDelta[1] = x, max_rotate_speed, 1.7f).SetEase(Ease.Linear);
            yield break;
        }
    }

    private IEnumerator PatternFinal1_4(float min_speed, float max_speed) {
        while (true) {
            yield return new WaitForSeconds(Random.Range(5f, 8f));
            DOTween.To(()=>m_BulletSpeed, x=>m_BulletSpeed = x, min_speed, 2f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(Random.Range(5f, 8f));
            DOTween.To(()=>m_BulletSpeed, x=>m_BulletSpeed = x, max_speed, 2f).SetEase(Ease.InOutQuad);
            yield break;
        }
    }

    private IEnumerator PatternFinal2() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float dir = GetAngleToTarget(transform.position, m_PlayerPosition);
        while (true) {
            CreateBulletsSector(1, transform.position, 5f, dir + Random.Range(-2f, 2f), accel, 26, 13.8461f);
            yield return new WaitForSeconds(0.4f);
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
        ExplosionEffect(2, -1);
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(0.7f, 0f);

        StartCoroutine(DeathExplosion1());
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(DeathExplosion2(3.6f));
        StartCoroutine(DeathExplosion3(3.6f));
        StartCoroutine(DeathExplosion4(3.6f));
        
        yield return new WaitForSeconds(4f);
        
        ExplosionEffect(2, 3); // 최종 파괴
        ExplosionEffect(2, -1, new Vector2(-4f, 3f), new MoveVector(2f, 126.87f));
        ExplosionEffect(2, -1, new Vector2(4f, 3f), new MoveVector(2f, -126.87f));
        ExplosionEffect(2, -1, new Vector2(0f, 3f), new MoveVector(1.2f, 0f));
        ExplosionEffect(1, -1, new Vector2(-1.5f, 2.4f) + Random.insideUnitCircle*0.5f, new MoveVector(0.6f, Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(1.5f, 2.4f) + Random.insideUnitCircle*0.5f, new MoveVector(0.6f, Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(-3.5f, 0.4f) + Random.insideUnitCircle*0.5f, new MoveVector(0.6f, Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(3.5f, 0.4f) + Random.insideUnitCircle*0.5f, new MoveVector(0.6f, Random.Range(0f, 360f)));
        m_SystemManager.ScreenEffect(1);
        m_SystemManager.ShakeCamera(2f);

        CreateItems();
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1() {
        while (true) {
            ExplosionEffect(1, -1, Random.insideUnitCircle, new MoveVector(5f, Random.Range(160f, 200f)));
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.35f);
            ExplosionEffect(0, 0, Random.insideUnitCircle * 2f, new MoveVector(Random.Range(2f, 3.5f), Random.Range(0f, 360f)));
            ExplosionEffect(1, -1, Random.insideUnitCircle * 5f, new MoveVector(Random.Range(1f, 2f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion3(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.4f);
            ExplosionEffect(2, 1, Random.insideUnitCircle * 2f, new MoveVector(Random.Range(2f, 3.5f), Random.Range(0f, 360f)));
            ExplosionEffect(2, -1, Random.insideUnitCircle * 5f, new MoveVector(Random.Range(0f, 1f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion4(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            t_add = Random.Range(0.1f, 0.5f);
            ExplosionEffect(1, 2, Random.insideUnitCircle * 4f, new MoveVector(Random.Range(1f, 2f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }
}
