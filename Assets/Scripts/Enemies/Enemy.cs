using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletDirection
{
    public const byte FIXED = 0;
    public const byte PLAYER = 1;
    public const byte CURRENT = 2;
}

public struct MoveVector
{
    public float speed, direction;

    public MoveVector(float speed, float direction) {
        this.speed = speed;
        this.direction = direction;
    }

    public MoveVector(Vector2 vector2) {
        this.speed = vector2.magnitude;
        this.direction = Vector2.SignedAngle(Vector2.down, vector2);
    }

    public Vector2 GetVector() {
        Vector2 vector2 = Quaternion.AngleAxis(this.direction, Vector3.forward) * Vector2.down;
        vector2 = vector2.normalized * speed;
        return vector2;
    }
}

public class EaseType
{
    public const int Linear = 0;
    public const int OutQuad = 1;
    public const int InQuad = 2;
    public const int InOutQuad = 3;
}

/*
public class TweenData<T, TResult> {
    public Func<T> getter;
    public Action<TResult> setter;

    TweenData(Func<T> getter, Action<TResult> setter) {
        this.getter = getter;
        this.setter = setter;
    }
}*/

public class TweenData
{
    public int duration; // duration동안 대기(TweenData) 혹은 duration에 걸쳐서 변화(TweenDataMovement)

    public TweenData(int duration) {
        this.duration = duration;
    }
}

public class TweenDataMoveVector : TweenData
{
    public MoveVector moveVector;
    public int easeType = EaseType.Linear;

    public TweenDataMoveVector(MoveVector moveVector, int duration = 0, int easeType = EaseType.Linear) : base(duration) {
        this.moveVector = moveVector;
        this.duration = duration;
        this.easeType = easeType;
    }
}

public class TweenDataPosition : TweenData
{
    public Vector3 position;
    public int easeType = EaseType.Linear;

    public TweenDataPosition(Vector3 position, int duration = 0, int easeType = EaseType.Linear) : base(duration) {
        this.position = position;
        this.duration = duration;
        this.easeType = easeType;
    }
}

public enum EnemyClass
{
    Zako,
    MiddleBoss,
    Boss,
}

public interface CanDeath {
    void OnDeath();
}

// ================ 적 ================ //

public abstract class Enemy : MonoBehaviour { // 총알

    public MoveVector m_MoveVector;
    [HideInInspector] public Vector2 m_Position2D;
    [HideInInspector] public bool m_IsUnattackable;

    protected SystemManager m_SystemManager = null;
    protected PlayerManager m_PlayerManager = null;
    protected PoolingManager m_PoolingManager = null;
    protected Vector2 m_PlayerPosition, m_BackgroundCameraSize;
    protected const float NO_CHANGE = 8739f;

    private float m_SafeLine;

    protected virtual void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;

        m_PlayerPosition = m_PlayerManager.m_Player.transform.position;
        m_BackgroundCameraSize = m_SystemManager.m_BackgroundCameraSize;
        m_SafeLine = m_PlayerManager.m_SafeLine;
    }

    protected Vector2 GetScreenPosition(Vector3 pos) { // Only Ground Units
        float main_camera_xpos = m_SystemManager.m_MainCamera.transform.position.x;
        Vector3 screen_pos = m_SystemManager.m_BackgroundCamera.WorldToScreenPoint(pos);
        Vector2 modified_pos = new Vector2(
            screen_pos[0]*m_BackgroundCameraSize.x/Screen.width - m_BackgroundCameraSize.x/2 + main_camera_xpos,
            screen_pos[1]*m_BackgroundCameraSize.y/Screen.height - m_BackgroundCameraSize.y);
        return modified_pos;
    }

    protected float GetAngleToTarget(Vector2 pos, Vector2 target) {
        // pos에서 target을 향하는 각도 (-180 ~ 180 범위)
        Vector2 point_direction_vector = target - pos;
        float target_player = Vector2.SignedAngle(Vector2.down, point_direction_vector);
        return target_player;
    }

    protected virtual void MoveDirection(float speed, float direction)
    {
        /* 총알용 MoveDirection */
        Vector2 vector2 = Quaternion.AngleAxis(direction, Vector3.forward) * Vector2.down;
        transform.Translate(vector2 * speed / Application.targetFrameRate * Time.timeScale, Space.World);
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
        else if (pos.y > 0 || pos.y < m_SafeLine) {
            return false;
        }
        return true;
    }
}

