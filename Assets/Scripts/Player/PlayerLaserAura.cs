using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserAura : PlayerObject
{
    private PlayerUnit _playerUnit;
    private PlayerLaserHandler _playerLaserHandler;
    private readonly HashSet<EnemyUnit> _enemySet = new();

    private void Start()
    {
        _playerLaserHandler = GetComponentInParent<PlayerLaserHandler>();
        _playerUnit = GetComponentInParent<PlayerUnit>();
        DamageLevel = _playerUnit.PlayerAttackLevel;

        _playerLaserHandler.Action_OnLaserIndexChanged += UpdateLaserIndex;
        _playerUnit.Action_OnUpdatePlayerAttackLevel += () => DamageLevel = _playerUnit.PlayerAttackLevel;
    }

    private void LateUpdate()
    {
        if (PauseManager.IsGamePaused)
            return;
        
        DealLaserDamage();
    }

    private void DealLaserDamage()
    {
        if (_enemySet.Count == 0)
            return;
        if (_playerUnit.SlowMode == false)
            return;

        foreach (var enemyUnit in _enemySet)
        {
            DealDamage(enemyUnit);
            HitCountController.Instance.HitCountLaserCounter++;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (PauseManager.IsGamePaused)
            return;
        if (_playerUnit.SlowMode == false)
            return;
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
            TriggerEnter(enemyUnit);
        }
    }

    private void OnTriggerExit2D(Collider2D other) // 충돌 감지
    {
        if (PauseManager.IsGamePaused)
            return;
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
            TriggerExit(enemyUnit);
        }
    }

    public override void ExecuteCollisionEnter(int id)
    {
        var enemyUnit = ObjectIdList[id] as EnemyUnit;

        if (enemyUnit == null)
        {
            Debug.LogError($"{ObjectIdList[id].GetType()} (id: {id}) can not cast to EnemyUnit!");
            return;
        }

        TriggerEnter(enemyUnit);
    }

    public override void ExecuteCollisionExit(int id)
    {
        var enemyUnit = ObjectIdList[id] as EnemyUnit;

        if (enemyUnit == null)
        {
            Debug.LogError($"{ObjectIdList[id].GetType()} (id: {id}) can not cast to EnemyUnit!");
            return;
        }

        TriggerExit(enemyUnit);
    }

    private void TriggerEnter(EnemyUnit enemyUnit)
    {
        if (PauseManager.IsGamePaused)
            return;
        
        if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
        {
            _enemySet.Add(enemyUnit);
        }
        else // 소형이면
        {
            if (enemyUnit.m_EnemyDeath.KillEnemy())
                HitCountController.Instance.AddHitCount();
        }
    }

    private void TriggerExit(EnemyUnit enemyUnit)
    {
        if (PauseManager.IsGamePaused)
            return;
        
        if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
        {
            _enemySet.Remove(enemyUnit);
        }
    }

    private void UpdateLaserIndex()
    {
        DamageLevel = _playerUnit.PlayerAttackLevel;
    }
}
