using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface UseObjectPool {
    void ReturnToPool();
}

// ============================================================================================ //

public abstract class EnemyUnit : EnemyObject, IRotatable // 적 개체, 포탑 (적 총알 제외)
{
    [Space(10)]
    public EnemyUnit m_ParentEnemy;
    public EnemyHealth m_EnemyHealth;
    public EnemyType m_EnemyType;
    public int m_Score;
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
    public EnemyUnit[] m_ChildEnemies;
    public Collider2D[] m_Collider2D; // 지상 적 콜라이더 보정 및 충돌 체크
    public Queue<TweenData> m_TweenDataQueue = new Queue<TweenData>();
    
    protected bool m_UpdateTransform = true;
    protected bool m_TimeLimitState = false;

    private readonly Vector3 m_AirEnemyAxis = new Vector3(0f, -0.4f, 1f);
    private Quaternion m_DefaultQuaternion;
    private Vector3 m_DefaultAxis;
    private List<EnemyUnit> m_ParentList = new List<EnemyUnit>();
    private bool m_LowHealthState = false;

    protected override void Awake()
    {
        base.Awake();
        m_DefaultAxis = - transform.transform.up;
        m_DefaultQuaternion = transform.localRotation;
        m_MoveVector.direction = - transform.rotation.eulerAngles.y;

        if (m_EnemyHealth == null) {
            Debug.LogError("NullReferenceException: " + this);
        }

        //m_MaxHealth = m_Health;

        EnemyUnit current_unit = m_ParentEnemy;
        
        while (current_unit != null) {
            m_ParentList.Add(current_unit);
            current_unit = current_unit.m_ParentEnemy;
        }
        
        m_EnemyHealth.Action_OnDying += HandleOnDying;
        
        SetPosition2D();
        GetPlayerPosition2D();
        StartCoroutine(PlayTweenData());
    }

    protected virtual void Update()
    {
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        UpdateTransform();
        GetPlayerPosition2D();
        SetPosition2D();
    }

    private void SetPosition2D() { // m_Position2D 변수의 좌표를 계산
        m_PlayerPosition = m_PlayerManager.GetPlayerPosition();
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
    

    

    public bool CheckDeadState() { // true이면 죽은 상태
        if (m_ParentEnemy == null) {
            if (m_IsDead) {
                return true;
            }
        }
        else {
            if (m_ParentEnemy.m_IsDead) {
                return m_ParentEnemy.CheckDeadState();
            }
        }
        return false;
    }

    internal void HandleOnDying() {
        m_IsDead = true;
        m_SystemManager.AddScore(m_Score);
        
        // -------------------
        DefatulExplosionEffect();
        
        if (m_ParentEnemy == null) { // PlayState, 음악 정지, 무적 시간 등
            if (m_EnemyType == EnemyType.Boss) {
                m_SystemManager.BossClear();
            }
            else if (m_EnemyType == EnemyType.MiddleBoss) {
                m_SystemManager.MiddleBossClear();
            }
        }
        // ---------------------
        //DOTween.Kill(transform);
        //ImageBlend(Color.red);
        StartCoroutine(DyingEffect());
    }

    protected abstract IEnumerator DyingEffect(); // 폭발 이펙트 등 (기본값은 없음), m_EnemyHealth.OnDeath(); 반드시 포함
    // CreateItems
    // CreateDebris



    protected override bool BulletCondition(Vector3 pos) {
        if (m_EnemyType != EnemyType.Boss) {
            if (m_ParentEnemy == null) {
                if (!m_EnemyHealth.IsInteractable()) {
                    return false;
                }
            }
            else if (!m_ParentEnemy.m_EnemyHealth.IsInteractable()) {
                return false;
            }
        }
        if (!base.BulletCondition(pos))
            return false;
        else
            return true;
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

    public void TakeDamage(int amount, sbyte damage_type = -1, bool blend = true)
    {
        if (m_EnemyHealth.m_HealthType == HealthType.Share) { // 본체에게 데미지 및 본체 색 blend
            m_ParentEnemy?.TakeDamage(amount, damage_type, true);
        }
        else if (m_EnemyHealth.m_HealthType == HealthType.Independent) { // 자신에게 데미지 및 자신 색 blend
            m_EnemyHealth.TakeDamage(amount, damage_type, blend);
        }
        else { // 본체와 자신에게 데미지 및 자신 색 blend
            m_ParentEnemy?.TakeDamage(amount, damage_type, false);
            m_EnemyHealth.TakeDamage(amount, damage_type, blend);
        }
    }

    public void OutOfBound() { // 경계 바깥 파괴
        if (m_EnemyType != EnemyType.Boss) {
            Destroy(gameObject);
            if (m_EnemyType == EnemyType.MiddleBoss) {
                m_SystemManager.MiddleBossClear();
            }
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
                Vector3 pos = transform.position + (Vector3) UnityEngine.Random.insideUnitCircle * 0.8f;
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


    private void CreateDebris() {
        if (m_Debris == Debris.None)
            return;
        GameObject obj = m_PoolingManager.PopFromPool("Debris", PoolingParent.DEBRIS);
        DebrisEffect debris = obj.GetComponent<DebrisEffect>();
        obj.transform.position = transform.position;
        obj.SetActive(true);
        debris.OnStart((int) m_Debris);
    }

    private GameObject DefatulExplosionEffect() {
        GameObject obj = null;
        if (m_DefaultExplosion != 0) {
            obj = m_PoolingManager.PopFromPool(m_DefaultExplosion.ToString(), PoolingParent.EXPLOSION);
            ExplosionEffect explosion_effect = obj.GetComponent<ExplosionEffect>();

            Vector3 explosion_pos;

            if ((1 << gameObject.layer & Layer.AIR) != 0) {
                explosion_effect.m_MoveVector = m_MoveVector;
                explosion_pos = new Vector3(transform.position.x, transform.position.y, Depth.EXPLOSION);
            }
            else
                explosion_pos = transform.position;
            
            obj.transform.position = explosion_pos;

            obj.SetActive(true);
            explosion_effect.OnStart();
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
            explosion_effect.OnStart();
            
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
                    Vector3 vec = UnityEngine.Random.insideUnitSphere * Mathf.Sqrt(m_GemNumber) * 0.5f;
                    obj[i].transform.position = transform.position + new Vector3(vec.x, 0f, vec.z);
                }
                break;
        }
    }


    // IRotatable Methods ----------------------------------

    public void RotateSlightly(Vector2 target, float speed, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
        UpdateTransform();
    }

    public void RotateSlightly(float target_angle, float speed, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
        UpdateTransform();
    }

    public void RotateImmediately(Vector2 target, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = target_angle + rot;
        UpdateTransform();
    }

    public void RotateImmediately(float target_angle, float rot = 0f) {
        if (CheckDeadState()) {
            return;
        }
        m_CurrentAngle = target_angle + rot;
        UpdateTransform();
    }

    public void UpdateTransform()
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
}

interface ITargetPosition {
    public void MoveTowardsToTarget(Vector2 target_vec2, int duration);
}

interface IHasAppearance {
    public IEnumerator AppearanceSequence();
    public void OnAppearanceComplete();
}

/*
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

public class EnemyBoss : EnemyUnit {
    protected void BossDestroyed() {
        m_SystemManager.StartStageClearCoroutine();
        Destroy(gameObject);
    }
}*/