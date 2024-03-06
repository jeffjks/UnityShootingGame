using System;
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
    // private static int _playerWeaponIndex;
    // private int _index;

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
    
    public virtual void OnStart()
    {
#if UNITY_EDITOR
        if (SystemManager.IsInGame && ReplayManager.PlayerWeaponStartLog)
            ReplayManager.WriteReplayLogFile($"PlayerWeaponStart {name}: {transform.position.ToString("N6")}");
#endif
        // _index = _playerWeaponIndex;
        // _playerWeaponIndex++;
        // if (_playerWeaponIndex >= Int32.MaxValue)
        //     _playerWeaponIndex = 0;
        
        _appliedDamage = false;
        _removeTimerCoroutine = SetRemoveTimer();
        //Debug.Log($"{ReplayManager.CurrentFrame}: PlayerWeapon Spawned {name} at {transform.position}");
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

    /*
    private void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        // if (SystemManager.GameMode == GameMode.Replay)
        //     return;
        // if (_appliedDamage)
        //     return;
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();

            var targetId = enemyUnit.EnemyUnitId;
            ReplayManager.WriteReplayCollisionData(EnemyId, targetId);

            TriggerEnter(enemyUnit);
        }
    }

    public override void ExecuteCollisionEnter(int id)
    {
        var enemyUnit = EnemyIdList[id] as EnemyUnit;

        if (enemyUnit == null)
        {
            Debug.LogError($"{EnemyIdList[id].GetType()} (id: {id}) can not cast to EnemyUnit!");
            return;
        }

        TriggerEnter(enemyUnit);
    }
    */

    private void TriggerEnter(EnemyUnit enemyUnit)
    {
#if UNITY_EDITOR
        if (ReplayManager.PlayerWeaponHitLog)
            ReplayManager.WriteReplayLogFile($"PlayerWeaponHit {name}->{enemyUnit.name}, {transform.position.ToString("N6")}");
#endif
            
        if (m_IsPenetrate && gameObject.CheckLayer(Layer.SMALL))
        {
            enemyUnit.m_EnemyDeath.KillEnemy();
        }
        else // 관통 공격이 아니며 적이 소형이 아닌 경우에는 데미지 주고 자신 파괴
        {
            DealDamage(enemyUnit);
            _appliedDamage = true;
            OnDeath();
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