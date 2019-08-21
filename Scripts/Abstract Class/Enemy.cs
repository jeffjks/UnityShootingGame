using System.Collections;
using UnityEngine;
using DG.Tweening;

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

namespace UnityEngine
{
    public struct MathfCustom2
    {
        public static float SmoothEnd(float t) // 점점 완만해짐 (이차함수 상하반전) (t = 0~1)
        {
        double from = 0;
        double to = 1;
        t = Mathf.Clamp01(t);
        t = (float) (- (double) t * (double) t + 2.0 * (double) t);
        return (float) ((double) to * (double) t + (double) from * (1.0 - (double) t));
        }
    }
}

public enum EnemyClass
{
    Zako,
    MiddleBoss,
    Boss,
}

// ================ 적 ================ //

public abstract class Enemy : MonoBehaviour { // 총알

    public MoveVector m_MoveVector;

    protected SystemManager m_SystemManager = null;
    protected PlayerManager m_PlayerManager = null;
    protected PoolingManager m_PoolingManager = null;
    protected Vector2 m_Position2D, m_PlayerPosition, m_BackgroundCameraSize;

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
    
    void Update()
    {
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        m_PlayerPosition = m_PlayerManager.m_Player.transform.position;
    }

    protected Vector2 GetScreenPosition(Vector3 pos) {
        float main_camera_xpos = m_PlayerManager.m_MainCamera.transform.position.x;
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
        transform.Translate(vector2 * speed * Time.deltaTime, Space.World);
    }


    // Type 0 총알
    protected GameObject[] CreateBulletsSector(byte image, Vector3 pos, float speed, float direction, EnemyBulletAccel accel, int num, float interval) {
        GameObject[] objs = new GameObject[num];
        for (int i=0; i<num; i++) {
            objs[i] = CreateBullet(image, pos, speed, direction - interval*(num - i*2 - 1)/2, accel);
        }
        return objs;
    }

    // Type (0), 1, 2 총알
    protected GameObject[] CreateBulletsSector(byte image, Vector3 pos, float speed, float direction, EnemyBulletAccel accel, int num, float interval,
    byte type, float timer, byte new_image, float new_speed, byte new_direction, float direction_add, EnemyBulletAccel new_accel,
    int new_num = 0, float new_interval = 0f, float secondTimer = 0) {
        GameObject[] objs = new GameObject[num];
        for (int i=0; i<num; i++) {
            objs[i] = CreateBullet(image, pos, speed, direction - interval*(num - i*2 - 1)/2, accel,
            type, timer, new_image, new_speed, new_direction, direction_add, new_accel, new_num, new_interval, secondTimer);
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

            obj.SetActive(true);
        }
        return obj;
    }

    // Type (0), 1, 2 총알
    protected GameObject CreateBullet(byte image, Vector3 pos, float speed, float direction, EnemyBulletAccel accel,
    byte type, float timer, byte new_image, float new_speed, byte new_direction, float direction_add, EnemyBulletAccel new_accel,
    int new_num = 0, float new_interval = 0f, float secondTimer = 0)
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

            enemyBullet.m_SecondTimer = secondTimer;
            enemyBullet.m_NewNumber = new_num;
            enemyBullet.m_NewInterval = new_interval;

            obj.SetActive(true);
        }
        return obj;
    }

    protected virtual bool BulletCondition(Vector3 pos) {
        float camera_x = m_PlayerManager.m_MainCamera.transform.position.x;

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
        else
            return true;
    }
}

// ============================================================================================ //

public abstract class EnemyUnit : Enemy // 적 개체, 포탑 (적 총알 제외)
{
    public enum Debris
    {
        None,
        Small,
        Large,
    };

    public enum Explosion
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

    public float m_Health;
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
    [SerializeField] private byte m_GemNumber = 0;
    [Space(10)]
    public EnemyUnit m_ParentEnemy = null;
    public Collider2D m_Collider2D = null; // 지상 적 콜라이더 보정 및 충돌 체크

