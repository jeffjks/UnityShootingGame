using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetector : PlayerObject
{
    public string m_Explosion;
    public TriggerBody m_TriggerBodyCenter;
    public TriggerBody m_TriggerBodyLarge;
    
    private PlayerController _playerController;
    private bool _hasCollided;

    private void Awake()
    {
        _maxDamageLevel = _playerDamageData.damageByLevel.Count - 1;
        
        m_TriggerBodyCenter = GetComponent<TriggerBody>();
        _playerController = GetComponent<PlayerController>();

        PlayerManager.Action_OnPlayerRevive += ResetCollisionState;
        SimulationManager.AddTriggerBody(m_TriggerBodyCenter);
        SimulationManager.AddTriggerBody(m_TriggerBodyLarge);
        m_TriggerBodyCenter.m_OnTriggerBodyEnter += OnTriggerBodyCenterEnter;
        m_TriggerBodyLarge.m_OnTriggerBodyEnter += OnTriggerBodyLargeEnter;
    }

    private void OnDestroy()
    {
        PlayerManager.Action_OnPlayerRevive -= ResetCollisionState;
        SimulationManager.RemoveTriggerBody(m_TriggerBodyCenter);
        SimulationManager.RemoveTriggerBody(m_TriggerBodyLarge);
        m_TriggerBodyCenter.m_OnTriggerBodyEnter -= OnTriggerBodyCenterEnter;
        m_TriggerBodyLarge.m_OnTriggerBodyEnter -= OnTriggerBodyLargeEnter;
    }

    private void ResetCollisionState()
    {
        _hasCollided = false;
    }
    
    private void OnTriggerBodyCenterEnter(TriggerBody other) // 충돌 감지
    {
        if (PlayerInvincibility.IsInvincible)
            return;

        if (other.m_TriggerBodyType == TriggerBodyType.Bullet) // 대상이 총알이면 대상과 자신 파괴
        {
            var enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
            TriggerEnter(enemyBullet);
        }
        else if (other.m_TriggerBodyType == TriggerBodyType.Enemy) // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
        {
            if (other.gameObject.CheckLayer(Layer.AIR) == false)
                return;
            
            var enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
            TriggerEnter(enemyObject);
        }
    }
    
    private void OnTriggerBodyLargeEnter(TriggerBody other) // 충돌 감지
    {
        if (PlayerManager.IsPlayerAlive == false)
            return;
        if (other.m_TriggerBodyType != TriggerBodyType.Item)
            return;
        
        var itemObject = other.gameObject.GetComponentInParent<Item>();
        itemObject.GetItem();
    }
    
    // private void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    // {
    //     // if (SystemManager.GameMode == GameMode.Replay)
    //     //     return;
    //     if (PlayerInvincibility.IsInvincible)
    //         return;
    //     
    //     if (other.gameObject.CompareTag("EnemyBullet")) // 대상이 총알이면 대상과 자신 파괴
    //     {
    //         var enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
    //         TriggerEnter(enemyBullet);
    //     }
    //     else if (other.gameObject.CompareTag("Enemy")) // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
    //     {
    //         if (other.gameObject.CheckLayer(Layer.AIR) == false)
    //             return;
    //         
    //         var enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
    //         TriggerEnter(enemyObject);
    //     }
    // }

    /*
    public override void ExecuteCollisionEnter(int id)
    {
        if (PlayerInvincibility.IsInvincible)
            return;
        
        var unitObject = EnemyIdList[id];
        
        if (unitObject is EnemyBullet enemyBullet)
        {
            TriggerEnter(enemyBullet);
        }
        else if (unitObject is EnemyUnit enemyUnit)
        {
            TriggerEnter(enemyUnit);
        }
    }
    */

    private void TriggerEnter(EnemyBullet enemyBullet)
    {
        if (!_hasCollided)
        {
            enemyBullet.PlayEraseAnimation();
        }
        OnDeath();
    }

    private void TriggerEnter(EnemyUnit enemyUnit)
    {
        DealDamage(enemyUnit);
        OnDeath();
    }
    
    private void OnDeath() {
        if (!PlayerInvincibility.IsInvincible) {
            if (!_hasCollided) {
                _hasCollided = true;
                GameObject obj = PoolingManager.PopFromPool(m_Explosion, PoolingParent.Explosion); // 폭발 이펙트

                obj.transform.position = new Vector3(transform.position.x, transform.position.y, Depth.EXPLOSION);
                obj.SetActive(true);
                
                _playerController.StopAttack();
                transform.position = PlayerManager.Instance.PlayerDead(transform.position);
            }
        }
    }
}
