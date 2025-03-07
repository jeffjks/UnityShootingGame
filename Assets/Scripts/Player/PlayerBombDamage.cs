﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombDamage : PlayerObject
{
    public int m_BombDuration;
    public TriggerBody m_TriggerBody;

    //private readonly List<EnemyUnit> _enemyList = new();
    private IEnumerator _bombDamageCoroutine;

    private void Start()
    {
        m_TriggerBody.m_BodyPolygon.m_BodyPolygonUnits.Clear();
        m_TriggerBody.m_BodyType = TriggerBody.BodyType.Polygon;
        
        var pointList = new List<Vector2>()
        {
            new (-Size.GAME_WIDTH / 2f, -Size.GAME_HEIGHT / 2f),
            new (-Size.GAME_WIDTH / 2f, Size.GAME_HEIGHT / 2f),
            new (Size.GAME_WIDTH / 2f, Size.GAME_HEIGHT / 2f),
            new (Size.GAME_WIDTH / 2f, -Size.GAME_HEIGHT / 2f),
        };
        var bodyPolygonUnit = new BodyPolygonUnit(pointList);
        m_TriggerBody.m_BodyPolygon.m_BodyPolygonUnits.Add(bodyPolygonUnit);
        
        //m_BoxCollider2D.size = new Vector2(Size.GAME_WIDTH, Size.GAME_HEIGHT);
    }

    private void Update()
    {
        if (PauseManager.IsGamePaused)
            return;
        
        transform.position = new Vector2(0f, -Size.GAME_HEIGHT/2);
    }

    public void Activate()
    {
        m_TriggerBody.enabled = true;
        SimulationManager.AddTriggerBody(m_TriggerBody);
        m_TriggerBody.m_OnTriggerBodyEnter += OnTriggerBodyEnter;
        m_TriggerBody.m_OnTriggerBodyExit += OnTriggerBodyExit;
        //m_TriggerBody.m_OnTriggerBodyStay += OnTriggerBodyStay;
    }

    public void Deactivate()
    {
        RemoveAllTickDamageContext();
        SimulationManager.RemoveTriggerBody(m_TriggerBody);
        m_TriggerBody.m_OnTriggerBodyEnter -= OnTriggerBodyEnter;
        m_TriggerBody.m_OnTriggerBodyExit -= OnTriggerBodyExit;
        //m_TriggerBody.m_OnTriggerBodyStay -= OnTriggerBodyStay;
        m_TriggerBody.enabled = false;
    }

    private void OnTriggerBodyEnter(TriggerBody other) // 충돌 감지
    {
        if (PauseManager.IsGamePaused)
            return;
        if (other.m_TriggerBodyType != TriggerBodyType.Enemy)
            return;
        
        var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();

        var enemyHealth = enemyUnit.m_EnemyHealth;
        var damageScale = _playerDamageData.damageScale[enemyUnit.m_EnemyType];
        var damageType = _playerDamageData.playerDamageType;
        var tickDamageContext = new TickDamageContext(Damage, damageScale, damageType);
        enemyHealth.AddTickDamageContext(m_ObjectName, other, tickDamageContext);
    }

    private void OnTriggerBodyExit(TriggerBody other) // 충돌 감지
    {
        if (PauseManager.IsGamePaused)
            return;
        if (other.m_TriggerBodyType != TriggerBodyType.Enemy)
            return;
        
        var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
        var enemyHealth = enemyUnit.m_EnemyHealth;
        enemyHealth.RemoveTickDamageContext(m_ObjectName, other);
    }

    private void RemoveAllTickDamageContext()
    {
        foreach (var triggerBody in m_TriggerBody.TriggerBodySet)
        {
            OnTriggerBodyExit(triggerBody);
        }
    }

    /*
    private void LateUpdate()
    {
        if (PauseManager.IsGamePaused)
            return;
        
        DealBombDamage();
    }

    private void DealBombDamage()
    {
        if (_enemyList.Count == 0)
            return;

        for (var i = _enemyList.Count - 1; i >= 0; --i)
        {
            DealDamage(_enemyList[i]);
        }
    }
    */

    private void OnTriggerBodyStay(TriggerBody other)
    {
        if (PauseManager.IsGamePaused)
            return;
        if (other.m_TriggerBodyType != TriggerBodyType.Enemy)
            return;
        
        var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();

        DealDamage(enemyUnit);
    }
    
    // private void OnTriggerStay2D(Collider2D other) // 충돌 감지
    // {
    //     if (PauseManager.IsGamePaused)
    //         return;
    //     
    //     if (other.gameObject.CompareTag("Enemy"))
    //     {
    //         var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
    //
    //         DealDamage(enemyUnit);
    //     }
    // }

    private IEnumerator BombDamageEnd() {
        yield return new WaitForMillisecondFrames(m_BombDuration);
        OnBombRemoved();
    }

    private void OnBombRemoved() {
        gameObject.SetActive(false);
    }
}
