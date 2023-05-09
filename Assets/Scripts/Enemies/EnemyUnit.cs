using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ============================================================================================ //

[RequireComponent(typeof(EnemyDeath))]
public abstract class EnemyUnit : EnemyObject, IRotatable // 적 개체, 포탑 (적 총알 제외)
{
    [HideInInspector] public EnemyHealth m_EnemyHealth; // Can bu null
    [HideInInspector] public EnemyDeath m_EnemyDeath;

    public EnemyType m_EnemyType;
    public int m_Score;
    [Space(10)]
    public Queue<TweenData> m_TweenDataQueue = new Queue<TweenData>();
    
    protected bool m_TimeLimitState = false;

    private readonly Vector3 m_AirEnemyAxis = new Vector3(0f, -0.4f, 1f);
    private Quaternion m_DefaultRotation;
    private bool m_Interactable = true;
    
    public event Action Action_StartInteractable;

    protected override void Awake()
    {
        base.Awake();
        m_DefaultRotation = transform.rotation;
        m_MoveVector.direction = - transform.rotation.eulerAngles.y;
        
        m_EnemyDeath = GetComponent<EnemyDeath>();
        
        m_EnemyDeath.Action_OnDying += HandleOnDying;
        m_EnemyDeath.Action_OnDying += DisableInteractable;
        if (m_EnemyType != EnemyType.Zako || (1 << gameObject.layer & Layer.AIR) != 0) { // 지상 자코가 아닐 경우
            if (TryGetComponent<EnemyColorBlender>(out EnemyColorBlender enemyColorBlender)) {
                Action_StartInteractable += enemyColorBlender.StartInteractableEffect;
            }
        }
        if (TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth)) {
            m_EnemyHealth = enemyHealth;
        }
        
