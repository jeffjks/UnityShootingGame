using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitObject : MonoBehaviour
{
    public MoveVector m_MoveVector;
    [DrawIf("m_IsRoot", true, ComparisonType.Equals)]
    public bool m_IsAir;

	//[HideInInspector] public Vector2 m_Position2D;
	protected float _currentAngle; // 현재 회전 각도

    public float AngleToPlayer => GetAngleToTarget(m_Position2D, PlayerManager.GetPlayerPosition());

    public virtual float CurrentAngle
    {
        get => _currentAngle;
        set => _currentAngle = value;
    }
    public Vector2 m_Position2D => GetPosition2d();

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

    // Rotate Methods ---------------------------------- TODO. have to remove
    
    public void RotateSlightly(Vector2 target, float speed, float rot = 0f)
    {
        // if (!IsRotatable)
        //     return;
        // if (m_EnemyDeath.m_IsDead)
        //     return;
        // float target_angle = GetAngleToTarget(m_Position2D, target);
        // CurrentAngle = Mathf.MoveTowardsAngle(CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
    }

    public void RotateSlightly(float target_angle, float speed, float rot = 0f)
    {
        // if (!IsRotatable)
        //     return;
        // if (m_EnemyDeath.m_IsDead)
        //     return;
        // CurrentAngle = Mathf.MoveTowardsAngle(CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
    }

    public void RotateImmediately(Vector2 target, float rot = 0f)
    {
        // if (!IsRotatable)
        //     return;
        // if (m_EnemyDeath.m_IsDead)
        //     return;
        // float target_angle = GetAngleToTarget(m_Position2D, target);
        // CurrentAngle = target_angle + rot;
    }

    public void RotateImmediately(float target_angle, float rot = 0f)
    {
        
        // if (!IsRotatable)
        //     return;
        // if (m_EnemyDeath.m_IsDead)
        //     return;
        // CurrentAngle = target_angle + rot;
    }
}


// ================ 적 ================ //

public abstract class EnemyObject : UnitObject // 적 개체 + 총알
{
    public Transform[] m_FirePosition;
    protected bool _isInteractable = true;
    protected IRotatePattern _rotatePattern;
    private const float SAFE_LINE = -11f;
    
    private float _customDirection;
    public float CustomDirection
    {
        get => _customDirection;
        set
        {
            _customDirection = value;
            _customDirection = Mathf.Repeat(_customDirection, 360f);
        }
    }
    
    public bool IsInteractable() {
        return _isInteractable;
    }


    // Type 0 총알
    protected GameObject[] CreateBulletsSector(byte image, Vector3 pos, float speed, float direction, BulletAccel accel, int num, float interval) {
        GameObject[] objs = new GameObject[num];
        /*
        if (CanCreateBullet(pos)) {
            for (int i = 0; i < num; i++) {
                objs[i] = CreateBullet(image, pos, speed, direction - interval*(num - i*2 - 1)/2, accel);
            }
        }*/
        return objs;
    }

    // Type (0), 1, 2 총알
    protected GameObject[] CreateBulletsSector(byte image, Vector3 pos, float speed, float direction, BulletAccel accel, int num, float interval,
        BulletSpawnType spawnType, int timer, byte new_image, float new_speed, BulletPivot pivot, float direction_add, BulletAccel new_accel,
    int new_num = 0, float new_interval = 0f, Vector2Int second_timer = new Vector2Int()) {
        GameObject[] objs = new GameObject[num];
        /*
        if (CanCreateBullet(pos)) {
            for (int i = 0; i < num; i++) {
                objs[i] = CreateBullet(image, pos, speed, direction - interval*(num - i*2 - 1)/2, accel,
                    spawnType, timer, new_image, new_speed, pivot, direction_add, new_accel, new_num, new_interval, second_timer);
            }
        }*/
        return objs;
    }