    protected Material[] m_Materials;
    protected Sequence m_Sequence = null;
    protected float m_CurrentAngle = 0f; // 현재 회전 각도
    protected bool m_IsAttackable = true;
    protected bool m_UpdateTransform = true;
    
    [HideInInspector] public float m_MaxHealth;
    [HideInInspector] public bool m_IsDead = false;

    private float m_TakingDamageTimer = 0f;
    protected Color[] m_DefaultAlbedo;
    private Color m_DamagingAlbedo = new Color(0.64f, 0.64f, 1f, 1f); // blue

    private readonly Vector3 m_AirEnemyAxis = new Vector3(0f, -0.4f, 1f);
    private Quaternion m_DefaultQuaternion;
    private Vector3 m_DefaultAxis;

    protected override void Awake()
    {
        base.Awake();
        m_DefaultAxis = -transform.transform.up;
        m_DefaultQuaternion = transform.localRotation;

        try {
            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            m_Materials = new Material[meshRenderers.Length];
            m_DefaultAlbedo = new Color[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++) {
                m_Materials[i] = meshRenderers[i].material;
                m_DefaultAlbedo[i] = meshRenderers[i].material.color;
            }
        }
        catch (System.NullReferenceException) {
            m_Materials = new Material[0];
        }
        m_MaxHealth = m_Health;
    }

