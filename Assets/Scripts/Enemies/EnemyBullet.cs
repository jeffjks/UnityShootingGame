using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class EnemyBullet : EnemyObject, IObjectPooling
{
    public string m_ObjectName;
    public BulletDatas m_BulletData;
    public CircleCollider2D m_CircleCollider;
    public CapsuleCollider2D m_CapsuleCollider;
    public Animator[] m_EraseAnimator;
    
    /*
    [HideInInspector] public int m_Timer;
    [HideInInspector] public OldBulletType m_Type;
    [HideInInspector] public byte m_NewImageType, m_NewDirectionType;
    [HideInInspector] public float m_NewDirectionAdder;
    [HideInInspector] public Vector2Int m_SecondTimer;
    [HideInInspector] public int m_NewNumber;
    [HideInInspector] public float m_NewInterval;
    public MoveVector m_NewMoveVector;
    public BulletAccel m_NewBulletAccel;*/
    private int _imageDepth;
    
    private bool _isPlayingEraseAnimation;
    private GameObject _bulletImageObject;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private GameObject m_BulletExplosion;
    private Collider2D _currentCollider;
    private Bullet _currentBullet;
    private BulletImage _bulletImage;
    private SubBulletPattern _subBulletPattern;

    //private Tween m_Tween = null;

    public EnemyObject OwnerEnemyObject { private get; set; }

    public BulletImage BulletImage
    {
        set
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

    private void OnEnable()
    {
        BulletManager.BulletNumber++;
        BulletManager.Action_OnBulletFreeStateStart += PlayEraseAnimation;
    }

    private void OnDisable()
    {
        BulletManager.Action_OnBulletFreeStateStart -= PlayEraseAnimation;
        BulletManager.BulletNumber--;
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
        
        m_MoveVector = new MoveVector(bulletProperty.speed, pivotDirection + bulletProperty.direction);
        BulletImage = bulletProperty.image;

        if (BulletManager.InBulletFreeState)
        {
            PlayEraseAnimation();
            return;
        }

        /*
        switch(m_Type) {
            case BulletSpawnType.Create: // n초후 다른 총알 생성
                if (m_SecondTimer == Vector2Int.zero)
                    StartCoroutine(CreateSubBullet(m_Timer));
                else
                    StartCoroutine(CreateSubBullet(m_Timer, Random.Range(m_SecondTimer.x, m_SecondTimer.y)));
                break;
            case BulletSpawnType.EraseAndCreate: // n초후 다른 총알 생성 후 파괴
                StartCoroutine(CreateSubBullet(m_Timer));
                StartCoroutine(StopAndDeath(m_Timer));
                break;
            default:
                break;
        }*/

        if (bulletProperty.accel.duration > 0)
            StartCoroutine(ApplyBulletAccel(bulletProperty.accel));
    }

    public void OnStart(BulletProperty bulletProperty, BulletSpawnTiming bulletSpawnTiming, BulletProperty newBulletProperty)
    {
        OnStart(bulletProperty);

        if (bulletSpawnTiming.spawnType != BulletSpawnType.None)
            StartCoroutine(SubBulletPattern(bulletSpawnTiming, newBulletProperty));
    }

    void Update()
    {
        CheckOutside();
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        //PlayerManager.GetPlayerPosition() = PlayerManager.GetPlayerPosition();
    }
    
    void LateUpdate()
    {
        if (IsRotatable) {
            _bulletImageObject.transform.Rotate(Vector3.back, 200f / Application.targetFrameRate * Time.timeScale, Space.Self);
        }
    }

    private void OnBulletImageChanged()
    {
        if (_currentBullet == m_BulletData.bullets[(int)_bulletImage])
            return;
        
        _currentBullet = m_BulletData.bullets[(int)_bulletImage];
        _spriteRenderer.sprite = _currentBullet.sprite;
        _animator.runtimeAnimatorController = _currentBullet.animatorController;
        IsRotatable = false;

        if (_bulletImage is BulletImage.PinkNeedle or BulletImage.BlueNeedle) { // Needle Form
            transform.eulerAngles = new Vector3(0f, 0f, m_MoveVector.direction);
        }
        else if (_bulletImage is BulletImage.BlueSmall or BulletImage.BlueLarge) { // Blue Circle Form
            _bulletImageObject.transform.rotation = Quaternion.identity;
            IsRotatable = true;
        }
        else {
            _bulletImageObject.transform.rotation = Quaternion.identity;
        }

        if (_currentBullet.colliderSize.Length == 1)
        {
            m_CircleCollider.radius = _currentBullet.colliderSize[0];
            _currentCollider = m_CircleCollider;
        }
        else
        {
            m_CapsuleCollider.size = new Vector2(_currentBullet.colliderSize[0], _currentBullet.colliderSize[1]);
            _currentCollider = m_CapsuleCollider;
        }
        _currentCollider.gameObject.SetActive(true);
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
            var t_spd = AC_Ease.ac_ease[easeType].Evaluate((float) (i+1) / frame);
            
            m_MoveVector.speed = Mathf.Lerp(initSpeed, targetSpeed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private IEnumerator SubBulletPattern(BulletSpawnTiming spawnTiming, BulletProperty property)
    {
        if (spawnTiming.delay > 0)
            yield return new WaitForMillisecondFrames(spawnTiming.delay);

        _subBulletPattern.m_BulletProperty = property;
        _subBulletPattern.m_BulletProperty.startPos = transform.position;

        switch (spawnTiming.spawnType)
        {
            case BulletSpawnType.EraseAndCreate:
                StartCoroutine(_subBulletPattern.ExecutePattern());
                PlayEraseAnimation();
                yield break;
            
            case BulletSpawnType.Create:
                while (true)
                {
                    StartCoroutine(_subBulletPattern.ExecutePattern());
                    yield return new WaitForMillisecondFrames(Random.Range(spawnTiming.period.x, spawnTiming.period.y));
                }
        }
    }

    /*
    private IEnumerator CreateSubBullet(int millisecond, int repeatMillisecond = -1) {
        if (millisecond != 0) {
            yield return new WaitForMillisecondFrames(millisecond);
        }

        while (true) {
            switch(m_NewDirectionType) {
                case BulletPivot.Fixed:
                    m_NewMoveVector.direction = 0f;
                    break;
                case BulletPivot.Player:
                    m_NewMoveVector.direction = GetAngleToTarget(transform.position, PlayerManager.GetPlayerPosition());
                    break;
                case BulletPivot.Current:
                    m_NewMoveVector.direction = m_MoveVector.direction;
                    break;
                default:
                    m_NewMoveVector.direction = 0f;
                    break;
            }
            Vector3 pos = transform.position;
            if (m_NewNumber == 0)
                CreateBullet(m_NewImageType, pos, m_NewMoveVector.speed, m_NewMoveVector.direction + m_NewDirectionAdder, m_NewBulletAccel);
            else
                CreateBulletsSector(m_NewImageType, pos, m_NewMoveVector.speed, m_NewMoveVector.direction + m_NewDirectionAdder, m_NewBulletAccel, m_NewNumber, m_NewInterval);
            
            if (repeatMillisecond == -1) {
                break;
            }
            else {
                yield return new WaitForMillisecondFrames(repeatMillisecond);
            }
        }
        yield break;
    }*/

    public void PlayEraseAnimation()
    {
        if (_isPlayingEraseAnimation)
            return;
        _isPlayingEraseAnimation = true;
        StopAllCoroutines();
        _bulletImageObject.SetActive(false);
        _currentCollider.gameObject.SetActive(false);
        
        m_EraseAnimator[_currentBullet.eraseIndex].gameObject.SetActive(true);
        m_EraseAnimator[_currentBullet.eraseIndex].Play("Erase", -1, 0f);
        StartCoroutine(ReturnToPoolDelayed(1000));
    }

    private void CheckOutside() { // 화면 바깥으로 나갈시 파괴
        float gap = 0.5f;
        Vector3 pos = transform.position;
        if (Mathf.Abs(pos.x) > Size.GAME_WIDTH*0.5f + gap) {
            ReturnToPool();
        }
        else if (Mathf.Abs(pos.y + Size.GAME_HEIGHT*0.5f) > Size.GAME_HEIGHT*0.5f + gap) {
            ReturnToPool();
        }
    }

    private IEnumerator ReturnToPoolDelayed(int millisecond) {
        if (millisecond != 0) {
            yield return new WaitForMillisecondFrames(millisecond);
        }
        ReturnToPool();
    }

    public void ReturnToPool() {
        StopAllCoroutines();
        _isPlayingEraseAnimation = false;
        m_EraseAnimator[_currentBullet.eraseIndex].gameObject.SetActive(false);
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.EnemyBullet);
    }

    /*
    protected override bool CanCreateBullet(Vector3 pos) {
        float camera_x = MainCamera.Camera.transform.position.x;

        if (!PlayerManager.IsPlayerAlive)
            return false;
        if (!SystemManager.IsOnGamePlayState())
            return false;
        if (2 * Mathf.Abs(pos.x - camera_x) > Size.MAIN_CAMERA_WIDTH)
            return false;
        if (pos.y is > 0 or < - Size.MAIN_CAMERA_HEIGHT)
            return false;
        
        return true;
    }*/
}
