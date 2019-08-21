using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBombDamage : PlayerDamageUnit
{
    [SerializeField] private BoxCollider2D m_BoxCollider2D = null;
    [SerializeField] private float m_BombDuration = 0f;

    void Start()
    {
        m_BoxCollider2D.size = new Vector2(Size.GAME_WIDTH, Size.GAME_HEIGHT);
    }

    void OnEnable()
    {
        Invoke("BombDamageEnd", m_BombDuration);
    }

    void OnTriggerStay2D(Collider2D other) // 충돌 감지
    {
        if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 공중, 공격 가능 상태면 데미지 주고 자신 파괴
            EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();

            if (enemyObject.m_Class == EnemyClass.Zako) {
                enemyObject.TakeDamage(m_Damage);
            }
            if (enemyObject.m_Class == EnemyClass.MiddleBoss) {
                if (enemyObject.m_ParentEnemy == null) {
                    enemyObject.TakeDamage(m_Damage * 0.5f);
                }
            }
            if (enemyObject.m_Class == EnemyClass.Boss) {
                if (enemyObject.m_ParentEnemy == null) {
                    enemyObject.TakeDamage(m_Damage * 0.33f);
                }
            }
        }
    }

    private void BombDamageEnd() {
        OnDeath();
    }

    public override void OnDeath() {
        gameObject.SetActive(false);
    }
}
