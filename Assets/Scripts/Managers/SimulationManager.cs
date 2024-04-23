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

    private List<TriggerBodyType> _triggerBodyTypes = new()
    {
        TriggerBodyType.PlayerCenter,
        TriggerBodyType.PlayerWeapon,
        TriggerBodyType.PlayerLarge
    };

    private void Update()
    {
        foreach (var movableObject in MovableObjects)
        {
            movableObject.SimulateMovement();
        }

        foreach (var triggerBodyType in _triggerBodyTypes)
        {
            ExecuteCheckOverlapTriggerBody(triggerBodyType);
        }

        foreach (var triggerBody in TriggerBodies[TriggerBodyType.PlayerWeapon])
        {
            triggerBody.OnTriggerBodyCollisionStay();
        }
    }

    private void ExecuteCheckOverlapTriggerBody(TriggerBodyType triggerBodyType)
    {
        var triggerList = GetTriggerList(triggerBodyType);
        
        foreach (var otherTriggerBodyType in triggerList)
        {
            foreach (var triggerBody in TriggerBodies[triggerBodyType])
            {
                foreach (var otherTriggerBody in TriggerBodies[otherTriggerBodyType])
                {
                    TriggerBodyManager.CheckOverlapTriggerBody(triggerBody, otherTriggerBody);
                }
            }
        }
    }

    private List<TriggerBodyType> GetTriggerList(TriggerBodyType triggerBodyType)
    {
        switch (triggerBodyType)
        {
            case TriggerBodyType.PlayerCenter:
                return m_TriggerDatas.playerCenterTriggerList;
            case TriggerBodyType.PlayerWeapon:
                return m_TriggerDatas.playerWeaponTriggerList;
            case TriggerBodyType.PlayerLarge:
                return m_TriggerDatas.playerLargeTriggerList;
            default:
                return new();
        }
    }
}
