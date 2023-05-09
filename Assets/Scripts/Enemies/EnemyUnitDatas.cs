using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class EnemyUnitData {
    public int m_UnitID;
	public GameObject m_Prefab;
    public EnemyType m_EnemyType;
    public HealthType m_HealthType;
    [DrawIf("m_HealthType", HealthType.Share, ComparisonType.NotEqual)]
    public int m_DefaultHealth = -1;
	public int m_Score;
}

[CreateAssetMenu(fileName = "Enemy Unit Datas", menuName = "Scriptable Object/Enemy Unit Datas")]
public class EnemyUnitDatas : ScriptableObject
{
    public List<EnemyUnitData> m_Values;
    private Dictionary<int, EnemyUnitData> m_EnemyUnitByID;
    private Dictionary<int, EnemyUnitData> EnemyUnitByID {
        get {
            if (m_EnemyUnitByID == null) { // 초기화 코드
                m_EnemyUnitByID = new Dictionary<int, EnemyUnitData>();
                m_Values.ForEach(p => m_EnemyUnitByID.Add(p.m_UnitID, p));
            }
            return m_EnemyUnitByID;
        }
    }

    public EnemyUnitData Find(int id) { // (?)
        return EnemyUnitByID[id];
    }
}