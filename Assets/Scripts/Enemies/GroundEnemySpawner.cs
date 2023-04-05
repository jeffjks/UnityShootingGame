using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemySpawner : EnemyUnit
{
    [System.Serializable]
    public class MovePattern
    {
        public int delay;
        public float speed;
        public float direction;
        public int duration;
    }

    public GameObject[] m_Enemy;
    public bool m_Normal, m_Expert, m_Hell;
    public float m_Speed, m_Direction;
    [Space(10)]
    public int m_SpawnPeriod;
    public int m_RemoveTimer;
    [Space(10)]
    public MovePattern[] m_MovePattern;
    [Space(10)]
    [Tooltip("스포너 활성화 시간")]
    public int m_ActivateTime;
    [Tooltip("스포너 비활성화 시간")]
    public int m_DeactivateTime;
    [Space(10)]
    public int m_AttackableTimer;

    void Start()
    {
        StartCoroutine(SpawnEnemyTanks());
        StartCoroutine(DestroySpawner());
    }

    private void SpawnEnemy(GameObject enemy) {
        GameObject ins = Instantiate(enemy, transform.position, transform.rotation);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.m_MoveVector = new MoveVector(m_Speed, m_Direction);
        
        if (m_AttackableTimer != 0) {
            enemy_unit.DisableAttackable(m_AttackableTimer);
        }
        
        for (int i = 0; i < m_MovePattern.Length; i++) {
            int pattern_delay = m_MovePattern[i].delay;
            MoveVector pattern_moveVector = new MoveVector(m_MovePattern[i].speed, m_MovePattern[i].direction);
            int pattern_duration = m_MovePattern[i].duration;

            enemy_unit.m_TweenDataQueue.Enqueue(new TweenData(pattern_delay));
            enemy_unit.m_TweenDataQueue.Enqueue(new TweenDataMoveVector(pattern_moveVector, pattern_duration));

            //sequence.AppendInterval(pattern_delay)
            //.Append(DOTween.To(()=>enemy_unit.m_MoveVector.direction, x=>enemy_unit.m_MoveVector.direction = x, pattern_direction, pattern_duration).SetEase(Ease.Linear))
            //.Join(DOTween.To(()=>enemy_unit.m_MoveVector.speed, x=>enemy_unit.m_MoveVector.speed = x, pattern_speed, pattern_duration).SetEase(Ease.Linear));
        }
        
        if (m_RemoveTimer > 0) {
            StartCoroutine(DestroySpawnedEnemy(ins));
        }
    }

    IEnumerator SpawnEnemyTanks()
    {
        yield return new WaitForMillisecondFrames(m_ActivateTime + 1000);
        while (true) {
            for (int i = 0; i < m_Enemy.Length; i++) {
                if (m_SystemManager.m_Difficulty == 0 && m_Normal)
                    SpawnEnemy(m_Enemy[i]);
                else if (m_SystemManager.m_Difficulty == 1 && m_Expert)
                    SpawnEnemy(m_Enemy[i]);
                else if (m_SystemManager.m_Difficulty == 2 && m_Hell)
                    SpawnEnemy(m_Enemy[i]);
                yield return new WaitForMillisecondFrames(m_SpawnPeriod);
            }
        }
    }

    IEnumerator DestroySpawner()
    {
        yield return new WaitForMillisecondFrames(m_DeactivateTime);
        Destroy(gameObject);
        yield break;
    }

    IEnumerator DestroySpawnedEnemy(GameObject gameObject)
    {
        yield return new WaitForMillisecondFrames(m_RemoveTimer);
        Destroy(gameObject);
        yield break;
    }
}
