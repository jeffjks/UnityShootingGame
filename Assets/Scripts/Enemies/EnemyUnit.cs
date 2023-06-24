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

    protected virtual void Awake()
    {
        Transform root = transform.root;
        if (transform == root)
        {
            m_DefaultRotation = Quaternion.identity;
        }
        else
        {
            m_DefaultRotation = transform.rotation * Quaternion.Inverse(root.rotation);
        }
        
        m_MoveVector.direction = - m_DefaultRotation.eulerAngles.y;
        
        m_EnemyDeath = GetComponent<EnemyDeath>();
        
        m_EnemyDeath.Action_OnDying += HandleOnDying;
        m_EnemyDeath.Action_OnDying += DisableInteractable;
        if (m_EnemyType != EnemyType.Zako || Utility.CheckLayer(gameObject, Layer.AIR)) { // 지상 자코가 아닐 경우
            if (TryGetComponent<EnemyColorBlender>(out EnemyColorBlender enemyColorBlender)) {
                Action_StartInteractable += enemyColorBlender.StartInteractableEffect;
            }
        }
        if (TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth)) {
            m_EnemyHealth = enemyHealth;
        }
        
        SetPosition2D();
    }

    protected virtual void Update()
    {
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        SetPosition2D();
    }

    private void LateUpdate()
    {
        UpdateTransform();
        SetColliderPosition();
    }

    public void SetPosition2D() { // m_Position2D 변수의 좌표를 계산
        if (Utility.CheckLayer(gameObject, Layer.AIR))
            m_Position2D = transform.position;
        else {
            m_Position2D = BackgroundCamera.GetScreenPosition(transform.position);
        }
    }

    private void SetColliderPosition() {
        if (Utility.CheckLayer(gameObject, Layer.AIR))
        {
            return;
        }
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
    }

    private void HandleOnDying() {
        m_EnemyDeath.m_IsDead = true;
        InGameDataManager.Instance.AddScore(m_Score);
        StartCoroutine(DyingEffect());
    }

    protected virtual IEnumerator DyingEffect() {
        m_EnemyDeath.OnDeath();
        yield break;
    }



    protected override bool BulletCondition(Vector3 pos) {
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
                TweenData tdmp = m_TweenDataQueue.Dequeue(); // delay동안 대기 후 duration동안 속도와 방향 변화
                UnitMovement unitMovement = tdmp.unitMovement;
                
                yield return new WaitForMillisecondFrames(unitMovement.delay);
                
                int frame = unitMovement.duration * Application.targetFrameRate / 1000;

                if (unitMovement is MovePattern) {
                    yield return SetTweenMovement(frame, tdmp, (MovePattern) unitMovement);
                }
                else if (unitMovement is MoveTarget) {
                    yield return SetTweenMovement(frame, tdmp, (MoveTarget) unitMovement);
                }
                else {
                    Debug.LogError("Unknown TweenData type!");
                }

                /*
                float init_speed = m_MoveVector.speed;
                float init_direction = m_MoveVector.direction;

                if (movePattern.keepSpeed) {
                    movePattern.speed = init_speed;
                }
                if (movePattern.keepDirection) {
                    movePattern.direction = init_direction;
                }

                if (frame == 0) { // 즉시 적용
                    m_MoveVector = new MoveVector(movePattern.speed, movePattern.direction);
                }
                else {
                    for (int i = 0; i < frame; ++i) {
                        //m_MoveVector.speed = init_speed + (tdm.speed - init_speed)*(i+1) / frame;
                        float t = AC_Ease.ac_ease[tdmp.easeType].Evaluate((float) (i+1)/frame);

                        m_MoveVector.speed = Mathf.Lerp(init_speed, movePattern.speed, (float) (i+1)/frame);
                        m_MoveVector.direction = Mathf.LerpAngle(init_direction, movePattern.direction, (float) (i+1)/frame);
                        //m_MoveVector.direction = init_direction + (tdm.direction - init_direction)*(i+1) / frame;
                        //Debug.Log(m_MoveVector.direction);
                        yield return new WaitForFrames(0);
                    }
                }*/
            }
            else { // Idle
                yield return null;
            }
        }
    }

    private IEnumerator SetTweenMovement(int frame, TweenData tdmp, MovePattern movePattern) {
        float init_speed = m_MoveVector.speed;
        float init_direction = m_MoveVector.direction;

        if (movePattern.keepSpeed) {
            movePattern.speed = init_speed;
        }
        if (movePattern.keepDirection) {
            movePattern.direction = init_direction;
        }

        if (frame == 0) { // 즉시 적용
            m_MoveVector = new MoveVector(movePattern.speed, movePattern.direction);
        }
        else {
            for (int i = 0; i < frame; ++i) {
                float t_lerp = AC_Ease.ac_ease[tdmp.easeType].Evaluate((float) (i+1)/frame);

                m_MoveVector.speed = Mathf.Lerp(init_speed, movePattern.speed, t_lerp);
                m_MoveVector.direction = Mathf.LerpAngle(init_direction, movePattern.direction, t_lerp);
                yield return new WaitForFrames(0);
            }
        }
    }

    private IEnumerator SetTweenMovement(int frame, TweenData tdmp, MoveTarget moveTarget) {
        Vector3 init_position = transform.position;
        Vector3 target_position = new Vector3(moveTarget.targetVector2.x, moveTarget.targetVector2.y, Depth.ENEMY);

        for (int i = 0; i < frame; ++i) {
            float t_lerp = AC_Ease.ac_ease[tdmp.easeType].Evaluate((float) (i+1)/frame);
            
            transform.position = Vector3.Lerp(init_position, target_position, t_lerp);
            yield return new WaitForMillisecondFrames(0);
        }
    }

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
        
        Quaternion target_rotation;
        
        float modelRotationAngle = m_CurrentAngle;

        if (Utility.CheckLayer(gameObject, Layer.AIR)) {
            target_rotation = Quaternion.AngleAxis(modelRotationAngle, (transform == transform.root) ? m_AirEnemyAxis : Vector3.down);
        }
        else {
            target_rotation = Quaternion.AngleAxis(modelRotationAngle, Vector3.down);
        }
        transform.rotation = m_DefaultRotation * target_rotation;
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