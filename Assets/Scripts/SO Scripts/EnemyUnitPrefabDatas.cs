using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Unit Prefab Data", menuName = "Scriptable Object/EnemyUnit Prefab Data")]

public class EnemyUnitPrefabDatas : ScriptableObject
{
    [Serializable]
    public struct EnemyUnitPrefab
    {
        public string enemyName;
        public EnemyUnit enemyUnit;
        public Vector3 defaultPosition;
    }

    public List<EnemyUnitPrefab> AirEnemyPrefabs;
    public List<EnemyUnitPrefab> GroundEnemyPrefabs;
}