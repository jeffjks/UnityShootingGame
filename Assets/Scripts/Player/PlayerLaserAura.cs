using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserAura : PlayerObject
{
    private PlayerLaser m_PlayerLaser = null;
    private PlayerUnit m_PlayerMovement = null;
    private int m_MinDamage, m_MaxDamage;

    private int m_ShotLevelBonus;
    private int m_LaserIndex;
    
    private void Awake()
    {
        m_PlayerLaser = GetComponentInParent<PlayerLaser>();
        m_PlayerMovement = GetComponentInParent<PlayerUnit>();
    }

    void OnTriggerStay2D(Collider2D other) // 닿을 때
    {
        if (m_PlayerMovement.SlowMode) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                
                if (Utility.CheckLayer(other.gameObject, Layer.LARGE)) { // 대형이면
                    DealDamage(enemyObject); // 데미지 줌
                }
                else { // 소형이면 기냥 죽임
                    enemyObject.m_EnemyDeath.OnDying();
                }
            }
        }
    }
}
