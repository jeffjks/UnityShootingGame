using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : PlayerObject
{
    public PlayerUnit m_PlayerUnit;
    public PlayerDamageDatas[] m_PlayerLaserDamageData;
    public TriggerBody m_TriggerBody;

    private PlayerLaserHandler _playerLaserHandler;
    //private readonly List<EnemyUnit> _enemyList = new();

    private void Start()
    {
        _playerLaserHandler = GetComponent<PlayerLaserHandler>();
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;
        
        _playerLaserHandler.Action_OnLaserIndexChanged += UpdateLaserIndex;
        _playerLaserHandler.Action_OnStopLaser += RemoveAllTickDamageContext;
        m_PlayerUnit.Action_OnUpdatePlayerAttackLevel += UpdatePlayerAttackLevel;
    }

    /*
    private void LateUpdate()
    {
        if (PauseManager.IsGamePaused)
            return;
        
        DealLaserDamage();
    }

    private void DealLaserDamage()
    {
        if (_enemyList.Count == 0)
            return;
        if (m_PlayerUnit.SlowMode == false)
            return;
        if (PauseManager.IsGamePaused)
            return;

        for (var i = _enemyList.Count - 1; i >= 0; --i)
        {
            DealDamage(_enemyList[i]);
            HitCountController.Instance.HitCountLaserCounter++;
        }
    }
    */

    public void OnEnable()
    {
        SimulationManager.AddTriggerBody(m_TriggerBody);
        m_TriggerBody.m_OnTriggerBodyEnter += OnTriggerBodyEnter;
        m_TriggerBody.m_OnTriggerBodyExit += OnTriggerBodyExit;
        //m_TriggerBody.m_OnTriggerBodyStay += OnTriggerBodyStay;
    }

    public void OnDisable()
    {
        SimulationManager.RemoveTriggerBody(m_TriggerBody);
        m_TriggerBody.m_OnTriggerBodyEnter -= OnTriggerBodyEnter;
        m_TriggerBody.m_OnTriggerBodyExit -= OnTriggerBodyExit;
        //m_TriggerBody.m_OnTriggerBodyStay -= OnTriggerBodyStay;
    }

    private void OnTriggerBodyEnter(TriggerBody other) // 충돌 감지
    {
        if (_playerLaserHandler.IsLaserShooting == false)
            return;
        if (other.m_TriggerBodyType != TriggerBodyType.Enemy)
            return;
        
        var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
            
        if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
        {
            var enemyHealth = enemyUnit.m_EnemyHealth;
            var damageScale = _playerDamageData.damageScale[enemyUnit.m_EnemyType];
            var damageType = _playerDamageData.playerDamageType;
            var tickDamageContext = new TickDamageContext(Damage, damageScale, damageType);
            enemyHealth.AddTickDamageContext(m_ObjectName, tickDamageContext);
        }
        else // 소형이면
        {
            enemyUnit.m_EnemyDeath.KillEnemy();
            HitCountController.Instance.AddHitCount();
        }
    }

    private void OnTriggerBodyExit(TriggerBody other) // 충돌 감지
    {
        if (other.m_TriggerBodyType != TriggerBodyType.Enemy)
            return;
        
        var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
            
        if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
        {
            var enemyHealth = enemyUnit.m_EnemyHealth;
            enemyHealth.RemoveTickDamageContext(m_ObjectName);
        }
    }

    private void RemoveAllTickDamageContext()
    {
        foreach (var triggerBody in m_TriggerBody.TriggerBodySet)
        {
            OnTriggerBodyExit(triggerBody);
        }
    }

    // private void OnTriggerStay2D(Collider2D other) // 충돌 감지
    // {
    //     if (m_PlayerUnit.SlowMode == false)
    //         return;
    //     
    //     if (other.gameObject.CompareTag("Enemy"))
    //     {
    //         var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
    //         
    //         if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
    //         {
    //             DealDamage(enemyUnit);
    //             HitCountController.Instance.HitCountLaserCounter++;
    //         }
    //         else // 소형이면
    //         {
    //             if (enemyUnit.m_EnemyDeath.IsDead)
    //                 return;
    //             enemyUnit.m_EnemyDeath.KillEnemy();
    //             HitCountController.Instance.AddHitCount();
    //         }
    //     }
    // }

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