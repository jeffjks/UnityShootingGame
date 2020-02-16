using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GroundEnemySpawner : EnemyUnit
{
    [System.Serializable]
    public class MovePattern
    {
        public float delay, direction, speed, time;
    }

    public GameObject[] m_Enemy;
    public bool m_Normal, m_Expert, m_Hell;
    public float m_Speed, m_Direction;
    public float m_SpawnPeriod, m_RemoveTimer;
    [Space(10)]
    public MovePattern[] m_MovePattern;
    [Space(10)]
    [Tooltip("스포너 활성화 시간")]
    public float m_ActivateTime;
    [Tooltip("스포너 비활성화 시간")]
    public float m_DeactivateTime;
    [Space(10)]
    public float m_AttackableTimer;

    void Start()
    {
        Invoke("StartSpawning", m_ActivateTime);
        Destroy(gameObject, m_DeactivateTime);
    }

    private void StartSpawning() {
        StartCoroutine(SpawnEnemyTanks());
    }

    private void SpawnEnemy(GameObject enemy) {
        GameObject obj = Instantiate(enemy, transform.position, transform.rotation);
        EnemyUnit enemy_unit = obj.GetComponent<EnemyUnit>();
        Sequence sequence = DOTween.Sequence();
        enemy_unit.m_MoveVector = new MoveVector(m_Speed, m_Direction);
        if (m_AttackableTimer != 0f) {
            enemy_unit.DisableAttackable(m_AttackableTimer);
        }
        
        for (int i = 0; i < m_MovePattern.Length; i++) {
            float pattern_delay = m_MovePattern[i].delay;
            float pattern_direction = m_MovePattern[i].direction;
            float pattern_speed = m_MovePattern[i].speed;
            float pattern_time = m_MovePattern[i].time;

            if (pattern_direction == 8739f) {
                pattern_direction = enemy_unit.m_MoveVector.direction;
            }
            if (pattern_speed == 8739f) {
                pattern_speed = enemy_unit.m_MoveVector.speed;
            }

            sequence.AppendInterval(pattern_delay)
            .Append(DOTween.To(()=>enemy_unit.m_MoveVector.direction, x=>enemy_unit.m_MoveVector.direction = x, pattern_direction, pattern_time).SetEase(Ease.Linear))
            .Join(DOTween.To(()=>enemy_unit.m_MoveVector.speed, x=>enemy_unit.m_MoveVector.speed = x, pattern_speed, pattern_time).SetEase(Ease.Linear));
        }
        
        if (m_RemoveTimer > 0) {
            Destroy(obj, m_RemoveTimer);
        }
    }

    IEnumerator SpawnEnemyTanks()
    {
        yield return new WaitForSeconds(1f);
        while (true) {
            for (int i = 0; i < m_Enemy.Length; i++) {
                if (m_SystemManager.m_Difficulty == 0 && m_Normal)
                    SpawnEnemy(m_Enemy[i]);
                else if (m_SystemManager.m_Difficulty == 1 && m_Expert)
                    SpawnEnemy(m_Enemy[i]);
                else if (m_SystemManager.m_Difficulty == 2 && m_Hell)
                    SpawnEnemy(m_Enemy[i]);
                yield return new WaitForSeconds(m_SpawnPeriod);
            }
        }
    }
}
