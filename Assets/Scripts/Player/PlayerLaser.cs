using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : PlayerLaserManager {
    
    public PlayerShooter m_PlayerShooter;
    public PlayerController m_PlayerController;
    public int[] m_MinDamage = new int[3];
    public int[] m_MaxDamage = new int[3];
    
    private int m_ShotLevelBonus;
    private PlayerDamageType m_PlayerDamageType = PlayerDamageType.Laser;

    void Start()
    {
        m_LaserIndex = m_PlayerLaserShooter.m_LaserIndex;
        m_ShotLevelBonus = (m_MaxDamage[m_LaserIndex] - m_MinDamage[m_LaserIndex])/4;
        m_Damage = m_MinDamage[m_LaserIndex];
        UpdateLaserDamage();
    }

    void OnTriggerStay2D(Collider2D other) // 닿을 때
    {
        if (m_PlayerController.m_SlowMode) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                
                if (Utility.CheckLayer(other.gameObject, Layer.LARGE)) { // 대형이면
                    DealDamage(enemyObject, m_LaserDamage, m_PlayerDamageType); // 데미지 줌
                }
                else { // 소형이면 기냥 죽임
                    enemyObject.m_EnemyDeath.OnDying();
                }
            }
        }
    }

    public void UpdateLaserDamage() {
        m_LaserDamage = (m_Damage + m_PlayerShooter.m_ShotLevel * m_ShotLevelBonus);
    }
}