// ============================================================================================ //

public abstract class EnemyUnit : Enemy, CanDeath // 적 개체, 포탑 (적 총알 제외)
{
    private enum Debris
    {
        None,
        Small,
        Medium,
        Large,
    };

    private enum Explosion
    {
        None,
        ExplosionG_1,
        ExplosionG_2,
        ExplosionG_3,
        Explosion_1,
        Explosion_2,
        Explosion_3,
        ExplosionSimple_1,
        ExplosionSimple_2,
        ExplosionStar,
        ExplosionMine,
    };

    [Space(10)]
    public EnemyUnit m_ParentEnemy;
    public int m_Health;
    public EnemyClass m_Class;
    public uint m_Score;
    [Space(10)]
    [Tooltip("사망 상태 돌입시 폭발 이펙트")]
    [SerializeField] private Debris m_Debris = 0;
    [SerializeField] private Explosion m_DefaultExplosion = 0;
    [SerializeField] protected AudioClip m_DefaultAudioClip = null;
    [SerializeField] private Explosion[] m_Explosion = new Explosion[0];
    [SerializeField] protected AudioClip[] m_AudioClip = new AudioClip[0];
    [Space(10)]
    [SerializeField] private GameObject m_ItemBox = null;
    [SerializeField] protected byte m_GemNumber = 0;
    [Space(10)]
    [Tooltip("자아(콜라이더)를 가진 자식들 (체크시 독립적인 붉은색 blend, 데미지 blend를 가짐)")]
    public bool m_ShareHealth;
    public EnemyUnit[] m_ChildEnemies;
    public Collider2D[] m_Collider2D; // 지상 적 콜라이더 보정 및 충돌 체크
    public Queue<TweenData> m_TweenDataQueue = new Queue<TweenData>();

    protected Material[] m_Materials;
    protected Material[] m_MaterialsAll;
    protected Color[] m_DefaultAlbedo;
    protected bool m_UpdateTransform = true;
    
    [HideInInspector] public float m_CurrentAngle; // 현재 회전 각도
    [Tooltip("-1일 경우 무적이며 데미지를 parent 오브젝트에게 전달")]
    [HideInInspector] public int m_MaxHealth;
    [HideInInspector] public bool m_IsDead = false;

    private int m_TakingDamageTimer;
    private int m_LowHealthBlinkTimer;
    private bool[] m_TakeDamageType = { false, false, false };
    private Color m_DamagingAlbedo = new Color(0.64f, 0.64f, 1f, 1f); // blue

    private readonly Vector3 m_AirEnemyAxis = new Vector3(0f, -0.4f, 1f);
    private Quaternion m_DefaultQuaternion;
    private Vector3 m_DefaultAxis;
    private List<EnemyUnit> m_ParentList = new List<EnemyUnit>();

    protected override void Awake()
    {
        base.Awake();
        m_DefaultAxis = - transform.transform.up;
        m_DefaultQuaternion = transform.localRotation;
        m_MoveVector.direction = - transform.rotation.eulerAngles.y;

        m_MaxHealth = m_Health;
        m_IsUnattackable = false;

        InitMaterials();

        EnemyUnit current_unit = m_ParentEnemy;
        
        while (current_unit != null) {
            m_ParentList.Add(current_unit);
            current_unit = current_unit.m_ParentEnemy;
        }
        
        GetCoordinates();
        StartCoroutine(PlayTweenData());
    }

    private void InitMaterials() {
        m_MaterialsAll = GetAllMetrials();
        m_Materials = GetMaterials();
    }

    private Material[] GetAllMetrials() { // 무적 해제, 사망 이펙트 용 (전체 Materials)
        if (m_Collider2D.Length == 0)
            return new Material[0];

        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        Material[] mat = new Material[meshRenderers.Length];
            m_DefaultAlbedo = new Color[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++) {
            mat[i] = meshRenderers[i].material;
            m_DefaultAlbedo[i] = meshRenderers[i].material.color;
        }
        return mat;
    }