    // Type 0 총알
    protected GameObject CreateBullet(byte image, Vector3 pos, float speed, float direction, BulletAccel accel)
    {
        GameObject obj = null;/*
        if (CanCreateBullet(pos)) {
            pos.z = Depth.ENEMY_BULLET;
            
            obj = PoolingManager.PopFromPool("EnemyBullet", PoolingParent.EnemyBullet);
            EnemyBullet enemyBullet = obj.GetComponent<EnemyBullet>();
            BulletImage bulletImage = (BulletImage) image;
            enemyBullet.transform.position = pos;
            enemyBullet.m_MoveVector = new MoveVector(speed, direction);
            //enemyBullet.m_BulletAccel = accel;

            //enemyBullet.m_Type = 0;
            //enemyBullet.m_Timer = 0;
            
            obj.SetActive(true);
            //enemyBullet.OnStart(bulletImage, null);
        }*/
        return obj;
    }

    // Type (0), 1, 2 총알
    protected GameObject CreateBullet(byte image, Vector3 pos, float speed, float direction, BulletAccel accel,
        BulletSpawnType spawnType, int timer, byte new_image, float new_speed, BulletPivot pivot, float direction_add, BulletAccel new_accel,
    int new_num = 0, float new_interval = 0f, Vector2Int second_timer = new Vector2Int())
    {
        GameObject obj = null;
        /*
        if (CanCreateBullet(pos)) {
            pos.z = Depth.ENEMY_BULLET;
            
            obj = PoolingManager.PopFromPool("EnemyBullet", PoolingParent.EnemyBullet);
            EnemyBullet enemyBullet = obj.GetComponent<EnemyBullet>();
            BulletImage bulletImage = (BulletImage) image;
            enemyBullet.transform.position = pos;
            enemyBullet.m_MoveVector = new MoveVector(speed, direction);
            enemyBullet.m_BulletAccel = accel;

            enemyBullet.m_Type = spawnType;
            enemyBullet.m_Timer = timer;

            enemyBullet.m_NewImageType = new_image;
            enemyBullet.m_NewMoveVector = new MoveVector(new_speed, 0);
            enemyBullet.m_NewDirectionType = pivot;
            enemyBullet.m_NewDirectionAdder = direction_add;
            enemyBullet.m_NewBulletAccel = new_accel;

            enemyBullet.m_SecondTimer = second_timer;
            enemyBullet.m_NewNumber = new_num;
            enemyBullet.m_NewInterval = new_interval;
            
            obj.SetActive(true);
            enemyBullet.OnStart(bulletImage, null);
        }*/
        return obj;
    }

    protected virtual bool CanCreateBullet(Vector3 pos) {
        float camera_x = MainCamera.Camera.transform.position.x;

        if (!PlayerManager.IsPlayerAlive)
            return false;
        if (!SystemManager.IsOnGamePlayState())
            return false;
        if (2 * Mathf.Abs(pos.x - camera_x) > Size.MAIN_CAMERA_WIDTH)
            return false;
        if (pos.y is > 0 or < SAFE_LINE)
            return false;
        
        return true;
    }

    public void SetRotatePattern(IRotatePattern rotatePattern)
    {
        _rotatePattern = rotatePattern;
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
    public int DamageLevel {
        set
        {
            _damageLevel = value;
            OnDamageLevelChanged();
        }
    }

    private void Awake()
    {
        _maxDamageLevel = _playerDamageData.damageByLevel.Count - 1;
    }

    protected virtual void OnDamageLevelChanged()
    {
        Damage = _playerDamageData.damageByLevel[_damageLevel];
    }
    
    protected void DealDamage(EnemyUnit enemyObject)
    {
        var damageScale = _playerDamageData.damageScale[enemyObject.m_EnemyType];
        var finalDamage = Damage * damageScale / 100;
        var damage_type = _playerDamageData.playerDamageType;
        enemyObject.m_EnemyHealth.TakeDamage(finalDamage, damage_type);
    }
}


