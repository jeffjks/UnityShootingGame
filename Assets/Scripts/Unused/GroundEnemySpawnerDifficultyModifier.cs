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
        if (m_SystemManager.GetDifficulty() == 0 && !m_Normal)
            m_GroundEnemySpawner.gameObject.SetActive(false);
        else if (m_SystemManager.GetDifficulty() == 1 && !m_Expert)
            m_GroundEnemySpawner.gameObject.SetActive(false);
        else if (m_SystemManager.GetDifficulty() == 2 && !m_Hell)
            m_GroundEnemySpawner.gameObject.SetActive(false);
        
        if (m_Normal && m_SpawnPeriod_Normal > 0) {
            //m_GroundEnemySpawner.SetSpawnPeriodTime(m_SpawnPeriod_Normal);
        }
        else if (m_Expert && m_SpawnPeriod_Expert > 0) {
            //m_GroundEnemySpawner.SetSpawnPeriodTime(m_SpawnPeriod_Expert);
        }
    }
}
