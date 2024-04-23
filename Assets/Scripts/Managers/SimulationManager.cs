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
    
    public static readonly LinkedList<MovableObject> MovableObjects = new();
    public static readonly Dictionary<TriggerBodyType, LinkedList<TriggerBody>> TriggerBodies = new()
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
            var outerList = TriggerBodies[triggerBodyType];
            var outerNode = outerList.First;
            while (outerNode != null)
            {
                var outerNext = outerNode.Next;
                var triggerBody = outerNode.Value;
                
                var innerList = TriggerBodies[otherTriggerBodyType];
                var innerNode = innerList.First;
                while (innerNode != null)
                {
                    var innerNext = innerNode.Next;
                    var otherTriggerBody = innerNode.Value;
                    
                    TriggerBodyManager.CheckOverlapTriggerBody(triggerBody, otherTriggerBody);
                    innerNode = innerNext;
                }

                outerNode = outerNext;
            }
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
