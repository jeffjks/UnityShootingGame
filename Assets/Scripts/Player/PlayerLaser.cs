using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : PlayerObject
{
    public PlayerUnit m_PlayerUnit;
    public PlayerDamageDatas[] m_PlayerLaserDamageData;

    private PlayerLaserHandler _playerLaserHandler;

    private void Start()
    {
        _playerLaserHandler = GetComponent<PlayerLaserHandler>();
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;
        
        _playerLaserHandler.Action_OnLaserIndexChanged += UpdateLaserIndex;
        m_PlayerUnit.Action_OnUpdatePlayerAttackLevel += UpdatePlayerAttackLevel;
    }

    void OnTriggerStay2D(Collider2D other) // 닿을 때
    {
        if (m_PlayerUnit.SlowMode) {
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
        _playerDamageData = m_PlayerLaserDamageData[_playerLaserHandler.LaserIndex];
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;
    }

    private void UpdatePlayerAttackLevel()
    {
        DamageLevel = m_PlayerUnit.PlayerAttackLevel;
    }
}