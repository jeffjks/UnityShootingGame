using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Damage", menuName = "Scriptable Object/Player Damage")]

public class PlayerDamageDatas : ScriptableObject
{
    [System.Serializable]
    public struct DamageScale
    {
        public int smallEnemy;
        public int largeEnemy;
        public int bossEnemy;
    }

    public PlayerDamageType playerDamageType;
    public DamageScale damageScale;
    public List<int> cooldownByLevel;
    public List<int> damageByLevel;
}