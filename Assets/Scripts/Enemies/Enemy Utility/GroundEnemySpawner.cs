using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemySpawner : MonoBehaviour
{
    public EnemySpawnerDatas m_EnemySpawnerDatas;
    private GameObject[] m_EnemyUnits;
    private MovePattern[] m_MovePattern;
    
    public bool m_Normal;
    public bool m_Expert;
    public bool m_Hell;

    private SystemManager m_SystemManager;

    void Start()
    {
        m_SystemManager = SystemManager.instance_sm;

        m_EnemyUnits = m_EnemySpawnerDatas.EnemyUnits;
        m_MovePattern = m_EnemySpawnerDatas.MovePatterns;
        
        if (m_MovePattern.Length == 0) {
            Debug.LogError("Enemy Spawner doesn't have any MovePattern class.");
            return;
        }
        if (m_EnemyUnits.Length == 0) {
            Debug.LogError("Enemy Spawner doesn't have any Enemy Prefabs.");
            return;
        }
        if (!CheckDifficulty()) {
            return;
        }
        StartCoroutine(SpawnEnemySequence());
        StartCoroutine(DestroySpawner());
    }

    private void SpawnEnemy(GameObject enemy) {
        GameObject ins = Instantiate(enemy, transform.position, Quaternion.identity);
        EnemyUnit enemy_unit = ins.GetComponent<EnemyUnit>();
        enemy_unit.m_MoveVector = new MoveVector(m_MovePattern[0].speed, m_MovePattern[0].direction);
        enemy_unit.RotateImmediately(m_MovePattern[0].direction);
        
        if (m_EnemySpawnerDatas.InteractableTimer != 0)
        {
            enemy_unit.DisableInteractable(m_EnemySpawnerDatas.InteractableTimer);
        }
        
        for (int i = 0; i < m_MovePattern.Length; i++) {
            enemy_unit.m_TweenDataQueue.Enqueue(new TweenData(m_MovePattern[i]));
        }
        enemy_unit.StartPlayTweenData();
        
        if (m_EnemySpawnerDatas.RemoveTimer > 0) {
            StartCoroutine(DestroySpawnedEnemy());
        }
    }

    private IEnumerator SpawnEnemySequence()
    {
        yield return new WaitForMillisecondFrames(m_EnemySpawnerDatas.ActivateTime + 1000);
        while (true) {
            for (int i = 0; i < m_EnemyUnits.Length; i++) {
                SpawnEnemy(m_EnemyUnits[i]);
                yield return new WaitForMillisecondFrames(m_EnemySpawnerDatas.SpawnPeriod);
            }
        }
    }

    private IEnumerator DestroySpawner()
    {
        yield return new WaitForMillisecondFrames(m_EnemySpawnerDatas.DeactivateTime);
        Destroy(gameObject);
    }

    private IEnumerator DestroySpawnedEnemy()
    {
        yield return new WaitForMillisecondFrames(m_EnemySpawnerDatas.RemoveTimer);
        Destroy(gameObject);
    }

    private bool CheckDifficulty() {
        int difficulty = m_SystemManager.GetDifficulty();
        if (difficulty == 0 && m_Normal) {
            return true;
        }
        if (difficulty == 1 && m_Expert) {
            return true;
        }
        if (difficulty == 2 && m_Hell) {
            return true;
        }
        return false;
    }
}
