using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TriggerBodyType
{
    GameBoundary,
    CameraBoundary,
    PlayerCenter,
    PlayerWeapon,
    PlayerLarge,
    Enemy,
    Bullet,
    Item,
}

[DefaultExecutionOrder(-99)]
public class SimulationManager : MonoBehaviour
{
    public TriggerDatas m_TriggerDatas;
    
    private readonly Dictionary<TriggerBodyType, List<TriggerBodyType>> _triggerCollisionMasks = new();
    public static readonly HashSet<IMovable> MovableObjects = new();
    private static readonly Dictionary<TriggerBodyType, HashSet<TriggerBody>> TriggerBodies = new()
    {
        { TriggerBodyType.GameBoundary, new () },
        { TriggerBodyType.CameraBoundary, new () },
        { TriggerBodyType.PlayerCenter, new () },
        { TriggerBodyType.PlayerWeapon, new () },
        { TriggerBodyType.PlayerLarge, new () },
        { TriggerBodyType.Enemy, new () },
        { TriggerBodyType.Bullet, new () },
        { TriggerBodyType.Item, new () },
    };

    private static readonly Queue<TriggerBody> TriggerBodiesToRemove = new();

    private readonly List<TriggerBodyType> _triggerBodyTypes = new()
    {
        TriggerBodyType.GameBoundary,
        TriggerBodyType.CameraBoundary,
        TriggerBodyType.PlayerCenter,
        TriggerBodyType.PlayerWeapon,
        TriggerBodyType.PlayerLarge
    };

    private void Start()
    {
        _triggerCollisionMasks[TriggerBodyType.GameBoundary] = m_TriggerDatas.gameBoundaryTriggerList;
        _triggerCollisionMasks[TriggerBodyType.CameraBoundary] = m_TriggerDatas.cameraBoundaryTriggerList;
        _triggerCollisionMasks[TriggerBodyType.PlayerCenter] = m_TriggerDatas.playerCenterTriggerList;
        _triggerCollisionMasks[TriggerBodyType.PlayerWeapon] = m_TriggerDatas.playerWeaponTriggerList;
        _triggerCollisionMasks[TriggerBodyType.PlayerLarge] = m_TriggerDatas.playerLargeTriggerList;
    }

    private void Update()
    {
        //SimulateMovement();
        SimulateOnTriggerBodyInit();

        RemoveTriggerBody();
        //SimulateOnTriggerBodyStay();
    }

    private void SimulateMovement()
    {
        foreach (var movableObject in MovableObjects)
        {
            movableObject.MoveTriggerBody();
        }
    }

    private void SimulateOnTriggerBodyInit()
    {
        foreach (var triggerBodyType in _triggerBodyTypes)
        {
            var masks = _triggerCollisionMasks[triggerBodyType];
            
            foreach (var otherTriggerBodyType in masks)
            {
                ExecuteCheckOverlapTriggerBody(triggerBodyType, otherTriggerBodyType);
            }
        }
    }

    private void RemoveTriggerBody()
    {
        while (TriggerBodiesToRemove.Count > 0)
        {
            var triggerBody = TriggerBodiesToRemove.Dequeue();
            TriggerBodies[triggerBody.m_TriggerBodyType].Remove(triggerBody);
        }
    }

    private void SimulateOnTriggerBodyStay()
    {
        foreach (var triggerBody in TriggerBodies[TriggerBodyType.PlayerWeapon])
        {
            triggerBody.OnTriggerBodyCollisionStay();
        }
    }

    private void ExecuteCheckOverlapTriggerBody(TriggerBodyType triggerBodyType, TriggerBodyType otherTriggerBodyType)
    {
        foreach (var triggerBody in TriggerBodies[triggerBodyType])
        {
            foreach (var otherTriggerBody in TriggerBodies[otherTriggerBodyType])
            {
                if (triggerBody == null || otherTriggerBody == null)
                    continue;
                TriggerBodyManager.CheckOverlapTriggerBody(triggerBody, otherTriggerBody);
            }
        }
    }

/*
    private List<TriggerBodyType> GetTriggerList(TriggerBodyType triggerBodyType)
    {
        switch (triggerBodyType)
        {
            case TriggerBodyType.GameBoundary:
                return m_TriggerDatas.gameBoundaryTriggerList;
            case TriggerBodyType.CameraBoundary:
                return m_TriggerDatas.cameraBoundaryTriggerList;
            case TriggerBodyType.PlayerCenter:
                return m_TriggerDatas.playerCenterTriggerList;
            case TriggerBodyType.PlayerWeapon:
                return m_TriggerDatas.playerWeaponTriggerList;
            case TriggerBodyType.PlayerLarge:
                return m_TriggerDatas.playerLargeTriggerList;
            default:
                Debug.LogError($"Unknown trigger body type detected: {triggerBodyType}");
                return new();
        }
    }
*/

    public static void AddTriggerBody(TriggerBody triggerBody)
    {
        TriggerBodies[triggerBody.m_TriggerBodyType].Add(triggerBody);
    }

    public static void RemoveTriggerBody(TriggerBody triggerBody)
    {
        TriggerBodiesToRemove.Enqueue(triggerBody);
    }
}
