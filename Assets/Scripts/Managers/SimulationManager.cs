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
    
    private static readonly List<TriggerBody> _iterateBuffer = new();

    private static readonly SpatialGrid _spatialGrid = new();

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
        InitSpatialGrids();

        Simulate();

        //Debug.Log(_spatialGrid);
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
        foreach (var triggerBodyType in _triggerBodyTypes)
        {
            if (!TriggerBodies.TryGetValue(triggerBodyType, out var hashSet))
                continue;

            _iterateBuffer.Clear();
            _iterateBuffer.AddRange(hashSet);

            foreach (var body in _iterateBuffer)
            {
                if (!body || !body.isActiveAndEnabled)
                    continue;

                var nearByResult = _spatialGrid.GetNearbyTriggerBodies(body);

                foreach (var near in nearByResult)
                {
                    if (!near || !near.isActiveAndEnabled)
                        continue;
                    
                    if (!_triggerCollisionMasks.TryGetValue(body.m_TriggerBodyType, out var mask))
                        continue;

                    if (!mask.Contains(near.m_TriggerBodyType))
                        continue;

                    TriggerBodyManager.CheckOverlapTriggerBody(body, near);
                }
            }
        }
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
                TriggerBodyManager.CheckOverlapTriggerBody(triggerBody, otherTriggerBody);
            }
        }
    }

    public static void AddTriggerBody(TriggerBody triggerBody)
    {
        TriggerBodies[triggerBody.m_TriggerBodyType].Add(triggerBody);
    }

    public static void RemoveTriggerBody(TriggerBody triggerBody)
    {
        TriggerBodies[triggerBody.m_TriggerBodyType].Remove(triggerBody);
    }
}
