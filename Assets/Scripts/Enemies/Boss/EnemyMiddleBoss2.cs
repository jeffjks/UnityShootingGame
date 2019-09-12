using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMiddleBoss2 : EnemyUnit
{
    public Transform m_FirePosition0;
    public Transform[] m_FirePosition2 = new Transform[2];
    public EnemyMiddleBoss2Turret0 m_Turret0;
    public EnemyMiddleBoss2Turret1[] m_Turret1 = new EnemyMiddleBoss2Turret1[2];
    [HideInInspector] public sbyte m_Phase;
    
    private float m_Direction;

    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        m_Phase = 1;
        m_MoveVector = new MoveVector(3f, 120f);

        m_Sequence = DOTween.Sequence()
        .AppendInterval(4f)
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 0f, 1f).SetEase(Ease.OutQuad)) // stop
        .AppendInterval(2f)
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 2f, 1f).SetEase(Ease.InQuad))
        .AppendInterval(1f)
        .Append(DOTween.To(()=>m_MoveVector.direction, x=>m_MoveVector.direction = x, 220f, 3.5f).SetEase(Ease.Linear))
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 0f, 1f).SetEase(Ease.OutQuad)) // stop
        .AppendInterval(0.5f)
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 2f, 1f).SetEase(Ease.InQuad))
        .Append(DOTween.To(()=>m_MoveVector.direction, x=>m_MoveVector.direction = x, 170f, 2.5f).SetEase(Ease.Linear))
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 0f, 1f).SetEase(Ease.OutQuad)) // stop
        .AppendInterval(1f)
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 2f, 1.5f).SetEase(Ease.InQuad))
        .Append(DOTween.To(()=>m_MoveVector.direction, x=>m_MoveVector.direction = x, 240f, 2.5f).SetEase(Ease.Linear));

        m_CurrentPattern1 = Pattern1A();
        StartCoroutine(m_CurrentPattern1);

        Destroy(gameObject, 30f);
    }


    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.375f) { // 체력 37.5% 이하
                ToNextPhase();
            }
        }

        m_Direction += 200f * Time.deltaTime;
        if (m_Direction >= 360f)
            m_Direction -= 360f;

        RotateImmediately(m_MoveVector.direction);

        base.Update();
    }

    public void ToNextPhase() {
        if (m_Phase == 2)
            return;
        m_Phase = 2;
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);

        m_CurrentPattern1 = Pattern2A();
        m_CurrentPattern2 = Pattern2B();
        StartCoroutine(m_CurrentPattern1);
        StartCoroutine(m_CurrentPattern2);

        if (m_Turret0 != null)
            m_Turret0.OnDeath();
        if (m_Turret1[0] != null)
            m_Turret1[0].OnDeath();
        if (m_Turret1[1] != null)
            m_Turret1[1].OnDeath();
        
        m_Collider2D[0].gameObject.SetActive(true);
        m_SystemManager.EraseBullets(0.5f);
    }


    private IEnumerator Pattern1A() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(2.5f);
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[0].position), 6.5f, Random.Range(0f, 360f), accel, 18, 20f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[0].position), 6.5f, Random.Range(0f, 360f), accel, 18, 20f);
                yield return new WaitForSeconds(2.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[1].position), 6.5f, Random.Range(0f, 360f), accel, 18, 20f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[1].position), 6.5f, Random.Range(0f, 360f), accel, 18, 20f);
                yield return new WaitForSeconds(2.5f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForSeconds(2f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForSeconds(2f);
            }
            else {
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForSeconds(1.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForSeconds(0.5f);
                CreateBulletsSector(2, GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForSeconds(1.5f);
            }
        }
    }

    private IEnumerator Pattern2A() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                CreateBullet(3, GetScreenPosition(m_FirePosition2[0].position), 6.4f, m_Direction, accel);
                CreateBullet(3, GetScreenPosition(m_FirePosition2[1].position), 6.4f, -m_Direction, accel);
                yield return new WaitForSeconds(0.11f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                CreateBulletsSector(5, GetScreenPosition(m_FirePosition2[0].position), 6.6f, m_Direction, accel, 2, 180f);
                CreateBulletsSector(5, GetScreenPosition(m_FirePosition2[1].position), 6.6f, -m_Direction, accel, 2, 180f);
                yield return new WaitForSeconds(0.07f);
            }
            else {
                CreateBulletsSector(5, GetScreenPosition(m_FirePosition2[0].position), 6.8f, m_Direction, accel, 2, 180f);
                CreateBulletsSector(5, GetScreenPosition(m_FirePosition2[1].position), 6.8f, -m_Direction, accel, 2, 180f);
                yield return new WaitForSeconds(0.04f);
            }
        }
    }

    private IEnumerator Pattern2B() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float random_value, target_angle;
        yield return new WaitForSeconds(1f);
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                random_value = Random.Range(0f, 360f);
                pos = GetScreenPosition(m_FirePosition0.position);
                target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                CreateBulletsSector(4, pos, 6f, random_value + target_angle, accel, 20, 18f);
                yield return new WaitForSeconds(1.8f);
            }
            else if (m_SystemManager.m_Difficulty == 1) {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 3; i++) {
                    pos = GetScreenPosition(m_FirePosition0.position);
                    target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                    CreateBulletsSector(4, pos, 6f + i*0.6f, random_value + target_angle, accel, 24, 15f);
                    yield return new WaitForSeconds(0.08f);
                }
                yield return new WaitForSeconds(0.9f);
            }
            else {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 3; i++) {
                    pos = GetScreenPosition(m_FirePosition0.position);
                    target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
                    CreateBulletsSector(4, pos, 6f + i*0.6f, random_value + target_angle, accel, 30, 12f);
                    yield return new WaitForSeconds(0.08f);
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }


    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector.speed = 0f;
        m_Phase = -1;

        StartCoroutine(DeathExplosion1(1.9f));
        StartCoroutine(DeathExplosion2(1.9f));

        yield return new WaitForSeconds(2f);
        ExplosionEffect(0, 0, new Vector2(-1f, 0f)); // 최종 파괴
        ExplosionEffect(0, -1, new Vector2(1f, 0f));
        ExplosionEffect(0, -1, new Vector3(-1f, 0f, 1.2f));
        ExplosionEffect(0, -1, new Vector3(1f, 0f, 1.2f));
        ExplosionEffect(0, -1, new Vector3(-1f, 0f, -1.2f));
        ExplosionEffect(0, -1, new Vector3(1f, 0f, -1.2f));
        m_SystemManager.ScreenEffect(0);
        
        CreateItems();
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector3 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.5f);
            random_pos = new Vector3(Random.Range(-1.6f, 1.6f), 2f, Random.Range(-1.8f, 1.8f));
            ExplosionEffect(1, 1, random_pos);
            random_pos = new Vector3(Random.Range(-1.6f, 1.6f), 2f, Random.Range(-1.8f, 1.8f));
            ExplosionEffect(1, -1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector3 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.4f, 0.7f);
            random_pos = new Vector3(Random.Range(-1.6f, 1.6f), 2f, Random.Range(-1.8f, 1.8f));
            ExplosionEffect(2, 2, random_pos);
            random_pos = new Vector3(Random.Range(-1.6f, 1.6f), 2f, Random.Range(-1.8f, 1.8f));
            ExplosionEffect(2, -1, random_pos);
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }
}
