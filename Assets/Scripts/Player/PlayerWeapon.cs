using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeapon : PlayerObject, IObjectPooling, IRotatable
{
    [SerializeField] protected float m_Speed;
    [SerializeField] private bool m_IsPenetrate;
    [SerializeField] private GameObject[] m_ActivatedObject;
    [Header("-1 : None / 0 : Shot(fire) / 1 : Homing(purple) / 2 : Rocket(metal)")]
    [SerializeField] private int m_HitEffectNumber = -1; // TODO. Enum화

    private int _currentForm;
    
    private bool _appliedDamage;
    
    public virtual void OnStart() {
        /*
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
        }*/
        _appliedDamage = false;
    }

    protected override void OnDamageLevelChanged()
    {
        base.OnDamageLevelChanged();
        if (m_ActivatedObject.Length == 0)
        {
            return;
        }
        m_ActivatedObject[_currentForm].SetActive(false);
        _currentForm = _damageLevel * (m_ActivatedObject.Length - 1) / _maxDamageLevel;
        m_ActivatedObject[_currentForm].SetActive(true);
    }

    public void SetPosition2D() { // m_Position2D 변수의 좌표를 계산
        m_Position2D = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_appliedDamage) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
            
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                if (m_IsPenetrate) { // 관통 공격이며
                    if (Utility.CheckLayer(other.gameObject, Layer.SMALL)) { // 적이 소형이면
                        enemyObject.m_EnemyDeath.OnDying(); // 기냥 죽임
                    }
                }
                else { // 그 이외의 경우에는
                    DealDamage(enemyObject); // 데미지 주고
                    _appliedDamage = true;
                    OnDeath(); // 자신 파괴
                }
            }
        }
    }

    private void OnDeath() {
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
        if (m_ActivatedObject.Length > 0)
        {
            m_ActivatedObject[_currentForm].SetActive(false);
        }
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