    private Material[] GetMaterials() {
        if (m_Collider2D.Length == 0) {
            if (m_ChildEnemies.Length == 0)
                return new Material[0];
        }
        if (m_MaxHealth < 0)
            return new Material[0];

        for (int i = 0; i < m_ChildEnemies.Length; i++) {
            m_ChildEnemies[i].gameObject.SetActive(false);
        }
        
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        Material[] mat = new Material[meshRenderers.Length];
        m_DefaultAlbedo = new Color[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++) {
            mat[i] = meshRenderers[i].material;
            m_DefaultAlbedo[i] = meshRenderers[i].material.color;
        }

        for (int i = 0; i < m_ChildEnemies.Length; i++) {
            m_ChildEnemies[i].gameObject.SetActive(true);
        }
        return mat;
    }


    protected virtual void Update()
    {
        for (int i = 0; i < m_TakeDamageType.Length; i++)
            m_TakeDamageType[i] = false;

        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        GetCoordinates();

        if (m_Health > m_MaxHealth) {
            m_Health = m_MaxHealth;
        }

        UpdateColorTimer();
    }

    protected void UpdateTransform()
    {
        if (m_CurrentAngle > 360f) {
            m_CurrentAngle -= 360f;
        }
        else if (m_CurrentAngle < 0f) {
            m_CurrentAngle += 360f;
        }
        
        Quaternion target_rotation;
        
        if (m_UpdateTransform) {
            float real_current_angle;
            Vector3 real_air_axis;

            if (m_ParentEnemy == null) { // 본체일 경우
                real_current_angle = m_CurrentAngle;
                real_air_axis = m_AirEnemyAxis;
            }
            else {
                real_current_angle = m_CurrentAngle;
                for (int i = 0; i < m_ParentList.Count; i++) {
                    real_current_angle -= m_ParentList[i].m_CurrentAngle;
                }
                //real_current_angle = m_CurrentAngle - m_ParentEnemy.m_CurrentAngle;
                real_air_axis = m_DefaultAxis;
            }

            if ((1 << gameObject.layer & Layer.AIR) != 0) { // 공중
                target_rotation = Quaternion.AngleAxis(real_current_angle, real_air_axis); // Vector3.forward
            }
            else { // 지상
                target_rotation = Quaternion.AngleAxis(real_current_angle, -transform.up); // Vector3.down
            }

            if (m_ParentEnemy == null) { // 본체일 경우
                transform.rotation = target_rotation;
            }
            else {
                transform.rotation = target_rotation * m_ParentEnemy.transform.rotation * transform.parent.localRotation * m_DefaultQuaternion;
            }
        }
    }
    

    
    protected void RotateSlightly(Vector2 target, float speed, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
        UpdateTransform();
    }

    protected void RotateSlightly(float target_angle, float speed, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
        UpdateTransform();
    }

    protected void RotateImmediately(Vector2 target, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = target_angle + rot;
        UpdateTransform();
    }

    protected void RotateImmediately(float target_angle, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        m_CurrentAngle = target_angle + rot;
        UpdateTransform();
    }

    private bool CheckDeadState() { // true이면 죽은 상태
        if (m_ParentEnemy == null) {
            if (m_IsDead) {
                return true;
            }
        }
        else {
            if (m_ParentEnemy.m_IsDead) {
                return true;
            }
        }
        return false;
    }


    public void EnableAttackable() {
        if (!m_IsUnattackable)
            return;
        if (m_Collider2D.Length == 0)
            return;
        m_IsUnattackable = false;
        for (int i = 0; i < m_Collider2D.Length; i++)
            m_Collider2D[i].enabled = true;
        StartCoroutine(AttackableTimer());
    }

    public void EnableInvincible(int millisecond = -1) { // millisecond간 무적. 0이면 미적용. -1이면 무기한 무적
        if (millisecond == 0)
            return;
        if (m_Collider2D.Length == 0)
            return;
        m_IsUnattackable = true;

        if (millisecond != -1)
            StartCoroutine(DisableInvincible(millisecond));
    }
    
