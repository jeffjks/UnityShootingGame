using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : PlayerObject
{
    public PlayerUnit m_PlayerUnit;
    public PlayerDamageDatas[] m_PlayerLaserDamageData;

    private PlayerLaserHandler _playerLaserHandler;
    private readonly HashSet<EnemyUnit> _enemySet = new();

    private void Start()
    {
        _playerLaserHandler = GetComponent<PlayerLaserHandler>();
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;
        
        _playerLaserHandler.Action_OnLaserIndexChanged += UpdateLaserIndex;
        m_PlayerUnit.Action_OnUpdatePlayerAttackLevel += UpdatePlayerAttackLevel;
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
        if (m_PlayerUnit.SlowMode == false)
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
        if (m_PlayerUnit.SlowMode == false)
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
        _playerDamageData = m_PlayerLaserDamageData[_playerLaserHandler.LaserIndex];
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;
    }

    private void UpdatePlayerAttackLevel()
    {
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;
    }
}