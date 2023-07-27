using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRotatePattern
{
    public void ExecuteRotatePattern(EnemyObject enemyObject);
}

public class RotatePattern_Stop : IRotatePattern
{
    public void ExecuteRotatePattern(EnemyObject enemyObject)
    {
        enemyObject.RotateUnit(enemyObject.CurrentAngle);
    }
}

public class RotatePattern_TargetPlayer : IRotatePattern
{
    private readonly float _speed;
    private readonly float _speedSub;
    
    public RotatePattern_TargetPlayer(float speed = 0f, float speedAtPlayerDead = 180f)
    {
        _speed = speed;
        _speedSub = speedAtPlayerDead;
    }
    
    public void ExecuteRotatePattern(EnemyObject enemyObject)
    {
        enemyObject.RotateUnit(enemyObject.AngleToPlayer, PlayerManager.IsPlayerAlive ? _speed : _speedSub);
    }
}

public class RotatePattern_Target_Conditional : IRotatePattern
{
    private readonly float _targetAngle;
    private readonly float _speed;
    private readonly float _targetAngleSub;
    private readonly float _speedSub;
    private readonly Func<bool> _condition;
    
    public RotatePattern_Target_Conditional(float targetAngle, float speed, Func<bool> condition, float targetAngleSub, float speedSub)
    {
        _targetAngle = targetAngle;
        _speed = speed;
        _condition = condition;
        _targetAngleSub = targetAngleSub;
        _speedSub = speedSub;
    }
    
    public void ExecuteRotatePattern(EnemyObject enemyObject)
    {
        if (_condition.Invoke())
            enemyObject.RotateUnit(_targetAngle, _speed);
        else
            enemyObject.RotateUnit(_targetAngleSub, _speedSub);
    }
}

public class RotatePattern_Target : IRotatePattern
{
    private readonly float _targetAngle;
    private readonly float _speed;
    
    public RotatePattern_Target(float targetAngle, float speed = 0f)
    {
        _targetAngle = targetAngle;
        _speed = speed;
    }
    
    public void ExecuteRotatePattern(EnemyObject enemyObject)
    {
        enemyObject.RotateUnit(_targetAngle, _speed);
    }
}

public class RotatePattern_RotateAround : IRotatePattern
{
    private readonly float _speed;
    
    public RotatePattern_RotateAround(float speed)
    {
        _speed = speed;
    }
    
    public void ExecuteRotatePattern(EnemyObject enemyObject)
    {
        var rotateSpeed = _speed;
        if (rotateSpeed > 0f)
            enemyObject.RotateUnit(enemyObject.CurrentAngle + 179f, rotateSpeed);
        else if (rotateSpeed < 0f)
            enemyObject.RotateUnit(enemyObject.CurrentAngle - 179f, rotateSpeed);
    }
}

public class RotatePattern_RotateAround_PingPong : IRotatePattern
{
    private readonly float _angleRange;
    private readonly float _speed;
    private readonly float _correctionValue;
    private float _tempAngle;
    
    public RotatePattern_RotateAround_PingPong(float currentAngle, float centerAngle, float angleRange, float speed)
    {
        _correctionValue = (centerAngle - angleRange / 2);
        _tempAngle = currentAngle - _correctionValue;
        _angleRange = angleRange;
        _speed = speed;
    }
    
    public void ExecuteRotatePattern(EnemyObject enemyObject)
    {
        _tempAngle += _speed / Application.targetFrameRate * Time.timeScale;
        //_tempAngle = Mathf.Repeat(_tempAngle, 360f);
        enemyObject.CurrentAngle = Mathf.PingPong(_tempAngle, _angleRange) + _correctionValue;
    }
}
