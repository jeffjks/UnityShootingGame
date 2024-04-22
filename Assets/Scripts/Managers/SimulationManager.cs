using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerBodyType
{
    PlayerCenter,
    PlayerWeapon,
    PlayerLarge,
    Enemy,
    Bullet,
    Item,
}

public class SimulationManager : MonoBehaviour
{
    public TriggerDatas m_TriggerDatas;
    
    public static LinkedList<MovableObject> MovableObjects = new();
    public static Dictionary<TriggerBodyType, LinkedList<TriggerBody>> TriggerBodies = new()
    {
        { TriggerBodyType.PlayerCenter, new () },
        { TriggerBodyType.PlayerWeapon, new () },
        { TriggerBodyType.PlayerLarge, new () },
        { TriggerBodyType.Enemy, new () },
        { TriggerBodyType.Bullet, new () },
        { TriggerBodyType.Item, new () },
    };

    private void Update()
    {
        foreach (var movableObject in MovableObjects)
        {
            movableObject.SimulateMovement();
        }

        foreach (var otherTriggerBodyType in m_TriggerDatas.playerCenterTriggerList)
        {
            foreach (var triggerBody in TriggerBodies[TriggerBodyType.PlayerCenter])
            {
                foreach (var otherTriggerBody in TriggerBodies[otherTriggerBodyType])
                {
                    TriggerBodyManager.CheckOverlapTriggerBody(triggerBody, otherTriggerBody);
                }
            }
        }

        foreach (var otherTriggerBodyType in m_TriggerDatas.playerWeaponTriggerList)
        {
            foreach (var triggerBody in TriggerBodies[TriggerBodyType.PlayerWeapon])
            {
                foreach (var otherTriggerBody in TriggerBodies[otherTriggerBodyType])
                {
                    TriggerBodyManager.CheckOverlapTriggerBody(triggerBody, otherTriggerBody);
                }
            }
        }

        foreach (var otherTriggerBodyType in m_TriggerDatas.playerLargeTriggerList)
        {
            foreach (var triggerBody in TriggerBodies[TriggerBodyType.PlayerLarge])
            {
                foreach (var otherTriggerBody in TriggerBodies[otherTriggerBodyType])
                {
                    TriggerBodyManager.CheckOverlapTriggerBody(triggerBody, otherTriggerBody);
                }
            }
        }
    }
}
