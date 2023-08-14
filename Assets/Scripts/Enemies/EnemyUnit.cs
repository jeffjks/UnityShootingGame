using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ============================================================================================ //

[RequireComponent(typeof(EnemyDeath))]
public abstract class EnemyUnit : EnemyObject // 적 개체, 포탑 (적 총알 제외)
{
    [HideInInspector] public EnemyHealth m_EnemyHealth; // Can be null
    [HideInInspector] public EnemyDeath m_EnemyDeath;

    public override float CurrentAngle
    {
        get => _currentAngle;
        set
        {
            _currentAngle = value;
            _currentAngle = Mathf.Repeat(_currentAngle, 360f);
            OnCurrentAngleChanged();
        }
    }

    public EnemyType m_EnemyType;
    public int m_Score;
    public bool m_IsRoot;
    public Queue<TweenData> m_TweenDataQueue = new ();
    public bool IsExecutingPattern => _currentPatterns.Count > 0;
    protected event UnityAction Action_OnPatternStopped;

    private readonly Vector3 _airEnemyAxis = new (0f, -0.4f, 1f);
    private Quaternion _defaultRotation;
    private readonly Dictionary<string, Coroutine> _currentPatterns = new();

    public event Action Action_StartInteractable;

    protected virtual void Awake()
    {
        if (m_IsRoot)
        {
            if (transform.parent)
            {
                transform.SetParent(null);
            }
        }
        else
        {
            var rootEnemyUnit = transform.root.GetComponent<EnemyUnit>();
            m_IsAir = rootEnemyUnit.m_IsAir;
        }
        
        Transform root = transform.root;
        if (transform == root)
        {
            _defaultRotation = Quaternion.identity;
        }
        else
        {
            _defaultRotation = transform.rotation * Quaternion.Inverse(root.rotation);
        }
        
        m_MoveVector.direction = - _defaultRotation.eulerAngles.y;
        
        m_EnemyDeath = GetComponent<EnemyDeath>();
        
        m_EnemyDeath.Action_OnKilled += HandleOnKilled;
        m_EnemyDeath.Action_OnKilled += DisableInteractable;
        m_EnemyDeath.Action_OnKilled += StopAllPatterns;
        if (m_EnemyType != EnemyType.Zako || m_IsAir) { // 지상 자코가 아닐 경우
            if (TryGetComponent(out EnemyColorBlender enemyColorBlender)) {
                Action_StartInteractable += enemyColorBlender.StartInteractableEffect;
            }
        }
        if (TryGetComponent(out EnemyHealth enemyHealth)) {
            m_EnemyHealth = enemyHealth;
        }
    }

    protected virtual void Update()
    {
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        m_RotatePattern?.ExecuteRotatePattern(this);
    }

    private void LateUpdate()
    {
        SetColliderPosition();
    }

    private void SetColliderPosition() {
        if (m_IsAir)
        {
            return;
        }
        Quaternion screenRotation = Quaternion.AngleAxis(CurrentAngle, Vector3.forward) * Quaternion.AngleAxis(Size.BACKGROUND_CAMERA_ANGLE, Vector3.right);
        
        if (m_EnemyHealth != null)
            m_EnemyHealth.SetColliderPositionOnScreen(m_Position2D, screenRotation);
    }

    public Coroutine StartPattern(string key, IBulletPattern bulletPattern)
    {
        if (!m_IsInteractable)
            return null;

        if (bulletPattern == null)
            return null;
        
        _currentPatterns.Add(key, null);
        var enumerator = bulletPattern.ExecutePattern(() => StopPattern(key));
        var coroutine = StartCoroutine(enumerator);
        
        if (_currentPatterns.ContainsKey(key))
            _currentPatterns[key] = coroutine;
        
        return coroutine;
    }

    public void StopPattern(string key)
    {
        if (_currentPatterns.TryGetValue(key, out var value))
        {
            if (value != null)
                StopCoroutine(value);
            _currentPatterns.Remove(key);
        }
        
        Action_OnPatternStopped?.Invoke();
    }

    public void StopAllPatterns() {
        foreach (var pattern in _currentPatterns)
        {
            if (pattern.Value != null)
                StopCoroutine(pattern.Value);
        }
        _currentPatterns.Clear();
        
        Action_OnPatternStopped?.Invoke();
    }
    
    

    


    private void DisableInteractable() {
        DisableInteractable(-1);
    }

    public void DisableInteractable(int millisecond) { // millisecond간 공격 불가. 0이면 미적용. -1이면 무기한 공격 불가
        if (millisecond == 0)
            return;
        m_IsInteractable = false;
        if (m_EnemyHealth != null)
            m_EnemyHealth.SetActiveColliders(false);
        
        if (millisecond != -1)
            StartCoroutine(InteractableTimer(millisecond));
    }

    protected void DisableInteractableAll(int millisecond = -1) { // millisecond간 공격 불가. 0이면 미적용. -1이면 무기한 공격 불가
        if (millisecond == 0)
            return;
        
        EnemyUnit[] enemyUnits = GetComponentsInChildren<EnemyUnit>();
        for (int i = 0; i < enemyUnits.Length; ++i) {
            enemyUnits[i].DisableInteractable(millisecond);
        }
    }

    public void EnableInteractable() {
        if (m_IsInteractable)
            return;
        m_IsInteractable = true;
        if (m_EnemyHealth != null)
            m_EnemyHealth.SetActiveColliders(true);
        StartCoroutine(InteractableTimer());
        Action_StartInteractable?.Invoke();
    }

    public void EnableInteractableAll() {
        EnemyUnit[] enemyUnits = GetComponentsInChildren<EnemyUnit>();
        for (int i = 0; i < enemyUnits.Length; ++i) {
            enemyUnits[i].EnableInteractable();
        }
    }

    private IEnumerator InteractableTimer(int millisecond = -1) {
        if (millisecond != -1) {
            yield return new WaitForMillisecondFrames(millisecond);
        }
        EnableInteractable();
    }

    private void HandleOnKilled() {
        m_EnemyDeath.IsDead = true;
        InGameDataManager.Instance.AddScore(m_Score);
        StartCoroutine(DyingEffect());
    }

    protected virtual IEnumerator DyingEffect() {
        m_EnemyDeath.OnEndDeathAnimation();
        yield break;
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
                float t_lerp = AC_Ease.ac_ease[(int)tdmp.easeType].Evaluate((float) (i+1)/frame);

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
            float t_lerp = AC_Ease.ac_ease[(int)tdmp.easeType].Evaluate((float) (i+1)/frame);
            
            transform.position = Vector3.Lerp(init_position, target_position, t_lerp);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    public void OutOfBound() { // 경계 바깥 파괴
        if (m_EnemyType == EnemyType.Boss) {
            return;
        }

        m_EnemyDeath.RemoveEnemy();
    }


    private void OnCurrentAngleChanged()
    {
        Quaternion target_rotation;

        if (m_IsAir) {
            target_rotation = Quaternion.AngleAxis(CurrentAngle, (transform == transform.root) ? _airEnemyAxis : Vector3.down);
        }
        else {
            target_rotation = Quaternion.AngleAxis(CurrentAngle, Vector3.down);
        }
        transform.rotation = _defaultRotation * target_rotation;
    }
}

interface ITargetPosition {
    public void MoveTowardsToTarget(Vector2 target_vec2, int duration);
}

interface IEnemyBossMain {
    public void OnBossKilled();
    public void OnEndBossDeathAnimation();
}