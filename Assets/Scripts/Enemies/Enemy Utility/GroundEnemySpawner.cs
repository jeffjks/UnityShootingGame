using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemySpawner : EnemyUnit
{
    public EnemySpawnerDatas m_EnemySpawnerDatas;
    private GameObject[] m_EnemyUnits;
    
    public GameObject[] m_Enemy;
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
        m_EnemyUnits = m_EnemySpawnerDatas.EnemyUnits;
        StartCoroutine(SpawnEnemySequence());
        StartCoroutine(DestroySpawner());
    }

    private void SpawnEnemy(GameObject enemy) {
        if (m_MovePattern.Length == 0)
        {
            Debug.LogError("Enemy Spawner doesn't have any MovePattern class.");
            return;
        }
        GameObject ins = Instantiate(enemy, transform.position, Quaternion.identity);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        //enemy_unit.m_MoveVector = new MoveVector(m_MovePattern[0].speed, m_MovePattern[0].direction);
        enemy_unit.RotateImmediately(m_MovePattern[0].direction);
        
        if (m_EnemySpawnerDatas.InteractableTimer != 0)
        {
            enemy_unit.DisableInteractable(m_EnemySpawnerDatas.InteractableTimer);
        }
        
        for (int i = 0; i < m_MovePattern.Length; i++) {
            enemy_unit.m_TweenDataQueue.Enqueue(new TweenDataMovePattern(m_MovePattern[i]));
        }
        enemy_unit.StartPlayTweenData();
        
        if (m_EnemySpawnerDatas.RemoveTimer > 0) {
            StartCoroutine(DestroySpawnedEnemy());
        }
    }

    IEnumerator SpawnEnemySequence()
    {
        yield return new WaitForMillisecondFrames(m_EnemySpawnerDatas.ActivateTime + 1000);
        while (true) {
            for (int i = 0; i < m_EnemyUnits.Length; i++) {
                SpawnEnemy(m_EnemyUnits[i]);
                yield return new WaitForMillisecondFrames(m_EnemySpawnerDatas.SpawnPeriod);
            }
        }
    }

    IEnumerator DestroySpawner()
    {
        yield return new WaitForMillisecondFrames(m_EnemySpawnerDatas.DeActivateTime);
        m_EnemyDeath.OnRemoved();
    }

    IEnumerator DestroySpawnedEnemy()
    {
        yield return new WaitForMillisecondFrames(m_EnemySpawnerDatas.RemoveTimer);
        m_EnemyDeath.OnRemoved();
    }

    public void SetSpawnPeriodTime(int millisecond) {
        m_SpawnPeriod = millisecond;
    }
}
