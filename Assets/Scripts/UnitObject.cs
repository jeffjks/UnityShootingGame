using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

[BurstCompile(FloatPrecision.Standard, FloatMode.Deterministic)]
[DefaultExecutionOrder(-100)]
public abstract class UnitObject : MonoBehaviour
{
    public MoveVector m_MoveVector;
    //[DrawIf("m_IsRoot", true, ComparisonType.Equals)]
    //public Rigidbody2D m_Rigidbody2D;
    [DrawIf("m_IsRoot", true, ComparisonType.Equals)]
    public bool m_IsAir;

	//[HideInInspector] public Vector2 m_Position2D;
	protected float _currentAngle; // 현재 회전 각도

    public float AngleToPlayer => GetAngleToTarget(Position2D, PlayerManager.GetPlayerPosition());

    public virtual float CurrentAngle
    {
        get => _currentAngle;
        set => _currentAngle = value;
    }
    public Vector2 Position2D => GetPosition2d();

    public virtual void ExecuteCollisionEnter(int targetId) { }

    public virtual void ExecuteCollisionExit(int targetId) { }

    protected void MoveDirection(float speed, float direction) // speed 속도로 direction 방향으로 이동. 0도는 아래, 90도는 오른쪽
    {
        if (!m_IsAir) {
            Vector3 vector3 = Quaternion.AngleAxis(direction, Vector3.down) * Vector3.back;
            transform.Translate(vector3 * speed / Application.targetFrameRate * Time.timeScale, Space.World);
        }
        else {
            Vector2 vector2 = Quaternion.AngleAxis(direction, Vector3.forward) * Vector2.down;
            transform.Translate(vector2 * speed / Application.targetFrameRate * Time.timeScale, Space.World);
        }
        //m_Rigidbody2D.MovePosition(Position2D);
    }

    protected float GetAngleToTarget(Vector2 pos, Vector2 target) {
        // pos에서 target을 향하는 각도 (-180 ~ 180 범위)
        Vector2 point_direction_vector = target - pos;
        float target_player = Vector2.SignedAngle(Vector2.down, point_direction_vector);
        return target_player;
    }

    private Vector2 GetPosition2d()
    {
        if (m_IsAir)
        {
            return transform.position;
        }
        return BackgroundCamera.GetScreenPosition(transform.position);
    }

    public void RotateUnit(float targetAngle, float speed = 0f)
    {
        if (speed <= 0f)
        {
            CurrentAngle = targetAngle;
            return;
        }
        CurrentAngle = Mathf.MoveTowardsAngle(CurrentAngle, targetAngle, speed / Application.targetFrameRate * Time.timeScale);
    }
}


// ================ 적 ================ //

public abstract class EnemyObject : UnitObject // 적 개체 + 총알
{
    public Transform[] m_FirePosition;
    protected bool m_IsInteractable = true;
    protected IRotatePattern m_RotatePattern;
    private const float SAFE_LINE = -11f;

    public CustomDirection m_CustomDirection;
    public bool TimeLimitState { get; set; }

    public class CustomDirection
    {
        private readonly float[] _array;

        public CustomDirection()
        {
            _array = new float[1];
        }

        public CustomDirection(int size)
        {
            _array = new float[size];
        }
        
        public float this[int index]
        {
            get => _array[index];
            set
            {
                _array[index] = value;
                _array[index] = Mathf.Repeat(_array[index], 360f);
            }
        }
 
        public int Length => _array.Length;
    }
    
    public bool IsInteractable() {
        return m_IsInteractable;
    }

    public void SetRotatePattern(IRotatePattern rotatePattern)
    {
        m_RotatePattern = rotatePattern;
        m_RotatePattern.ExecuteRotatePattern(this);
    }
}



// ================ 적에게 데미지를 주는 개체 ================ //

public abstract class PlayerObject : UnitObject
{
    public string m_ObjectName;
    public int Damage { get; protected set; }
    
    [SerializeField] protected PlayerDamageDatas _playerDamageData;
    
    protected int _maxDamageLevel;
    protected int _damageLevel;
    public int DamageLevel
    {
        set
        {
            _damageLevel = value;
            OnDamageLevelChanged();
        }
    }

    private void Awake()
    {
        _maxDamageLevel = _playerDamageData.damageByLevel.Count - 1;
        OnDamageLevelChanged();
    }

    protected virtual void OnDamageLevelChanged()
    {
        Damage = _playerDamageData.damageByLevel[_damageLevel];
    }
    
    protected void DealDamage(EnemyUnit enemyObject)
    {
        var damageScale = _playerDamageData.damageScale[enemyObject.m_EnemyType];
        var finalDamage = Damage * damageScale / 100;
        var damageType = _playerDamageData.playerDamageType;
        enemyObject.m_EnemyHealth.TakeDamage(finalDamage, damageType);
    }
}


