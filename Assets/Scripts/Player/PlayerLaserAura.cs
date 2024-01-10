using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserAura : PlayerObject
{
    private PlayerUnit _playerUnit;
    private PlayerLaserHandler _playerLaserHandler;

    private void Start()
    {
        _playerLaserHandler = GetComponentInParent<PlayerLaserHandler>();
        _playerUnit = GetComponentInParent<PlayerUnit>();
        DamageLevel = _playerUnit.PlayerAttackLevel;

        _playerLaserHandler.Action_OnLaserIndexChanged += UpdateLaserIndex;
        _playerUnit.Action_OnUpdatePlayerAttackLevel += () => DamageLevel = _playerUnit.PlayerAttackLevel;
    }

    void OnTriggerStay2D(Collider2D other) // 닿을 때
    {
        if (PauseManager.IsGamePaused)
            return;
        
        if (_playerUnit.SlowMode) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                
                if (other.gameObject.CheckLayer(Layer.LARGE)) { // 대형이면
                    DealDamage(enemyObject); // 데미지 줌
                    HitCountController.Instance.HitCountLaserCounter++;
                }
                else { // 소형이면 기냥 죽임
                    if (enemyObject.m_EnemyDeath.KillEnemy())
                        HitCountController.Instance.AddHitCount();
                }
            }
        }
    }

    private void UpdateLaserIndex()
    {
        DamageLevel = _playerUnit.PlayerAttackLevel;
    }
}
