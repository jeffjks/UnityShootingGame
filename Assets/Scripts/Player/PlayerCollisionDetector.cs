using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetector : PlayerObject
{
    public string m_Explosion;

    private PlayerUnit _playerUnit;
    private PlayerController _playerController;
    private bool _hasCollided;

    private void Awake()
    {
        _maxDamageLevel = _playerDamageData.damageByLevel.Count - 1;
        
        _playerUnit = GetComponent<PlayerUnit>();
        _playerController = GetComponent<PlayerController>();

        PlayerManager.Action_OnPlayerRevive += ResetCollisionState;
    }

    private void OnDestroy()
    {
        PlayerManager.Action_OnPlayerRevive -= ResetCollisionState;
    }

    private void ResetCollisionState()
    {
        _hasCollided = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("EnemyBullet")) // 대상이 총알이면 대상과 자신 파괴
        {
            if (PlayerInvincibility.IsInvincible)
                return;
            
            var enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
            TriggerEnter(enemyBullet);
        }

        else if (other.gameObject.CompareTag("Enemy")) // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
        {
            if (PlayerInvincibility.IsInvincible)
                return;
            if (other.gameObject.CheckLayer(Layer.AIR) == false)
                return;
            
            var enemyObject = other.gameObject.GetComponentInParent<EnemyBullet>();
            TriggerEnter(enemyObject);
        }
    }

    public override void ExecuteCollisionEnter(int id)
    {
        if (PlayerInvincibility.IsInvincible)
            return;
        
        var unitObject = ObjectIdList[id];
        
        if (unitObject is EnemyBullet enemyBullet)
        {
            TriggerEnter(enemyBullet);
        }
        else if (unitObject is EnemyUnit enemyUnit)
        {
            TriggerEnter(enemyUnit);
        }
    }

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
