using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemySpawnerDifficultyModifier : MonoBehaviour
{
    public GroundEnemySpawner m_GroundEnemySpawner;

    public bool m_Normal, m_Expert, m_Hell;
    public int m_SpawnPeriod_Normal, m_SpawnPeriod_Expert;

    private SystemManager m_SystemManager;

    void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
    }
    
    void Start()
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal && !m_Normal)
            m_GroundEnemySpawner.gameObject.SetActive(false);
        else if (SystemManager.Difficulty == GameDifficulty.Expert && !m_Expert)
            m_GroundEnemySpawner.gameObject.SetActive(false);
        else if (SystemManager.Difficulty == GameDifficulty.Hell && !m_Hell)
            m_GroundEnemySpawner.gameObject.SetActive(false);
        
        if (m_Normal && m_SpawnPeriod_Normal > 0) {
            //m_GroundEnemySpawner.SetSpawnPeriodTime(m_SpawnPeriod_Normal);
        }
        else if (m_Expert && m_SpawnPeriod_Expert > 0) {
            //m_GroundEnemySpawner.SetSpawnPeriodTime(m_SpawnPeriod_Expert);
        }
    }
}
