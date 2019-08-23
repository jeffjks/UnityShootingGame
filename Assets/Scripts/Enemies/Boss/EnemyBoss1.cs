using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss1 : EnemyUnit
{
    private Vector3[] m_TargetPosition = new Vector3[2];
    private Quaternion[] m_TargetQuaternion = new Quaternion[2];
    private float m_AppearanceTime = 2f;
    private byte m_Phase = 0;

    private IEnumerator m_Pattern1, m_Pattern2;

    void Start()
    {
        DisableAttackable(m_AppearanceTime);

        m_UpdateTransform = false;
        m_TargetPosition[0] = new Vector3(4f, -1f, Depth.ENEMY);
        m_TargetPosition[1] = new Vector3(0f, -5f, Depth.ENEMY);

        transform.rotation = Quaternion.Euler(0f, -35f, 0f);
        m_TargetQuaternion[0] = Quaternion.Euler(0f, 20f, 0f);
        m_TargetQuaternion[1] = Quaternion.identity;

        float appearance_time_1 = 0.55f;
        float appearance_time_2 = 1f - 0.55f;
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveX(3f, m_AppearanceTime*appearance_time_1).SetEase(Ease.OutQuad))
        .Join(transform.DOMoveY(-2f, m_AppearanceTime*appearance_time_1).SetEase(Ease.Linear))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[0], m_AppearanceTime*appearance_time_1).SetEase(Ease.InOutQuad))
        .Append(transform.DOMoveX(0f, m_AppearanceTime*appearance_time_2).SetEase(Ease.InOutQuad))
        .Join(transform.DOMoveY(-4.5f, m_AppearanceTime*appearance_time_2).SetEase(Ease.OutQuad))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[1], m_AppearanceTime*appearance_time_2).SetEase(Ease.InQuad));
        //.Append(DOTween.To(()=>transform.position, x=>transform.position = x, m_TargetPosition[1], 1.5f).SetEase(Ease.InQuad));
        //.Append(transform.DOMove(m_TargetPosition, m_AppearanceTime).SetEase(Ease.OutQuad))
        /*
        .Append(transform.DOMove(m_TargetPosition[0], m_AppearanceTime - 1.5f).SetEase(Ease.OutQuad))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[0], m_AppearanceTime - 1.5f).SetEase(Ease.InQuad))
        .Append(transform.DOMove(m_TargetPosition[0], m_AppearanceTime - 1.5f).SetEase(Ease.OutQuad))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[0], m_AppearanceTime - 1.5f).SetEase(Ease.InQuad));*/
        
        // m_Pattern1 = Pattern1(m_SystemManager.m_Difficulty);
        //StartCoroutine(m_Pattern1);

        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (m_Phase == 0) {
            if (m_Health <= m_MaxHealth * 0.4f) { // 체력 40% 이하
                m_SystemManager.BulletsToGems(1f);
                m_MoveVector.speed = 0f;
                m_Phase = 1;
                StopCoroutine(m_Pattern1);
                // m_Pattern2 = Pattern2();
                ExplosionEffect(2, -1, new Vector2(2f, 0f));
                ExplosionEffect(2, 1, new Vector2(-2f, 0f));
            }
            
            if (m_IsAttackable) {
                if (transform.position.x >= m_TargetPosition[1].x + 2f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.left));
                }
                else if (transform.position.x <= m_TargetPosition[1].x - 2f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.right));
                }
                else if (transform.position.y >= m_TargetPosition[1].y + 0.6f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.down));
                }
                else if (transform.position.y <= m_TargetPosition[1].y - 0.6f) {
                    m_MoveVector = new MoveVector(Vector2.Reflect(m_MoveVector.GetVector(), Vector2.up));
                }
            }
        }

        base.Update();
    }

    private void OnAppearanceComplete() {
        m_Sequence.Kill();
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveX(-1f, 2f).SetEase(Ease.Linear))
        .Append(transform.DOMoveX(1f, 4f).SetEase(Ease.Linear))
        .Append(transform.DOMoveX(0f, 2f).SetEase(Ease.Linear))
        .SetLoops(-1, LoopType.Restart);
    }


    

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(1f, 0f);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(DeathExplosion1());
        StartCoroutine(DeathExplosion2());

        yield return new WaitForSeconds(3f);
        ExplosionEffect(2, 2); // 최종 파괴
        ExplosionEffect(0, 0, new Vector2(3f, 0f));
        ExplosionEffect(0, 0, new Vector2(-3f, 0f));
        ExplosionEffect(0, 0, new Vector2(0f, 2f));
        ExplosionEffect(0, 0, new Vector2(0f, -1.5f));
        m_SystemManager.ScreenEffect(1);
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator DeathExplosion1() {
        float timer = 0f, random_timer = 0f;
        Vector2 random_pos;
        while (timer < 1.5f) {
            random_timer = Random.Range(0.2f, 0.5f);
            random_pos = (Vector2) Random.insideUnitCircle * 3;
            ExplosionEffect(0, 0, random_pos);
            ExplosionEffect(0, -1, random_pos);
            yield return new WaitForSeconds(random_timer);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion2() {
        float timer = 0f, random_timer = 0f;
        Vector2 random_pos;
        while (timer < 1.5f) {
            random_timer = Random.Range(0.4f, 0.7f);
            random_pos = (Vector2) Random.insideUnitCircle * 3;
            ExplosionEffect(1, 1, random_pos);
            ExplosionEffect(1, -1, random_pos);
            yield return new WaitForSeconds(random_timer);
        }
        yield return null;
    }
}