    protected virtual void Update()
    {
        if (m_Health > m_MaxHealth) {
            m_Health = m_MaxHealth;
        }

        if (m_TakingDamageTimer > 0f) {
            m_TakingDamageTimer -= 60 * Time.deltaTime;
        }
        else if (!m_IsDead) {
            ImageBlend(m_DefaultAlbedo);
        }
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        GetCoordinates();
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
            if ((1 << gameObject.layer & Layer.AIR) != 0) { // 공중
                if (m_ParentEnemy == null)
                    target_rotation = Quaternion.AngleAxis(m_CurrentAngle, m_AirEnemyAxis); // Vector3.forward
                else
                    target_rotation = Quaternion.AngleAxis(m_CurrentAngle, m_DefaultAxis); // Vector3.forward
            }
            else { // 지상
                target_rotation = Quaternion.AngleAxis(m_CurrentAngle, -transform.up); // Vector3.down
            }

            if (transform.parent == null) {
                transform.rotation = target_rotation;
            }
            else {
                transform.rotation = target_rotation * transform.parent.localRotation * m_DefaultQuaternion;
            }
        }
    }
    

    
    protected void RotateSlightly(Vector2 target, float speed, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed * Time.deltaTime);
    }

    protected void RotateSlightly(float target_angle, float speed, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed * Time.deltaTime);
    }

    protected void RotateImmediately(Vector2 target, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = target_angle + rot;
    }

    protected void RotateImmediately(float target_angle, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        m_CurrentAngle = target_angle + rot;
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
        if (m_Collider2D == null)
            return;
        m_IsAttackable = true;
        m_Collider2D.enabled = true;
    }

    public void DisableAttackable(float timer = 0f) { // timer초간 공격 불가. 0이면 미적용. -1이면 무기한 공격 불가
        if (m_Collider2D == null)
            return;
        if (timer == 0)
            return;
        m_IsAttackable = false;
        m_Collider2D.enabled = false;
        
        if (timer != -1)
            Invoke("AttackableTimer", timer);
    }

    public void AttackableTimer() {
        StartCoroutine(SetAttackable());
    }

    private IEnumerator SetAttackable() { // 무적 해제 이펙트
        EnableAttackable();
        byte color = 210; // white
        while(color > 0f) {
            color -= 10;
            for (int i = 0; i < m_Materials.Length; i++) {
                m_Materials[i].SetColor("_EmissionColor", new Color32(color, color, color, 255));
                m_Materials[i].EnableKeyword("_EMISSION");
            }
            yield return null;
        }
        yield break;
    }

    protected override bool BulletCondition(Vector3 pos) {
        if (m_ParentEnemy == null) {
            if (!m_IsAttackable) {
                return false;
            }
        }
        else {
            if (!m_ParentEnemy.m_IsAttackable) {
                return false;
            }
        }
        if (!base.BulletCondition(pos))
            return false;
        else
            return true;
    }


    protected override void MoveDirection(float speed, float direction) { // speed 속도로 direction 방향으로 이동.
        if ((1 << gameObject.layer & Layer.AIR) != 0) {
            Vector2 vector2 = Quaternion.AngleAxis(direction, Vector3.forward) * Vector2.down;
            transform.Translate(vector2 * speed * Time.deltaTime, Space.World);
        }
        else {
            Vector3 vector3 = Quaternion.AngleAxis(direction, Vector3.down) * Vector3.back;
            transform.Translate(vector3 * speed * Time.deltaTime, Space.World);
        }
        UpdateTransform();
    }

    public void GetCoordinates() { // m_PlayerPosition와 m_Position2D 변수의 좌표를 계산
        m_PlayerPosition = m_PlayerManager.m_Player.transform.position;
        if ((1 << gameObject.layer & Layer.AIR) != 0)
            m_Position2D = transform.position;
        else {
            m_Position2D = GetScreenPosition(transform.position);
            if (m_Collider2D != null) {
                m_Collider2D.transform.rotation = Quaternion.AngleAxis(m_CurrentAngle, Vector3.forward) * Quaternion.AngleAxis(Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
                m_Collider2D.transform.position = m_Position2D;
            }
        }

    }

    public void TakeDamage(float amount)
    {
        if (m_IsAttackable) {
            m_Health -= amount;

            if (!m_IsDead) {
                ImageBlend(m_DamagingAlbedo);
                
                m_TakingDamageTimer = 4f;

                if (m_Health <= 0f) {
                    m_IsDead = true;
                    OnDeath();
                }
            }
        }
    }

    public void OnDeath() {
        m_IsDead = true;
        m_SystemManager.AddScore(m_Score);
        
        DefatulExplosionEffect();
        CreateDebris();
        
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
                GameObject obj = m_PoolingManager.PopFromPool("ItemGemAir", PoolingParent.ITEM_GEM);
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
                        obj[i].transform.position = transform.position + (Vector3) Random.insideUnitCircle;
                    }
                    break;
            }
            for (int i = 0; i < m_GemNumber; i++) {
                obj[i].SetActive(true);
            }
        }
        
        if (m_Class == EnemyClass.Boss) {
            m_SystemManager.BossClear();
        }
        else if (m_Class == EnemyClass.MiddleBoss) {
            m_SystemManager.MiddleBossClear();
        }
        DOTween.Kill(transform);
        DisableAttackable(-1f);
        ImageBlend(Color.red);
        if (m_Sequence != null) {
            m_Sequence.Kill();
        }
        StartCoroutine(AdditionalOnDeath());
    }

    protected virtual IEnumerator AdditionalOnDeath() { // 추가 폭발 이펙트 (기본값은 없음)
        Destroy(gameObject);
        yield break;
    }

    void OnDestroy() { // 최종 파괴 (자연사도 작동)
        DOTween.Kill(transform);
        if (m_Class == EnemyClass.Boss) {
            m_SystemManager.StartCoroutine("StageClear");
        }
        else if (m_Class == EnemyClass.MiddleBoss) {
            m_SystemManager.MiddleBossClear();
        }
    }


    protected void ImageBlend(Color target_color) {
        if (m_ParentEnemy != null)
            return;
        for (int i = 0; i < m_Materials.Length; i++) {
            m_Materials[i].color = target_color;
        }
    }

    protected void ImageBlend(Color[] target_color) { // Overload
        if (m_ParentEnemy != null)
            return;
        for (int i = 0; i < m_Materials.Length; i++) {
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
        if (m_DefaultExplosion == 0)
            return null;
        GameObject obj = m_PoolingManager.PopFromPool(m_DefaultExplosion.ToString(), PoolingParent.EXPLOSION);

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
    


    private void StopTimer() {
        StartCoroutine(Stop());
    }

    private IEnumerator Stop() {
        while (m_MoveVector.speed > 0) {
            m_MoveVector.speed -= 0.1f;
            yield return null;
        }
        m_MoveVector.speed = 0f;
        yield break;
    }
}