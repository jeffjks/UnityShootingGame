using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBossTrue : EnemyUnit
{
    public GameObject m_BombBarrier;
    public Transform m_Core;
    [HideInInspector] public sbyte m_Phase;
    [HideInInspector] public float m_Direction;
    
    private Vector3 m_TargetPosition;
    private float m_AppearanceTime = 2f;

    private IEnumerator m_CurrentPhase;
    private IEnumerator[] m_CurrentPattern = new IEnumerator[3];
    private Vector3 m_RotateAxis = new Vector2(1f, 1f);
    private float m_RotateAngle, m_RotateAxisAngle = 45f;
    //private int m_RotateAxisSide;

    void Start()
    {
        DisableAttackable(m_AppearanceTime);
        m_TargetPosition = new Vector3(0f, -3.8f, Depth.ENEMY);
        //m_RotateAxisSide = 2*Random.Range(0, 2) - 1;
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMove(m_TargetPosition, m_AppearanceTime).SetEase(Ease.InQuad));

        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase > 0) {
            if (transform.position.x >= m_TargetPosition.x + 1.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
            }
            else if (transform.position.x <= m_TargetPosition.x - 1.5f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
            }
            else if (transform.position.y >= m_TargetPosition.y + 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
            }
            else if (transform.position.y <= m_TargetPosition.y - 0.4f) {
                m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
            }
        }
        
        Rotate();
        BombBarrier();

        if (m_Direction > 360f)
            m_Direction -= 360f;
        else if (m_Direction < 0f)
            m_Direction += 360f;
            
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
        yield return new WaitForSeconds(2f);

        while (m_Phase == 1) {
            yield return new WaitForSeconds(2f);
            /*
            m_CurrentPattern[0] = Pattern1_A1();
            StartCoroutine(m_CurrentPattern[0]);
            m_CurrentPattern[1] = Pattern1_A2();
            StartCoroutine(m_CurrentPattern[1]);*/
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
            pos = transform.position;
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
    

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForSeconds(3f);

        while (m_Phase == 2) {
            yield return new WaitForSeconds(2f);
        }
        yield break;
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
