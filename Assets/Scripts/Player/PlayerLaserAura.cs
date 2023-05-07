using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserAura : PlayerObject
{
    private PlayerLaserManager m_PlayerLaser = null;
    private PlayerUnit m_PlayerController = null;
    private int m_MinDamage, m_MaxDamage;

    private int m_LaserDamage;
    private int m_ShotLevelBonus;
    private int m_LaserIndex;
    private PlayerDamageType m_PlayerDamageType = PlayerDamageType.LaserAura;

    void OnEnable() {
        if (m_PlayerLaser != null)
            m_LaserDamage = m_PlayerLaser.m_LaserDamage;
    }

    void Start()
    {
        m_PlayerLaser = GetComponentInParent<PlayerLaserManager>();
        m_PlayerController = GetComponentInParent<PlayerUnit>();
        m_LaserDamage = m_PlayerLaser.m_LaserDamage;
    }

    void OnTriggerStay2D(Collider2D other) // 닿을 때
    {
        if (m_PlayerController.m_SlowMode) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                
                if ((1 << other.gameObject.layer & Layer.LARGE) != 0) { // 대형이면
                    DealDamage(enemyObject, m_LaserDamage, m_PlayerDamageType); // 데미지 줌
                }
                else { // 소형이면 기냥 죽임
                    enemyObject.m_EnemyDeath.OnDying();
                }
            }
        }
    }
}
