using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Trigger Data", menuName = "Scriptable Object/Trigger Data")]

public class TriggerDatas : ScriptableObject
{
    public List<TriggerBodyType> gameBoundaryTriggerList;
    public List<TriggerBodyType> cameraBoundaryTriggerList;
    public List<TriggerBodyType> playerCenterTriggerList;
    public List<TriggerBodyType> playerWeaponTriggerList;
    public List<TriggerBodyType> playerLargeTriggerList;
}