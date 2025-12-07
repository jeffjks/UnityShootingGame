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
    
    private readonly Dictionary<TriggerBodyType, HashSet<TriggerBodyType>> _triggerCollisionMasks = new();
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

    private static readonly SpatialGrid _spatialGrid = new();
    /*
    private static readonly Dictionary<TriggerBodyType, SpatialGrid> _spatialGrids = new()
    {
        { TriggerBodyType.GameBoundary, new (TriggerBodyType.GameBoundary) },
        { TriggerBodyType.CameraBoundary, new (TriggerBodyType.CameraBoundary) },
        { TriggerBodyType.PlayerCenter, new (TriggerBodyType.PlayerCenter) },
        { TriggerBodyType.PlayerWeapon, new (TriggerBodyType.PlayerWeapon) },
        { TriggerBodyType.PlayerLarge, new (TriggerBodyType.PlayerLarge) },
        { TriggerBodyType.Enemy, new (TriggerBodyType.Enemy) },
        { TriggerBodyType.Bullet, new (TriggerBodyType.Bullet) },
        { TriggerBodyType.Item, new (TriggerBodyType.Item) },
    };*/

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
        _triggerCollisionMasks[TriggerBodyType.GameBoundary] = new (m_TriggerDatas.gameBoundaryTriggerList);
        _triggerCollisionMasks[TriggerBodyType.CameraBoundary] = new (m_TriggerDatas.cameraBoundaryTriggerList);
        _triggerCollisionMasks[TriggerBodyType.PlayerCenter] = new (m_TriggerDatas.playerCenterTriggerList);
        _triggerCollisionMasks[TriggerBodyType.PlayerWeapon] = new (m_TriggerDatas.playerWeaponTriggerList);
        _triggerCollisionMasks[TriggerBodyType.PlayerLarge] = new (m_TriggerDatas.playerLargeTriggerList);
    }

    private void Update()
    {
        //SimulateMovement();

        RemoveTriggerBody();
        
        InitSpatialGrids();

        Simulate();

        //Debug.Log(_spatialGrid);
        //SimulateOnTriggerBodyStay();
    }

    private void SimulateMovement()
    {
        foreach (var movableObject in MovableObjects)
        {
            movableObject.MoveTriggerBody();
        }
    }

    private void InitSpatialGrids()
    {
        _spatialGrid.Clear();

        foreach (var item in TriggerBodies)
        {
            var triggerBodyType = item.Key;
            foreach (var body in TriggerBodies[triggerBodyType])
            {
                _spatialGrid.Add(body);
            }
        }
    }

    private void Simulate()
    {
        var str = string.Empty;
        foreach (var triggerBodyType in _triggerBodyTypes)
        {
            //if (triggerBodyType == TriggerBodyType.PlayerCenter)
                //str += $"[{triggerBodyType}]\n";
            foreach (var body in TriggerBodies[triggerBodyType])
            {
                var nearByResult = _spatialGrid.GetNearbyTriggerBodies(body);

                foreach (var near in nearByResult)
                {
                    //if (body.m_TriggerBodyType == TriggerBodyType.PlayerWeapon && near.m_TriggerBodyType == TriggerBodyType.Enemy)
                    //    Debug.Log($"AAA: {body}, {near}");
                    var result = false;
                    if (_triggerCollisionMasks[body.m_TriggerBodyType].Contains(near.m_TriggerBodyType))
                    {
                        result = TriggerBodyManager.CheckOverlapTriggerBody(body, near);
                    }
                    // if (triggerBodyType == TriggerBodyType.PlayerWeapon && near.m_TriggerBodyType == TriggerBodyType.Enemy)
                    // {
                    //     str += $"{body} -> {near} ({near.m_TriggerBodyType}), {result}\n";
                    // }
                }
            }
        }
        //Debug.Log(str);
    }

    private void SimulateOnTriggerBodyInitLegacy()
    {
        foreach (var triggerBodyType in _triggerBodyTypes)
        {
            var masks = _triggerCollisionMasks[triggerBodyType];
            
            foreach (var otherTriggerBodyType in masks)
            {
                ExecuteCheckOverlapTriggerBodyLegacy(triggerBodyType, otherTriggerBodyType);
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

    private void ExecuteCheckOverlapTriggerBody(TriggerBodyType triggerBodyType)
    {
        foreach (var triggerBody in TriggerBodies[triggerBodyType])
        {
            if (triggerBody == null)
                continue;
            
            var otherTriggerBodies = _spatialGrid.GetNearbyTriggerBodies(triggerBody);
            foreach (var otherTriggerBody in otherTriggerBodies)
            {
                //if (triggerBody == null || otherTriggerBody == null)
                //    continue;
                TriggerBodyManager.CheckOverlapTriggerBody(triggerBody, otherTriggerBody);
            }
        }
    }

    private void ExecuteCheckOverlapTriggerBodyLegacy(TriggerBodyType triggerBodyType, TriggerBodyType otherTriggerBodyType)
    {
        foreach (var triggerBody in TriggerBodies[triggerBodyType])
        {
            foreach (var otherTriggerBody in TriggerBodies[otherTriggerBodyType])
            {
                //if (triggerBody == null || otherTriggerBody == null)
                //    continue;
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
