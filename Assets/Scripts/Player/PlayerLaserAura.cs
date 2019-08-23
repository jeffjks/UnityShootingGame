using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserAura : PlayerDamageUnit
{
    [SerializeField] private PlayerLaser m_PlayerLaser = null;
    [SerializeField] private PlayerController m_PlayerController = null;
    private float m_MinDamage, m_MaxDamage;

    private float m_LaserDamage;
    private float m_ShotLevelBonus;
    private int m_LaserIndex;

    void OnEnable() {
        m_LaserDamage = m_PlayerLaser.m_LaserDamage;
    }

    void OnTriggerStay2D(Collider2D other) // 닿을 때
    {
        if (m_PlayerController.m_SlowMode) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                
                if ((1 << other.gameObject.layer & Layer.LARGE) != 0) { // 대형이면
                    enemyObject.TakeDamage(m_LaserDamage, 2); // 데미지 줌
                }
                else { // 소형이면 기냥 죽임
                    enemyObject.OnDeath();
                }
            }
        }
    }

    public override void OnDeath() {
        Destroy(gameObject);
    }
}