        SetPosition2D();
        GetPlayerPosition2D();
    }

    protected virtual void Update()
    {
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        SetPosition2D();
        GetPlayerPosition2D();
    }

    private void LateUpdate()
    {
        UpdateTransform();
        SetColliderPosition();
    }

    public void SetPosition2D() { // m_Position2D 변수의 좌표를 계산
        m_PlayerPosition = m_PlayerManager.GetPlayerPosition();
        if ((1 << gameObject.layer & Layer.AIR) != 0)
            m_Position2D = transform.position;
        else {
            m_Position2D = GetScreenPosition(transform.position);
        }
    }

    private void SetColliderPosition() {
        Quaternion screenRotation = Quaternion.AngleAxis(m_CurrentAngle, Vector3.forward) * Quaternion.AngleAxis(Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
        m_EnemyHealth?.SetColliderPositionOnScreen(m_Position2D, screenRotation);
    }

    
    

    


    public void DisableInteractable() {
        DisableInteractable(-1);
    }

    public void DisableInteractable(int millisecond) { // millisecond간 공격 불가. 0이면 미적용. -1이면 무기한 공격 불가
        if (millisecond == 0)
            return;
        m_Interactable = false;
        m_EnemyHealth?.SetActiveColliders(false);
        
        if (millisecond != -1)
            StartCoroutine(InteractableTimer(millisecond));
    }

    public void DisableInteractableAll(int millisecond = -1) { // millisecond간 공격 불가. 0이면 미적용. -1이면 무기한 공격 불가
        if (millisecond == 0)
            return;
        
        EnemyUnit[] enemyUnits = GetComponentsInChildren<EnemyUnit>();
        for (int i = 0; i < enemyUnits.Length; ++i) {
            enemyUnits[i].DisableInteractable(millisecond);
        }
    }

    public void EnableInteractable() {
        if (m_Interactable)
            return;
        m_Interactable = true;
        m_EnemyHealth?.SetActiveColliders(true);
        StartCoroutine(InteractableTimer());
        Action_StartInteractable?.Invoke();
    }

    public void EnableInteractableAll() {
        EnemyUnit[] enemyUnits = GetComponentsInChildren<EnemyUnit>();
        for (int i = 0; i < enemyUnits.Length; ++i) {
            enemyUnits[i].EnableInteractable();
        }
    }

    public bool IsInteractable() {
        return m_Interactable;
    }

    private IEnumerator InteractableTimer(int millisecond = -1) {
        if (millisecond != -1) {
            yield return new WaitForMillisecondFrames(millisecond);
        }
        EnableInteractable();
        yield break;
    }

    internal void HandleOnDying() {
        m_EnemyDeath.m_IsDead = true;
        m_SystemManager.AddScore(m_Score);
        
        // ------------------- TODO : 보스쪽에 델리게이트로 구현 필요
        //DefatulExplosionEffect();
        // ---------------------
        //DOTween.Kill(transform);
        //ImageBlend(Color.red);
        StartCoroutine(DyingEffect());
    }

    protected virtual IEnumerator DyingEffect() {
        m_EnemyDeath.OnDeath();
        yield break;
    }
    // 폭발 이펙트 등 구현
    // 추가 구현시 OnDeath(); 반드시 포함 (인터페이스 사용?)
    // CreateItems, CreateDebris 는 Action_OnDeath에 등록



    protected override bool BulletCondition(Vector3 pos) {
        /*
        if (m_EnemyType != EnemyType.Boss) {
            if (m_ParentEnemy == null) {
                if (!IsInteractable()) {
                    return false;
                }
            }
            else if (!m_ParentEnemy.IsInteractable()) {
                return false;
            }
        }*/
        if (!IsInteractable()) {
            return false;
        }
        return base.BulletCondition(pos);
    }

    public void StartPlayTweenData() {
        StartCoroutine(PlayTweenData());
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

    /*
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
    }*/

    public void OutOfBound() { // 경계 바깥 파괴
        if (m_EnemyType == EnemyType.Boss) {
            return;
        }

        m_EnemyDeath.OnRemoved();
    }


    // IRotatable Methods ----------------------------------

    public void RotateSlightly(Vector2 target, float speed, float rot = 0f) {
        if (m_EnemyDeath.m_IsDead) {
            return;
        }
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
    }

    public void RotateSlightly(float target_angle, float speed, float rot = 0f) {
        if (m_EnemyDeath.m_IsDead) {
            return;
        }
        m_CurrentAngle = Mathf.MoveTowardsAngle(m_CurrentAngle, target_angle + rot, speed / Application.targetFrameRate * Time.timeScale);
    }

    public void RotateImmediately(Vector2 target, float rot = 0f) {
        if (m_EnemyDeath.m_IsDead) {
            return;
        }
        float target_angle = GetAngleToTarget(m_Position2D, target);
        m_CurrentAngle = target_angle + rot;
    }

    public void RotateImmediately(float target_angle, float rot = 0f) {
        if (m_EnemyDeath.m_IsDead) {
            return;
        }
        m_CurrentAngle = target_angle + rot;
    }

    public void UpdateTransform()
    {
        if (m_CurrentAngle > 360f) {
            m_CurrentAngle -= 360f;
        }
        else if (m_CurrentAngle < 0f) {
            m_CurrentAngle += 360f;
        }
        
        Quaternion target_rotation = Quaternion.identity;
        
        float modelRotationAngle = m_CurrentAngle;

        if ((1 << gameObject.layer & Layer.AIR) != 0) { // 공중
            target_rotation = Quaternion.AngleAxis(modelRotationAngle, (transform == this.transform.root) ? m_AirEnemyAxis : Vector3.down);
        }
        else { // 지상
            target_rotation = Quaternion.AngleAxis(modelRotationAngle, Vector3.down);
        }
        transform.rotation = m_DefaultRotation * target_rotation;
        return;
    }
}

interface ITargetPosition {
    public void MoveTowardsToTarget(Vector2 target_vec2, int duration);
}

interface IHasAppearance {
    public IEnumerator AppearanceSequence();
    public void OnAppearanceComplete();
}

interface IEnemyBossMain {
    public void OnBossDying();
    public void OnBossDeath();
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