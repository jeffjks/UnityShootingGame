using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyPlaneMedium5 : EnemyUnit
{
    [HideInInspector] public byte m_State;
    private float m_AppearanceTime = 3f;
    private sbyte m_Side;

    void Start ()
    {
        float time_limit = 8.5f;
        
        if (transform.position.x < 0)
            m_Side = -1;
        else
            m_Side = 1;

        m_MoveVector = new MoveVector(7f, -72f * m_Side);
        
        m_Sequence = DOTween.Sequence();
        m_Sequence.AppendInterval(2.2f);
        DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 0f, m_AppearanceTime).SetEase(Ease.OutQuad);

        Invoke("Appearance", m_AppearanceTime);
        Invoke("TimeLimit", m_AppearanceTime + time_limit);
    }

    protected override void FixedUpdate()
    {
        RotateImmediately(m_MoveVector.direction);

        if (m_State == 2)
            Dissappearance();
        
        
        base.FixedUpdate();
    }

    private void Dissappearance() {
        if (m_MoveVector.speed < 6.4f)
            m_MoveVector.speed += 4.4f * Time.fixedDeltaTime;
        else
            m_MoveVector.speed = 6.4f;

        if (Mathf.Abs(m_MoveVector.direction) < 96f)
            m_MoveVector.direction += -18f * m_Side * Time.fixedDeltaTime;
        else
            m_MoveVector.direction = -96f * m_Side;
    }

    private void Appearance() {
        m_State = 1;
    }

    private void TimeLimit() {
        m_State = 2;
        DOTween.Kill(m_Sequence);
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector2(0f, 1.5f));
        ExplosionEffect(0, -1, new Vector2(0f, -2f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
