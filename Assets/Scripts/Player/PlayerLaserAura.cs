using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserAura : PlayerObject
{
    public PlayerUnit m_PlayerUnit;
    public TriggerBody m_TriggerBody;
    
    private PlayerLaserHandler _playerLaserHandler;

    private void Start()
    {
        _playerLaserHandler = GetComponentInParent<PlayerLaserHandler>();
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;

        _playerLaserHandler.Action_OnLaserIndexChanged += UpdateLaserIndex;
        m_PlayerUnit.Action_OnUpdatePlayerAttackLevel += () => DamageLevel = m_PlayerUnit.PlayerAttackLevel;
    }

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
        if (m_PlayerUnit.SlowMode == false)
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
            if (enemyUnit.m_EnemyDeath.IsDead)
                return;
            enemyUnit.m_EnemyDeath.KillEnemy();
            HitCountController.Instance.AddHitCount();
        }
    }

    private void OnTriggerBodyExit(TriggerBody other) // 충돌 감지
    {
        if (m_PlayerUnit.SlowMode == false)
            return;
        if (other.m_TriggerBodyType != TriggerBodyType.Enemy)
            return;
        
        var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
            
        if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
        {
            var enemyHealth = enemyUnit.m_EnemyHealth;
            enemyHealth.RemoveTickDamageContext(m_ObjectName);
        }
    }

    private void OnTriggerBodyStay(TriggerBody other) // 충돌 감지
    {
        if (m_PlayerUnit.SlowMode == false)
            return;
        if (other.m_TriggerBodyType != TriggerBodyType.Enemy)
            return;
        
        var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
            
        if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
        {
            DealDamage(enemyUnit);
            HitCountController.Instance.HitCountLaserCounter++;
        }
        else // 소형이면
        {
            if (enemyUnit.m_EnemyDeath.IsDead)
                return;
            enemyUnit.m_EnemyDeath.KillEnemy();
            HitCountController.Instance.AddHitCount();
        }
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

    /*
    private void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (SystemManager.GameMode == GameMode.Replay)
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
        if (SystemManager.GameMode == GameMode.Replay)
            return;
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemyUnit = other.gameObject.GetComponentInParent<EnemyUnit>();
            TriggerExit(enemyUnit);
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

    public override void ExecuteCollisionExit(int id)
    {
        var enemyUnit = EnemyIdList[id] as EnemyUnit;

        if (enemyUnit == null)
        {
            Debug.LogError($"{EnemyIdList[id].GetType()} (id: {id}) can not cast to EnemyUnit!");
            return;
        }

        TriggerExit(enemyUnit);
    }

    private void TriggerEnter(EnemyUnit enemyUnit)
    {
        if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
        {
            _enemyList.Add(enemyUnit);
        }
        else // 소형이면
        {
            if (enemyUnit.m_EnemyDeath.KillEnemy())
                HitCountController.Instance.AddHitCount();
        }
    }

    private void TriggerExit(EnemyUnit enemyUnit)
    {
        if (enemyUnit.gameObject.CheckLayer(Layer.LARGE)) // 대형이면
        {
            _enemyList.Remove(enemyUnit);
        }
    }
    */

    private void UpdateLaserIndex()
    {
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;
    }
}
