using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeapon : PlayerObject, IObjectPooling
{
    [SerializeField] protected float m_Speed;
    [SerializeField] private bool m_IsPenetrate;
    [SerializeField] private GameObject[] m_ActivatedObject;
    [SerializeField] private HitEffectType _hitEffectType;

    public override float CurrentAngle
    {
        get => _currentAngle;
        set
        {
            _currentAngle = value;
            OnCurrentAngleChanged(_currentAngle);
        }
    }

    private int _currentForm;
    
    private bool _appliedDamage;
    
    public virtual void OnStart() {
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
        if (_hitEffectType != HitEffectType.None) {
            GameObject obj = PoolingManager.PopFromPool("PlayerHitEffect", PoolingParent.Explosion); // 히트 이펙트
            HitEffect hitEffect = obj.GetComponent<HitEffect>();
            hitEffect.SetHitEffectType(_hitEffectType);
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

    /*
    public new void RotateSlightly(Vector2 target, float speed, float rot = 0f) {
        float target_angle = GetAngleToTarget(m_Position2D, target);
        CurrentAngle = Mathf.MoveTowardsAngle(CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
    }

    public new void RotateSlightly(float target_angle, float speed, float rot = 0f) {
        CurrentAngle = Mathf.MoveTowardsAngle(CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
    }

    public new void RotateImmediately(Vector2 target, float rot = 0f) {
        float target_angle = GetAngleToTarget(m_Position2D, target);
        CurrentAngle = target_angle + rot;
    }

    public new void RotateImmediately(float target_angle, float rot = 0f) {
        CurrentAngle = target_angle + rot;
    }*/

    private void OnCurrentAngleChanged(float value)
    {
        if (CurrentAngle > 360f) {
            CurrentAngle -= 360f;
        }
        else if (CurrentAngle < 0f) {
            CurrentAngle += 360f;
        }

        transform.rotation = Quaternion.AngleAxis(CurrentAngle, Vector3.forward); // Vector3.forward
    }
}