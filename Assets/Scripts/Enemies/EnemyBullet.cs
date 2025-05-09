﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class EnemyBullet : EnemyObject, IObjectPooling
{
    public string m_ObjectName;
    public BulletDatas m_BulletData;
    public TriggerBody m_TriggerBody;
    //public TriggerBody m_TriggerBodyCapsule;
    public Animator[] m_EraseAnimator;
    
    private int _imageDepth;
    private bool _isRotating;
    
    private GameObject _bulletImageObject;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private GameObject m_BulletExplosion;
    //private TriggerBody _currentTriggerBody;
    private Bullet _currentBullet;
    private BulletImage _bulletImage;
    private SubBulletPattern _subBulletPattern;
    private bool _isInList;
    private const float BoundaryPadding = 0.375f;
    private bool _isPlayingEraseAnimation { get; set; }

    public EnemyObject OwnerEnemyObject { private get; set; }

    public BulletImage BulletImage
    {
        get => _bulletImage;
        private set
        {
            _bulletImage = value;
            OnBulletImageChanged();
        }
    }

    /* m_Type ====================
    deltaValue가 0이면 속도 변화 X
    0 = 일반 총알
    1 = n초후 다른 총알 생성
    2 = n초후 파괴 후 다른 총알 생성

    생성 총알 : 0만 가능
    ImageType
    BulletAccel : targetSpeed = 0, deltaValue = 0
    MoveVector : speed = v, direction = 고정 방향 + @, 플레이어 방향 + @, 현재 방향 + @

    0 = image, pos, speed, direction, (accel)
    1, 2 = image, pos, speed, direction, (accel) / type, timer, new_image, new_speed, new_direction, direction_add , new_accel)

    second_timer : Vector(a, b) : a~b 사이 랜덤한 시간 간격으로 반복
    =========================== */

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _bulletImageObject = _spriteRenderer.gameObject;
        _animator = _bulletImageObject.GetComponent<Animator>();
        _subBulletPattern = new SubBulletPattern(this);
    }

    public void OnStart(BulletProperty bulletProperty)
    {
        SetSortingLayer();
        _isPlayingEraseAnimation = false;

        var pivotDirection = 0f;
        switch (bulletProperty.pivot)
        {
            case BulletPivot.Fixed:
                break;
            case BulletPivot.Player:
                pivotDirection = GetAngleToTarget(transform.position, PlayerManager.GetPlayerPosition());
                break;
            case BulletPivot.Current:
                pivotDirection = OwnerEnemyObject.CurrentAngle;
                break;
        }

        var bulletDirection = pivotDirection + bulletProperty.direction;
        m_MoveVector = new MoveVector(bulletProperty.speed, bulletDirection);
        BulletImage = bulletProperty.image;
        CurrentAngle = bulletDirection;

        if (BulletManager.InBulletFreeState)
        {
            PlayEraseAnimation();
            return;
        }
        
        AddToBulletList();

        if (bulletProperty.accel.duration > 0)
            StartCoroutine(ApplyBulletAccel(bulletProperty.accel));
        
        SimulationManager.AddTriggerBody(m_TriggerBody);
    }

    public void OnStart(BulletProperty bulletProperty, BulletSpawnTiming bulletSpawnTiming, BulletProperty newBulletProperty)
    {
        OnStart(bulletProperty);

        if (bulletSpawnTiming.spawnType != BulletSpawnType.None)
            StartCoroutine(SubBulletPattern(bulletSpawnTiming, newBulletProperty));
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        
        if (MainCamera.IsOutOfCamera(transform.position, BoundaryPadding))
            ReturnToPool();
        //PlayerManager.GetPlayerPosition() = PlayerManager.GetPlayerPosition();
    }
    
    private void LateUpdate()
    {
        if (_isRotating) {
            _bulletImageObject.transform.Rotate(Vector3.back, 200f / Application.targetFrameRate * Time.timeScale, Space.Self);
        }
    }

    private void OnBulletImageChanged()
    {
        _currentBullet = m_BulletData.bullets[(int)_bulletImage];
        _spriteRenderer.sprite = _currentBullet.sprite;
        _animator.runtimeAnimatorController = _currentBullet.animatorController;
        _bulletImageObject.SetActive(true);
        _isRotating = false;
        _bulletImageObject.transform.localRotation = Quaternion.identity;

        if (_bulletImage is BulletImage.PinkNeedle or BulletImage.BlueNeedle) { // Needle Form
            transform.rotation = Quaternion.Euler(0f, 0f, m_MoveVector.direction);
        }
        else if (_bulletImage is BulletImage.BlueSmall or BulletImage.BlueLarge) { // Blue Circle Form
            transform.rotation = Quaternion.identity;
            _isRotating = true;
        }
        else {
            transform.rotation = Quaternion.identity;
        }

        m_TriggerBody.SetCircleSize(_currentBullet.triggerBodyRadius);
        // _currentTriggerBody = m_TriggerBody;
        // if (_currentBullet.colliderSize.Length == 1)
        // {
        //     m_TriggerBody.SetCircleSize(_currentBullet.colliderSize[0]);
        //     _currentTriggerBody = m_TriggerBody;
        // }
        // else
        // {
        //     m_TriggerBodyCapsule.SetBoxSize(new Vector2(_currentBullet.colliderSize[0], _currentBullet.colliderSize[1]));
        //     _currentTriggerBody = m_TriggerBodyCapsule;
        // }
        m_TriggerBody.gameObject.SetActive(true);
    }

    private void SetSortingLayer()
    {
        _imageDepth = BulletManager.BulletsSortingLayer++;
        _spriteRenderer.sortingOrder = _imageDepth;
    }

    private IEnumerator ApplyBulletAccel(BulletAccel accel)
    {
        var targetSpeed = accel.targetSpeed;
        var duration = accel.duration;
        var frame = duration * Application.targetFrameRate / 1000;

        if (frame == 0)
        {
            m_MoveVector.speed = targetSpeed;
            yield break;
        }
        
        var initSpeed = m_MoveVector.speed;
        var easeType = (initSpeed < targetSpeed) ? EaseType.InQuad : EaseType.OutQuad;

        for (var i = 0; i < frame; ++i) {
            var t_spd = AC_Ease.ac_ease[(int)easeType].Evaluate((float) (i+1) / frame);
            
            m_MoveVector.speed = Mathf.Lerp(initSpeed, targetSpeed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private IEnumerator SubBulletPattern(BulletSpawnTiming spawnTiming, BulletProperty property)
    {
        if (_isPlayingEraseAnimation)
            yield break;
        if (spawnTiming.delay > 0)
            yield return new WaitForMillisecondFrames(spawnTiming.delay);

        _subBulletPattern.m_BulletProperty = property;

        switch (spawnTiming.spawnType)
        {
            case BulletSpawnType.EraseAndCreate:
                CreateSubBullet();
                PlayEraseAnimation(false);
                break;
            
            case BulletSpawnType.Create:
                while (true)
                {
                    CreateSubBullet();
                    if (spawnTiming.period == Vector2Int.zero)
                    {
                        break;
                    }
                    var period = Random.Range(spawnTiming.period.x, spawnTiming.period.y);
                    yield return new WaitForMillisecondFrames(period);
                }
                break;
        }
    }

    private void CreateSubBullet()
    {
        _subBulletPattern.m_BulletProperty.startPos = transform.position;
        StartCoroutine(_subBulletPattern.ExecutePattern());
    }

    public void ClampDirection(float pivotDirection, float rangeAngle)
    {
        var rangeAngleHalf = rangeAngle / 2;
        var deltaAngle = Mathf.DeltaAngle(m_MoveVector.direction, pivotDirection); // ~-45, -45~45, 45~
        if (deltaAngle < -rangeAngleHalf) {
            m_MoveVector.direction = pivotDirection + rangeAngleHalf;
        }
        else if (deltaAngle > rangeAngleHalf) {
            m_MoveVector.direction = pivotDirection - rangeAngleHalf;
        }
    }

    public void PlayEraseAnimation()
    {
        PlayEraseAnimation(true);
    }

    private void PlayEraseAnimation(bool hasSpeed)
    {
        if (_isPlayingEraseAnimation)
            return;
        if (!hasSpeed)
            m_MoveVector.speed = 0f;
        _isPlayingEraseAnimation = true;
        StopAllCoroutines();
        _bulletImageObject.SetActive(false);
        m_TriggerBody.gameObject.SetActive(false);
        
        m_EraseAnimator[_currentBullet.eraseIndex].gameObject.SetActive(true);
        m_EraseAnimator[_currentBullet.eraseIndex].Play("Erase", -1, 0f);
        StartCoroutine(ReturnToPoolDelayed(1000));
    }

    private IEnumerator ReturnToPoolDelayed(int millisecond) {
        RemoveFromBulletList();
        if (millisecond != 0) {
            yield return new WaitForMillisecondFrames(millisecond);
        }
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        
        RemoveFromBulletList();
        StopAllCoroutines();
        //_isPlayingEraseAnimation = false;
        //m_TriggerBody.gameObject.SetActive(false);
        m_EraseAnimator[_currentBullet.eraseIndex].gameObject.SetActive(false);
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.EnemyBullet);
    }

    private void OnDestroy()
    {
        RemoveFromBulletList();
    }

    private void OnDisable()
    {
        SimulationManager.RemoveTriggerBody(m_TriggerBody);
        //SimulationManager.RemoveTriggerBody(m_TriggerBodyCapsule);
    }

    private bool AddToBulletList()
    {
        if (_isInList)
            return false;
        _isInList = true;
        BulletManager.EnemyBulletList.AddLast(this);
        BulletManager.Action_OnBulletFreeStateStart += PlayEraseAnimation;
        return true;
    }

    private bool RemoveFromBulletList()
    {
        if (!_isInList)
            return false;
        _isInList = false;
        BulletManager.Action_OnBulletFreeStateStart -= PlayEraseAnimation;
        BulletManager.EnemyBulletList.Remove(this);
        return true;
    }
}
