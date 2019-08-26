using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMiddleBoss2 : EnemyUnit
{
    public EnemyMiddleBoss2Turret0 m_Turret0;
    public EnemyMiddleBoss2Turret1[] m_Turret1 = new EnemyMiddleBoss2Turret1[2];
    [HideInInspector] public byte m_Phase = 0;
    
    private Vector3 m_TargetPosition;
    private Quaternion m_TargetQuaternion;
    private bool m_TimeLimitState = false;

    private IEnumerator m_Pattern1, m_Pattern2;

    void Start()
    {
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

        Destroy(gameObject, 28f);
    }


    protected override void Update()
    {
        if (m_Phase == 0) {
            if (m_Health <= m_MaxHealth * 0.375f) { // 체력 37.5% 이하
                ToPhase1();
            }
        }

        RotateImmediately(m_MoveVector.direction);

        base.Update();
    }

    public void ToPhase1() {
        if (m_Phase == 1)
            return;
        m_Phase = 1;

        if (m_Turret0 != null)
            m_Turret0.OnDeath();
        if (m_Turret1[0] != null)
            m_Turret1[0].OnDeath();
        if (m_Turret1[1] != null)
            m_Turret1[1].OnDeath();
        
        m_Collider2D[0].gameObject.SetActive(true);
        m_SystemManager.EraseBullets(1f);
    }


    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector.speed = 0f;

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
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
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
        Vector2 random_pos;
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