    private IEnumerator DisableInvincible(int millisecond) {
        yield return new WaitForMillisecondFrames(millisecond);
        m_IsUnattackable = false;
        yield break;
    }

    public void DisableAttackable(int millisecond = -1) { // millisecond간 공격 불가. 0이면 미적용. -1이면 무기한 공격 불가
        if (m_Collider2D.Length == 0)
            return;
        if (millisecond == 0)
            return;
        m_IsUnattackable = true;
        for (int i = 0; i < m_Collider2D.Length; i++)
            m_Collider2D[i].enabled = false;
        
        if (millisecond != -1)
            StartCoroutine(AttackableTimer(millisecond));
    }

    private IEnumerator AttackableTimer(int millisecond = -1) {
        if (millisecond != -1) {
            yield return new WaitForMillisecondFrames(millisecond);
        }
        EnableAttackable();
        if (m_Class != EnemyClass.Zako || (1 << gameObject.layer & Layer.AIR) != 0) { // 지상 자코가 아닐 경우
            StartCoroutine(SetAttackableEffect());
        }
        yield break;
    }

    private IEnumerator SetAttackableEffect() { // 무적 해제 이펙트
        float color = 0.82f; // white
        while (color > 0f) {
            color -= 0.04f*Time.deltaTime*60f;
            for (int i = 0; i < m_MaterialsAll.Length; i++) {
                m_MaterialsAll[i].SetColor("_EmissionColor", new Color(color, color, color, 1f));
                m_MaterialsAll[i].EnableKeyword("_EMISSION");
            }
            yield return new WaitForFrames(0);
        }
        yield break;
    }

    protected override bool BulletCondition(Vector3 pos) {
        if (m_Class != EnemyClass.Boss) {
            if (m_ParentEnemy == null) {
                if (m_IsUnattackable) {
                    return false;
                }
            }
            else if (m_ParentEnemy.m_IsUnattackable) {
                return false;
            }
        }
        if (!base.BulletCondition(pos))
            return false;
        else
            return true;
    }


    protected override void MoveDirection(float speed, float direction) { // speed 속도로 direction 방향으로 이동. 0도는 아래, 90도는 오른쪽
        if ((1 << gameObject.layer & Layer.AIR) != 0) { // Air
            Vector2 vector2 = Quaternion.AngleAxis(direction, Vector3.forward) * Vector2.down;
            transform.Translate(vector2 * speed / Application.targetFrameRate * Time.timeScale, Space.World);
        }
        else { // Ground
            Vector3 vector3 = Quaternion.AngleAxis(direction, Vector3.down) * Vector3.back;
            transform.Translate(vector3 * speed / Application.targetFrameRate * Time.timeScale, Space.World);
        }

        UpdateTransform();
    }

    private IEnumerator PlayTweenData() { // 프레임 기반
        while (true) {
            if (m_TweenDataQueue.Count > 0) {
                TweenData td = m_TweenDataQueue.Dequeue();
                if (td is TweenDataMoveVector) { // duration동안 속도와 방향 변화
                    TweenDataMoveVector tdm = (TweenDataMoveVector) td;
                    int frame = tdm.duration * Application.targetFrameRate / 1000;
                    float init_speed = m_MoveVector.speed;
                    float init_direction = m_MoveVector.direction;
                    //Debug.Log($"{init_direction}, {tdm.direction}");

                    if (tdm.moveVector.speed == NO_CHANGE) {
                        tdm.moveVector.speed = init_speed;
                    }
                    if (tdm.moveVector.direction == NO_CHANGE) {
                        tdm.moveVector.direction = init_direction;
                    }

                    if (frame == 0) { // 즉시 적용
                        m_MoveVector = new MoveVector(tdm.moveVector.speed, tdm.moveVector.direction);
                    }
                    else {
                        for (int i = 0; i < frame; ++i) {
                            //m_MoveVector.speed = init_speed + (tdm.speed - init_speed)*(i+1) / frame;
                            float t = AC_Ease.ac_ease[tdm.easeType].Evaluate((float) (i+1)/frame);

                            m_MoveVector.speed = Mathf.Lerp(init_speed, tdm.moveVector.speed, (float) (i+1)/frame);
                            m_MoveVector.direction = Mathf.LerpAngle(init_direction, tdm.moveVector.direction, (float) (i+1)/frame);
                            //m_MoveVector.direction = init_direction + (tdm.direction - init_direction)*(i+1) / frame;
                            //Debug.Log(m_MoveVector.direction);
                            yield return new WaitForFrames(0);
                        }
                    }
                }
                else { // duration동안 대기
                    yield return new WaitForMillisecondFrames(td.duration);
                }
            }
            else { // Idle
                yield return new WaitForFrames(0);
            }
        }
    }

