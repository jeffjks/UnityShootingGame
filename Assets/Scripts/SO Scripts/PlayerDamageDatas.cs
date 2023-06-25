using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Damage", menuName = "Scriptable Object/Player Damage Data")]

public class PlayerDamageDatas : ScriptableObject
{
    [System.Serializable]
    public struct DamageScale
    {
        public int zakoEnemy;
        public int middleBossEnemy;
        public int bossEnemy;
        
        public int this[EnemyType enemyType]
        {
            get
            {
                switch (enemyType)
                {
                    case EnemyType.Zako:
                        return zakoEnemy;
                    case EnemyType.MiddleBoss:
                        return middleBossEnemy;
                    case EnemyType.Boss:
                        return bossEnemy;
                }
                throw new IndexOutOfRangeException();
            }
        }
    }

    public PlayerDamageType playerDamageType;
    public DamageScale damageScale;
    public List<int> cooldownByLevel;
    public List<int> damageByLevel;
}