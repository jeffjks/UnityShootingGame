using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : PlayerDamageUnit {

    [SerializeField] private PlayerLaserShooter m_PlayerLaserShooter = null;
    [SerializeField] private PlayerShooter m_PlayerShooter = null;
    [SerializeField] private PlayerController m_PlayerController = null;
    public float[] m_MinDamage = new float[3];
    public float[] m_MaxDamage = new float[3];

    [HideInInspector] public float m_LaserDamage;
    private float m_ShotLevelBonus;
    private int m_LaserIndex;

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
                
                if ((1 << other.gameObject.layer & Layer.LARGE) != 0) { // 대형이면
                    enemyObject.TakeDamage(m_LaserDamage, 1); // 데미지 줌
                }
                else { // 소형이면 기냥 죽임
                    enemyObject.OnDeath();
                }
            }
        }
    }

    public void UpdateLaserDamage() {
        m_LaserDamage = (m_Damage + m_PlayerShooter.m_ShotLevel * m_ShotLevelBonus);
    }

    public override void OnDeath() {
        Destroy(gameObject);
    }
}