    public void GetCoordinates() { // m_PlayerPosition와 m_Position2D 변수의 좌표를 계산
        m_PlayerPosition = m_PlayerManager.m_Player.transform.position;
        if ((1 << gameObject.layer & Layer.AIR) != 0)
            m_Position2D = transform.position;
        else {
            m_Position2D = GetScreenPosition(transform.position);
            for (int i = 0; i < m_Collider2D.Length; i++) {
                m_Collider2D[i].transform.rotation = Quaternion.AngleAxis(m_CurrentAngle, Vector3.forward) * Quaternion.AngleAxis(Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
                m_Collider2D[i].transform.position = GetScreenPosition(transform.position);
            }
        }

    }

    public void TakeDamage(int amount, sbyte damage_type = -1, bool blend = true)
    {
        // damage_type - -1:일반공격, 0:레이저, 1:레이저(Aura), 2:폭탄
        // blend - ImageBlend 실행 여부
        if (!m_IsUnattackable) {
            if (m_ShareHealth) {
                if (m_MaxHealth == -1)
                    m_ParentEnemy.TakeDamage(amount, damage_type, true); // 최대체력이 -1일시 본체에게 데미지 그대로 전달 및 본체 색 blend
                else
                    m_ParentEnemy.TakeDamage(amount, damage_type, false); // 최대체력이 -1이 아닐시 본체에게 데미지 그대로 전달 및 자신 색 blend
            }
            if (damage_type >= 0) {
                if (m_TakeDamageType[damage_type])
                    return;
                else
                    m_TakeDamageType[damage_type] = true;
            }

            if (!m_IsDead) {
                if (blend) {
                    m_TakingDamageTimer = 67;
                }

                if (m_MaxHealth >= 0f) {
                    m_Health -= amount;

                    if (m_Health <= 0) {
                        m_IsDead = true;
                        KilledByPlayer();
                        OnDeath();
                    }
                }
            }
        }
    }

    public void OnDeath() {
        m_IsDead = true;
        m_SystemManager.AddScore(m_Score);
        
        DefatulExplosionEffect();
        CreateDebris();
        
        if (m_ParentEnemy == null) { // PlayState, 음악 정지, 무적 시간 등
            if (m_Class == EnemyClass.Boss) {
                m_SystemManager.BossClear();
            }
            else if (m_Class == EnemyClass.MiddleBoss) {
                m_SystemManager.MiddleBossClear();
            }
        }
        //DOTween.Kill(transform);
        ImageBlend(Color.red);
        StartCoroutine(AdditionalOnDeath());
        DisableAttackable();
    }

    protected virtual void KilledByPlayer() { // 플레이어가 죽인 경우
        return;
    }

    protected virtual IEnumerator AdditionalOnDeath() { // 추가 폭발 이펙트 (기본값은 없음)
        CreateItems();
        Destroy(gameObject);
        yield break;
    }

    void OnDestroy() { // 최종 파괴 (자연사도 작동)
        //DOTween.Kill(transform);
        if (m_SystemManager == null)
            return;
        else if (m_ParentEnemy != null)
            return;

        if (m_Class == EnemyClass.Boss) {
            m_SystemManager.StartCoroutine("StageClear");
        }
        else if (m_Class == EnemyClass.MiddleBoss) {
            m_SystemManager.MiddleBossClear();
        }
    }

    protected void CreateItems() {
        if (m_ItemBox != null) { // 아이템 드랍
            Vector3 item_pos;
            if ((1 << gameObject.layer & Layer.AIR) != 0) {
                item_pos = new Vector3(m_Position2D.x, m_Position2D.y, Depth.ITEMS);
            }
            else {
                item_pos = transform.position;
            }
            Instantiate(m_ItemBox, item_pos, Quaternion.identity);
        }
        if ((1 << gameObject.layer & Layer.AIR) != 0) {
            for (int i = 0; i < m_GemNumber; i++) {
                GameObject obj = m_PoolingManager.PopFromPool("ItemGemAir", PoolingParent.ITEM_GEM_AIR);
                Vector3 pos = transform.position + (Vector3) Random.insideUnitCircle * 0.8f;
                obj.transform.position = new Vector3(pos.x, pos.y, Depth.ITEMS);
                obj.SetActive(true);
            }
        }
        else {
            GameObject[] obj = new GameObject[m_GemNumber];
            for (int i = 0; i < m_GemNumber; i++) {
                obj[i] = m_PoolingManager.PopFromPool("ItemGemGround", 3);
            }
            CreateGems(obj);
            for (int i = 0; i < m_GemNumber; i++) {
                obj[i].SetActive(true);
            }
        }
    }


    private void UpdateColorTimer() {
        bool red_blink = false;
        if (m_IsDead)
            return;

        if (m_MaxHealth < 0f) {
            if (m_ParentEnemy.m_MaxHealth < 10000) { // 본체의 최대 체력이 10000 미만이면 체력 30% 이하시 붉은색 점멸
                if (m_ParentEnemy.m_Health < m_ParentEnemy.m_MaxHealth * 0.3f) {
                    red_blink = LowHealthImageBlend();
                }
            }
            else { // 본체의 최대 체력이 10000 이상이면 체력 3000 미만시 붉은색 점멸
                if (m_ParentEnemy.m_Health < 3000) {
                    red_blink = LowHealthImageBlend();
                }
            }
        }
        else if (m_MaxHealth < 10000) { // 최대 체력이 10000 미만이면 체력 30% 이하시 붉은색 점멸
            if (m_Health < m_MaxHealth * 3 / 10) {
                red_blink = LowHealthImageBlend();
            }
        }
        else { // 최대 체력이 10000 이상이면 체력 3000 미만시 붉은색 점멸
            if (m_Health < 3000) {
                red_blink = LowHealthImageBlend();
            }
        }

        if (red_blink)
            return;

        if (m_TakingDamageTimer > 0) {
            m_TakingDamageTimer -= (int) (1000 / Application.targetFrameRate * Time.timeScale);
            ImageBlend(m_DamagingAlbedo);
        }
        else {
            ImageBlend(m_DefaultAlbedo);
        }
    }

    private bool LowHealthImageBlend() { // true이면 빨간색
        if (m_LowHealthBlinkTimer > 0) {
            m_LowHealthBlinkTimer -= (int) (1000 / Application.targetFrameRate * Time.timeScale);
            if (m_LowHealthBlinkTimer > 383) {
                return true;
            }
        }
        else {
            m_LowHealthBlinkTimer = 500;
            ImageBlend(Color.red);
            return true;
        }
        return false;
    }

    protected void ImageBlend(Color target_color) {
        for (int i = 0; i < m_Materials.Length; i++) {
            if (m_Materials[i] != null)
                m_Materials[i].color = target_color;
        }
    }

    protected void ImageBlend(Color[] target_color) { // Overload
        for (int i = 0; i < m_Materials.Length; i++) {
            if (m_Materials[i] != null)
                m_Materials[i].color = target_color[i];
        }
    }


    private void CreateDebris() {
        if (m_Debris == Debris.None)
            return;
        GameObject obj = m_PoolingManager.PopFromPool("Debris", PoolingParent.DEBRIS);
        DebrisEffect debris = obj.GetComponent<DebrisEffect>();
        obj.transform.position = transform.position;
        debris.m_DebrisSize = (int) m_Debris;
        obj.SetActive(true);
    }

    private GameObject DefatulExplosionEffect() {
        GameObject obj = null;
        if (m_DefaultExplosion != 0) {
            obj = m_PoolingManager.PopFromPool(m_DefaultExplosion.ToString(), PoolingParent.EXPLOSION);

            Vector3 explosion_pos;

            if ((1 << gameObject.layer & Layer.AIR) != 0) {
                ExplosionEffect explosion_effect = obj.GetComponent<ExplosionEffect>();
                explosion_effect.m_MoveVector = m_MoveVector;
                explosion_pos = new Vector3(transform.position.x, transform.position.y, Depth.EXPLOSION);
            }
            else
                explosion_pos = transform.position;
            
            obj.transform.position = explosion_pos;

            obj.SetActive(true);
        }
        m_SystemManager.m_SoundManager.PlayAudio(m_DefaultAudioClip);
        return obj;
    }

    protected GameObject ExplosionEffect(int explosion, int audio, Vector3? pos = null, MoveVector? moveVector = null) {
        try {
            GameObject obj = m_PoolingManager.PopFromPool(m_Explosion[explosion].ToString(), PoolingParent.EXPLOSION);
            
            ExplosionEffect explosion_effect = obj.GetComponent<ExplosionEffect>();
            explosion_effect.m_MoveVector = moveVector ?? new MoveVector(0f, 0f);
            
            Vector3 explosion_pos = transform.TransformPoint(pos ?? Vector3.zero);
            
            if ((1 << gameObject.layer & Layer.AIR) != 0)
                explosion_pos = new Vector3(explosion_pos.x, explosion_pos.y, Depth.EXPLOSION);
            
            obj.transform.position = explosion_pos;
            
            obj.SetActive(true);
            if (audio < 0)
                return obj;
            else {
                m_SystemManager.m_SoundManager.PlayAudio(m_AudioClip[audio]);
                return obj;
            }
        }
        catch {
            Debug.LogAssertion("Explosion OutOfRangeException has occured.");
            return null;
        }
    }

    private void CreateGems(GameObject[] obj) {
        switch(m_GemNumber) {
            case 1:
                obj[0].transform.position = transform.position;
                break;
            case 2:
                obj[0].transform.position = transform.position + new Vector3(0f, 0f, 0.25f);
                obj[1].transform.position = transform.position + new Vector3(0f, 0f, -0.25f);
                break;
            case 3:
                obj[0].transform.position = transform.position + new Vector3(0f, 0f, 0.25f);
                obj[1].transform.position = transform.position + new Vector3(-0.29f, 0f, -0.25f);
                obj[2].transform.position = transform.position + new Vector3(0.29f, 0f, -0.25f);
                break;
            case 4:
                obj[0].transform.position = transform.position + new Vector3(-0.25f, 0f, 0.25f);
                obj[1].transform.position = transform.position + new Vector3(-0.25f, 0f, -0.25f);
                obj[2].transform.position = transform.position + new Vector3(0.25f, 0f, 0.25f);
                obj[3].transform.position = transform.position + new Vector3(0.25f, 0f, -0.25f);
                break;
            case 5:
                obj[0].transform.position = transform.position + new Vector3(-0.3f, 0f, 0.3f);
                obj[1].transform.position = transform.position + new Vector3(-0.3f, 0f, -0.3f);
                obj[2].transform.position = transform.position;
                obj[3].transform.position = transform.position + new Vector3(0.3f, 0f, 0.3f);
                obj[4].transform.position = transform.position + new Vector3(0.3f, 0f, -0.3f);
                break;
            default:
                for (int i = 0; i < m_GemNumber; i++) {
                    Vector3 vec = Random.insideUnitSphere * Mathf.Sqrt(m_GemNumber) * 0.5f;
                    obj[i].transform.position = transform.position + new Vector3(vec.x, 0f, vec.z);
                }
                break;
        }
    }
}

public class HasTargetPosition : EnemyUnit {
    public IEnumerator MoveTowardsToTarget(Vector2 target_vec2, int duration) {
        Vector3 init_position = transform.position;
        Vector3 target_position = new Vector3(target_vec2.x, target_vec2.y, Depth.ENEMY);
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_position, target_position, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }
}