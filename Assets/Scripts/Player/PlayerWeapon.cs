using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeapon : PlayerObject, IObjectPooling, IRotatable
{
    [SerializeField] private int[] m_DamageBonus = {0, 0, 0};
    [SerializeField] protected float m_Speed = 0;
    [SerializeField] private bool m_IsPenetrate = false;
    [SerializeField] private GameObject[] m_ActivatedObject = null;
    [Header("-1 : None / 0 : Shot(fire) / 1 : Homing(purple) / 2 : Rocket(metal)")]
    [SerializeField] private int m_HitEffectNumber = -1;

    [HideInInspector] public int m_DamageLevel;
    
    protected bool m_HasDamaged = false;
    
    public virtual void OnStart() {
        try {
            m_Damage = m_DefaultDamage + m_DamageBonus[m_DamageLevel];
        }
        catch {
            Debug.LogAssertion("Damage Level Index Out Of Bound: "+m_DamageLevel);
        }
        for(int i = 0; i < m_ActivatedObject.Length; i++) {
            m_ActivatedObject[i].SetActive(false);
        }
        if (m_ActivatedObject.Length > 0) {
            m_ActivatedObject[m_DamageLevel].SetActive(true);
        }
        m_HasDamaged = false;
    }

    public void SetPosition2D() { // m_Position2D 변수의 좌표를 계산
        m_Position2D = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_HasDamaged) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
            
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                if (m_IsPenetrate) { // 관통 공격이며
                    if (Utility.CheckLayer(other.gameObject, Layer.SMALL)) { // 적이 소형이면
                        enemyObject.m_EnemyDeath.OnDying(); // 기냥 죽임
                    }
                }
                else { // 그 이외의 경우에는
                    DealDamage(enemyObject, m_Damage); // 데미지 주고
                    m_HasDamaged = true;
                    PlayHitAnimation(); // 자신 파괴
                }
            }
        }
    }

    private void PlayHitAnimation() {
        if (m_HitEffectNumber != -1) {
            GameObject obj = PoolingManager.PopFromPool("PlayerHitEffect", PoolingParent.Explosion); // 히트 이펙트
            HitEffect hitEffect = obj.GetComponent<HitEffect>();
            hitEffect.m_HitEffectType = m_HitEffectNumber;
            hitEffect.transform.position = new Vector3(transform.position.x, transform.position.y, Depth.HIT_EFFECT);
            obj.SetActive(true);
            hitEffect.OnStart();
        }
        ReturnToPool();
    }

    public void ReturnToPool() {
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.PlayerMissile);
    }



    // IRotatable Methods ----------------------------------

    public void RotateSlightly(Vector2 target, float speed, float rot = 0f) {
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
        UpdateTransform();
    }

    public void RotateSlightly(float target_angle, float speed, float rot = 0f) {
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
        UpdateTransform();
    }

    public void RotateImmediately(Vector2 target, float rot = 0f) {
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = target_angle + rot;
        UpdateTransform();
    }

    public void RotateImmediately(float target_angle, float rot = 0f) {
        m_CurrentAngle = target_angle + rot;
        UpdateTransform();
    }

    public void UpdateTransform()
    {
        if (m_CurrentAngle > 360f) {
            m_CurrentAngle -= 360f;
        }
        else if (m_CurrentAngle < 0f) {
            m_CurrentAngle += 360f;
        }

        transform.rotation = Quaternion.AngleAxis(m_CurrentAngle, Vector3.forward); // Vector3.forward
    }
}