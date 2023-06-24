using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserManager : PlayerObject {

    public PlayerLaserShooterManager m_PlayerLaserShooterManager;
    public PlayerShootHandler m_PlayerShootHandler;
    public PlayerUnit m_PlayerUnit;
    public PlayerDamageDatas[] m_PlayerDamageData;

    void Start()
    {
        UpdateLaserDamage();
        m_PlayerLaserShooterManager.Action_OnStartLaser += UpdateLaserDamage;
    }

    void OnTriggerStay2D(Collider2D other) // 닿을 때
    {
        if (m_PlayerUnit.SlowMode) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                var laserIndex = m_PlayerLaserShooterManager.LaserIndex;
                
                if (Utility.CheckLayer(other.gameObject, Layer.LARGE)) { // 대형이면
                    DealDamage(enemyObject, m_Damage, m_PlayerDamageData[laserIndex].playerDamageType); // 데미지 줌
                }
                else { // 소형이면 기냥 죽임
                    enemyObject.m_EnemyDeath.OnDying();
                }
            }
        }
    }

    public void UpdateLaserDamage()
    {
        var laserIndex = m_PlayerLaserShooterManager.LaserIndex;
        var attackLevel = m_PlayerShootHandler.PlayerAttackLevel;
        m_Damage = m_PlayerDamageData[laserIndex].damageByLevel[attackLevel];
    }
}