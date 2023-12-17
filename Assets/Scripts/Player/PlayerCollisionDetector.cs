using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetector : MonoBehaviour
{
    public string m_Explosion;

    private PlayerUnit _playerUnit;
    private PlayerController _playerController;
    private bool _hasCollided;

    private void Awake()
    {
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
    
    void OnTriggerEnter2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("EnemyBullet")) { // 대상이 총알이면 대상과 자신 파괴
            if (!PlayerInvincibility.IsInvincible) {
                if (!_hasCollided) {
                    try {
                        EnemyBullet enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
                        enemyBullet.PlayEraseAnimation();
                    }
                    catch {
                        return;
                    }
                }
                OnDeath();
            }
        }

        else if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
            if (!PlayerInvincibility.IsInvincible) {
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();

                if (gameObject.CheckLayer(Layer.AIR)) {
                    _playerUnit.DealCollisionDamage(enemyObject);
                    OnDeath();
                }
            }
        }
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
