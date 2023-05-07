using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitObject : MonoBehaviour
{
    public MoveVector m_MoveVector;

	[HideInInspector] public Vector2 m_Position2D;
	[HideInInspector] public float m_CurrentAngle; // 현재 회전 각도

    protected void MoveDirection(float speed, float direction) // speed 속도로 direction 방향으로 이동. 0도는 아래, 90도는 오른쪽
    {
        if ((1 << gameObject.layer & Layer.GROUND) != 0) { // Ground
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
}

public interface IRotatable
{
    void RotateSlightly(Vector2 target, float speed, float rot = 0f);
    void RotateSlightly(float target_angle, float speed, float rot = 0f);
    void RotateImmediately(Vector2 target, float rot = 0f);
    void RotateImmediately(float target_angle, float rot = 0f);
    void UpdateTransform();
    void SetPosition2D();
}




// ================ 적 ================ //

public abstract class EnemyObject : UnitObject { // 적 개체 + 총알

    protected SystemManager m_SystemManager = null;
    protected PlayerManager m_PlayerManager = null;
    protected PoolingManager m_PoolingManager = null;
    protected Vector2 m_PlayerPosition, m_BackgroundCameraSize;

    protected const float NO_CHANGE = 8739f;
    private const float SAFE_LINE = -11f;

    protected virtual void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;

        GetPlayerPosition2D();
        m_BackgroundCameraSize = m_SystemManager.m_BackgroundCameraSize;
    }

    protected Vector2 GetScreenPosition(Vector3 pos) { // Only Ground Units
        float main_camera_xpos = m_SystemManager.m_MainCamera.transform.position.x;
        Vector3 screen_pos = m_SystemManager.m_BackgroundCamera.WorldToScreenPoint(pos);
        Vector2 modified_pos = new Vector2(
            screen_pos[0]*m_BackgroundCameraSize.x/Screen.width - m_BackgroundCameraSize.x/2 + main_camera_xpos,
            screen_pos[1]*m_BackgroundCameraSize.y/Screen.height - m_BackgroundCameraSize.y
            );
        return modified_pos;
    }


    // Type 0 총알
    protected GameObject[] CreateBulletsSector(byte image, Vector3 pos, float speed, float direction, EnemyBulletAccel accel, int num, float interval) {
        GameObject[] objs = new GameObject[num];
        if (BulletCondition(pos)) {
            for (int i = 0; i < num; i++) {
                objs[i] = CreateBullet(image, pos, speed, direction - interval*(num - i*2 - 1)/2, accel);
            }
        }
        return objs;
    }

    // Type (0), 1, 2 총알
    protected GameObject[] CreateBulletsSector(byte image, Vector3 pos, float speed, float direction, EnemyBulletAccel accel, int num, float interval,
    byte type, int timer, byte new_image, float new_speed, byte new_direction, float direction_add, EnemyBulletAccel new_accel,
    int new_num = 0, float new_interval = 0f, Vector2Int second_timer = new Vector2Int()) {
        GameObject[] objs = new GameObject[num];
        if (BulletCondition(pos)) {
            for (int i = 0; i < num; i++) {
                objs[i] = CreateBullet(image, pos, speed, direction - interval*(num - i*2 - 1)/2, accel,
                type, timer, new_image, new_speed, new_direction, direction_add, new_accel, new_num, new_interval, second_timer);
            }
        }
        return objs;
    }

    // Type 0 총알
    protected GameObject CreateBullet(byte image, Vector3 pos, float speed, float direction, EnemyBulletAccel accel)
    {
        GameObject obj = null;
        if (BulletCondition(pos)) {
            pos.z = Depth.ENEMY_BULLET;
            
            obj = m_PoolingManager.PopFromPool("EnemyBullet", PoolingParent.ENEMY_BULLET);
            EnemyBullet enemyBullet = obj.GetComponent<EnemyBullet>();
            enemyBullet.m_ImageType = image;
            enemyBullet.transform.position = pos;
            enemyBullet.m_MoveVector = new MoveVector(speed, direction);
            enemyBullet.m_EnemyBulletAccel = accel;

            enemyBullet.m_Type = 0;
            enemyBullet.m_Timer = 0;

            m_SystemManager.AddBullet();
            obj.SetActive(true);
            enemyBullet.OnStart();
        }
        return obj;
    }

    // Type (0), 1, 2 총알
    protected GameObject CreateBullet(byte image, Vector3 pos, float speed, float direction, EnemyBulletAccel accel,
    byte type, int timer, byte new_image, float new_speed, byte new_direction, float direction_add, EnemyBulletAccel new_accel,
    int new_num = 0, float new_interval = 0f, Vector2Int second_timer = new Vector2Int())
    {
        GameObject obj = null;
        if (BulletCondition(pos)) {
            pos.z = Depth.ENEMY_BULLET;
            
            obj = m_PoolingManager.PopFromPool("EnemyBullet", PoolingParent.ENEMY_BULLET);
            EnemyBullet enemyBullet = obj.GetComponent<EnemyBullet>();
            enemyBullet.m_ImageType = image;
            enemyBullet.transform.position = pos;
            enemyBullet.m_MoveVector = new MoveVector(speed, direction);
            enemyBullet.m_EnemyBulletAccel = accel;

            enemyBullet.m_Type = type;
            enemyBullet.m_Timer = timer;

            enemyBullet.m_NewImageType = new_image;
            enemyBullet.m_NewMoveVector = new MoveVector(new_speed, 0);
            enemyBullet.m_NewDirectionType = new_direction;
            enemyBullet.m_NewDirectionAdder = direction_add;
            enemyBullet.m_NewEnemyBulletAccel = new_accel;

            enemyBullet.m_SecondTimer = second_timer;
            enemyBullet.m_NewNumber = new_num;
            enemyBullet.m_NewInterval = new_interval;

            m_SystemManager.AddBullet();
            obj.SetActive(true);
            enemyBullet.OnStart();
        }
        return obj;
    }

    protected virtual bool BulletCondition(Vector3 pos) {
        float camera_x = m_SystemManager.m_MainCamera.transform.position.x;

        if (!m_PlayerManager.m_PlayerIsAlive) {
            return false;
        }
        else if (m_SystemManager.m_PlayState >= 2) {
            return false;
        }
        else if (2 * Mathf.Abs(pos.x - camera_x) > Size.CAMERA_WIDTH) {
            return false;
        }
        else if (pos.y < SAFE_LINE || 0 < pos.y) {
            return false;
        }
        return true;
    }

    protected void GetPlayerPosition2D() {
        m_PlayerPosition = m_PlayerManager.GetPlayerPosition();
    }
}



// ================ 적에게 데미지를 주는 개체 ================ //

public abstract class PlayerObject : UnitObject
{
    public string m_ObjectName;
    public int m_Damage;
    [Header("단위: %")]
    public int[] m_DamageScale = new int[3];
    
    protected PlayerManager m_PlayerManager = null;
    protected PoolingManager m_PoolingManager = null;
    protected int m_DefaultDamage;

    protected virtual void Awake()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        m_DefaultDamage = m_Damage;

        //m_PositionInt2D = Vector2Int.RoundToInt(transform.position*256);
    }
    
    protected void DealDamage(EnemyUnit enemyObject, int damage, PlayerDamageType damage_type = PlayerDamageType.Normal) {
        try {
            enemyObject.m_EnemyHealth.TakeDamage(damage * m_DamageScale[(int) enemyObject.m_EnemyType] / 100, damage_type);
        }
        catch (System.IndexOutOfRangeException) {
            enemyObject.m_EnemyHealth.TakeDamage(damage, damage_type);
        }
    }
}


