using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Spawner Data", menuName = "Scriptable Object/Enemy Spawner Data")]

public class EnemySpawnerDatas : ScriptableObject
{
    public GameObject[] EnemyUnits;
    public int SpawnPeriod;
    public int InteractableTimer;
    public int RemoveTimer;
    public MovePattern[] MovePatterns;
    public int ActivateTime;
    public int DeactivateTime;
}