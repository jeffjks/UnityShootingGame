using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DebugEnemy : MonoBehaviour
{
    public TextMeshProUGUI m_DifficultyText;
    public TextMeshProUGUI m_EnemyText;
    public EnemyUnitPrefabDatas m_EnemyUnitPrefabData;
    public List<AnimationCurve> m_AnimationCurves;

    private readonly List<EnemyUnitPrefabDatas.EnemyUnitPrefab> _enemyPrefabs = new();
    private EnemyUnit _currentEnemyUnit;
    private int _enemyIndex;

    private int EnemyIndex
    {
        get => _enemyIndex;
        set
        {
            _enemyIndex = value;
            _enemyIndex = Mathf.Clamp(_enemyIndex, 0, 4);
            m_EnemyText.SetText($"{_enemyPrefabs[_enemyIndex].enemyName}");
        }
    }

    private void Start()
    {
        foreach (var animationCurve in m_AnimationCurves)
        {
            AC_Ease.ac_ease.Add(animationCurve);
        }

        GameManager.IsDebugScene = true;
        
        _enemyPrefabs.AddRange(m_EnemyUnitPrefabData.AirEnemyPrefabs);
        _enemyPrefabs.AddRange(m_EnemyUnitPrefabData.GroundEnemyPrefabs);
        CreateEnemy();
        OnUpdateDifficulty();
    }

    public void OnClickNextDifficulty()
    {
        SystemManager.SetDifficulty(SystemManager.Difficulty.GetEnumNext(false));
        OnUpdateDifficulty();
    }

    public void OnClickPrevDifficulty()
    {
        SystemManager.SetDifficulty(SystemManager.Difficulty.GetEnumPrev(false));
        OnUpdateDifficulty();
    }

    private void OnUpdateDifficulty()
    {
        m_DifficultyText.SetText(SystemManager.Difficulty.ToString());
    }

    public void OnClickNextEnemy()
    {
        RemoveEnemy();
        EnemyIndex--;
    }

    public void OnClickPrevEnemy()
    {
        RemoveEnemy();
        EnemyIndex++;
    }

    private void RemoveEnemy()
    {
        if (!_currentEnemyUnit)
            return;
        _currentEnemyUnit.m_EnemyDeath.RemoveEnemy();
        _currentEnemyUnit = null;
    }

    public void OnClickNextPhase()
    {
    }

    public void OnClickCreateEnemy()
    {
        CreateEnemy();
    }

    private void CreateEnemy()
    {
        if (_currentEnemyUnit)
        {
            Debug.LogWarning($"Enemy unit already exists.");
            return;
        }
        var enemyPrefab = _enemyPrefabs[_enemyIndex];
        _currentEnemyUnit = Instantiate(enemyPrefab.enemyUnit, enemyPrefab.defaultPosition, Quaternion.identity);

        if (_currentEnemyUnit.m_IsAir)
        {
            var trans = _currentEnemyUnit.transform;
            var pos = trans.position;
            pos.z = Depth.ENEMY;
            trans.position = pos;
        }

        if (_currentEnemyUnit is ITargetPosition targetingEnemyUnit)
        {
            targetingEnemyUnit.MoveTowardsToTarget(new Vector2(0f, -3f), 1200);
        }
    }

    public void OnClickKillEnemy()
    {
        if (!_currentEnemyUnit)
        {
            Debug.LogWarning($"There is no enemy unit.");
            return;
        }
        _currentEnemyUnit.m_EnemyDeath.KillEnemy();
        _currentEnemyUnit = null;
    }
}
