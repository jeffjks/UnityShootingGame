using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerWeapon : PlayerObject, IObjectPooling
{
    [SerializeField] protected float m_Speed;
    [SerializeField] private bool m_IsPenetrate;
    [SerializeField] private GameObject[] _activatedObject;
    [SerializeField] private string _playerMissileHit;
    [SerializeField] private int _removeTimer;

    private IEnumerator _removeTimerCoroutine;

    public override float CurrentAngle
    {
        get => _currentAngle;
        set
        {
            _currentAngle = value;
            _currentAngle = Mathf.Repeat(_currentAngle, 360f);
            OnCurrentAngleChanged();
        }
    }

    private int _currentForm;
    private bool _appliedDamage;
    
    public virtual void OnStart() {
        _appliedDamage = false;
        _removeTimerCoroutine = SetRemoveTimer();
        StartCoroutine(_removeTimerCoroutine);
    }

    protected override void OnDamageLevelChanged()
    {
        base.OnDamageLevelChanged();
        if (_activatedObject.Length == 0)
        {
            return;
        }
        _activatedObject[_currentForm].SetActive(false);
        _currentForm = _damageLevel * (_activatedObject.Length - 1) / _maxDamageLevel;
        _activatedObject[_currentForm].SetActive(true);
    }

    protected void SetMissilePosition(Vector3 position)
    {
        _activatedObject[_currentForm].transform.position = position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_appliedDamage) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
            
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                if (m_IsPenetrate) { // 관통 공격이며
                    if (Utility.CheckLayer(other.gameObject, Layer.SMALL)) { // 적이 소형이면
                        enemyObject.m_EnemyDeath.KillEnemy(); // 기냥 죽임
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
        GameObject obj = PoolingManager.PopFromPool(_playerMissileHit, PoolingParent.Explosion); // 히트 이펙트
        PlayerMissileHitEffect hitEffect = obj.GetComponent<PlayerMissileHitEffect>();
        hitEffect.transform.position = new Vector3(transform.position.x, transform.position.y, Depth.HIT_EFFECT);
        obj.SetActive(true);
        hitEffect.OnStart();
        ReturnToPool();
    }

    public void ReturnToPool() {
        if (_activatedObject.Length > 0)
        {
            _activatedObject[_currentForm].SetActive(false);
        }
        if (_removeTimerCoroutine != null)
            StopCoroutine(_removeTimerCoroutine);
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.PlayerMissile);
    }

    private IEnumerator SetRemoveTimer()
    {
        yield return new WaitForMillisecondFrames(_removeTimer);
        ReturnToPool();
    }

    private void OnCurrentAngleChanged()
    {
        transform.rotation = Quaternion.AngleAxis(_currentAngle, Vector3.forward); // Vector3.forward
    }
}