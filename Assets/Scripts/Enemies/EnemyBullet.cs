using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class EnemyBullet : EnemyObject, IObjectPooling
{
    public string m_ObjectName;
    public BulletType m_BulletType;
    public BulletDatas m_BulletData;
    public CircleCollider2D m_CircleCollider;
    public CapsuleCollider2D m_CapsuleCollider;
    
    [HideInInspector] public int m_Timer;
    [HideInInspector] public OldBulletType m_Type;
    [HideInInspector] public byte m_NewImageType, m_NewDirectionType;
    [HideInInspector] public float m_NewDirectionAdder;
    [HideInInspector] public Vector2Int m_SecondTimer;
    [HideInInspector] public int m_NewNumber;
    [HideInInspector] public float m_NewInterval;
    public MoveVector m_NewMoveVector;
    public EnemyBulletAccel m_EnemyBulletAccel;
    public EnemyBulletAccel m_NewEnemyBulletAccel;
    private int _imageDepth;

    private bool _rotateBullet; // 자동 회전
    private GameObject _bulletImage;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private GameObject m_BulletExplosion;
    private Collider2D _currentCollider;
    private Bullet _currentBullet;
    //private Tween m_Tween = null;

    [SerializeField] private GameObject[] m_BulletTypeObject = null;
    
    [SerializeField] private GameObject[] _bulletEraseObject = null;
    [SerializeField] private Animator[] _eraseAnimator = null;

    /* m_Type ====================
    deltaValue가 0이면 속도 변화 X
    0 = 일반 총알
    1 = n초후 다른 총알 생성
    2 = n초후 파괴 후 다른 총알 생성

    생성 총알 : 0만 가능
    ImageType
    EnemyBulletAccel : targetSpeed = 0, deltaValue = 0
    MoveVector : speed = v, direction = 고정 방향 + @, 플레이어 방향 + @, 현재 방향 + @

    0 = image, pos, speed, direction, (accel)
    1, 2 = image, pos, speed, direction, (accel) / type, timer, new_image, new_speed, new_direction, direction_add , new_accel)

    second_timer : Vector(a, b) : a~b 사이 랜덤한 시간 간격으로 반복
    =========================== */

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = _bulletImage.GetComponent<Animator>();
        _bulletImage = _spriteRenderer.gameObject;
    }

    private void OnEnable()
    {
        BulletManager.BulletNumber++;
    }

    private void OnDisable()
    {
        BulletManager.BulletNumber--;
    }

    public void OnStart(BulletType bulletType, UnityAction<int, OldBulletType> subBullet) {
        //m_Position = Vector2Int.RoundToInt(transform.position*256);
        m_BulletType = bulletType;
        _rotateBullet = false;

        _currentBullet = m_BulletData.bullets[(int)m_BulletType];
        _spriteRenderer.sprite = _currentBullet.sprite;
        _animator.runtimeAnimatorController = _currentBullet.animatorController;

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

        if (m_BulletType is BulletType.PinkNeedle or BulletType.BlueNeedle) { // Needle Form
            transform.eulerAngles = new Vector3(0f, 0f, m_MoveVector.direction);
        }
        else if (m_BulletType is BulletType.BlueSmall or BulletType.BlueLarge) { // Blue Circle Form
            _bulletImage.transform.rotation = Quaternion.identity;
            _rotateBullet = true;
        }
        else {
            _bulletImage.transform.rotation = Quaternion.identity;
        }

        SetSortingLayer();

        if (BulletManager.InBulletFreeState) {
            PlayEraseAnimation();
            return;
        }

        switch(m_Type) {
            case OldBulletType.CREATE: // n초후 다른 총알 생성
                if (m_SecondTimer == Vector2Int.zero)
                    StartCoroutine(CreateSubBullet(m_Timer));
                else
                    StartCoroutine(CreateSubBullet(m_Timer, Random.Range(m_SecondTimer.x, m_SecondTimer.y)));
                break;
            case OldBulletType.ERASE_AND_CREATE: // n초후 다른 총알 생성 후 파괴
                StartCoroutine(CreateSubBullet(m_Timer));
                StartCoroutine(StopAndDeath(m_Timer));
                break;
            default:
                break;
        }

        float targetSpeed = m_EnemyBulletAccel.targetSpeed;
        int duration = m_EnemyBulletAccel.duration;

        if (targetSpeed > 0) {
            StartCoroutine(ChangeBulletSpeed(targetSpeed, duration));
        }
    }

    void Update()
    {
        CheckOutside();
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        //PlayerManager.GetPlayerPosition() = PlayerManager.GetPlayerPosition();
    }
    
    void LateUpdate()
    {
        if (_rotateBullet) {
            _bulletImage.transform.Rotate(Vector3.back, 200f / Application.targetFrameRate * Time.timeScale, Space.Self);
        }
    }

    private void SetSortingLayer()
    {
        _imageDepth = BulletManager.BulletsSortingLayer++;
        _spriteRenderer.sortingOrder = _imageDepth;
    }

    private IEnumerator ChangeBulletSpeed(float targetSpeed, int duration) {
        int frame = duration * Application.targetFrameRate / 1000;
        float init_speed = m_MoveVector.speed;
        int easeType;

        if (init_speed < targetSpeed) { // 가속
            easeType = EaseType.InQuad;
        }
        else { // 감속
            easeType = EaseType.OutQuad;
        }

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[easeType].Evaluate((float) (i+1) / frame);
            
            m_MoveVector.speed = Mathf.Lerp(init_speed, targetSpeed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator CreateSubBullet(int millisecond, int repeatMillisecond = -1) {
        if (millisecond != 0) {
            yield return new WaitForMillisecondFrames(millisecond);
        }

        while (true) {
            switch(m_NewDirectionType) {
                case BulletDirection.FIXED:
                    m_NewMoveVector.direction = 0f;
                    break;
                case BulletDirection.PLAYER:
                    m_NewMoveVector.direction = GetAngleToTarget(transform.position, PlayerManager.GetPlayerPosition());
                    break;
                case BulletDirection.CURRENT:
                    m_NewMoveVector.direction = m_MoveVector.direction;
                    break;
                default:
                    m_NewMoveVector.direction = 0f;
                    break;
            }
            Vector3 pos = transform.position;
            if (m_NewNumber == 0)
                CreateBullet(m_NewImageType, pos, m_NewMoveVector.speed, m_NewMoveVector.direction + m_NewDirectionAdder, m_NewEnemyBulletAccel);
            else
                CreateBulletsSector(m_NewImageType, pos, m_NewMoveVector.speed, m_NewMoveVector.direction + m_NewDirectionAdder, m_NewEnemyBulletAccel, m_NewNumber, m_NewInterval);
            
            if (repeatMillisecond == -1) {
                break;
            }
            else {
                yield return new WaitForMillisecondFrames(repeatMillisecond);
            }
        }
        yield break;
    }

    private IEnumerator StopAndDeath(int millisecond) {
        if (millisecond != 0) {
            yield return new WaitForMillisecondFrames(millisecond);
        }
        //m_Tween.Kill();
        m_MoveVector.speed = 0f;
        PlayEraseAnimation();
        yield break;
    }

    public void PlayEraseAnimation() {
        StopAllCoroutines();
        _bulletImage.SetActive(false);
        _currentCollider.gameObject.SetActive(false);

        var eraseIndex = (m_BulletType is BulletType.BlueLarge or BulletType.BlueNeedle or BulletType.BlueSmall) ? 0 : 1;
        _bulletEraseObject[eraseIndex].SetActive(true);
        _eraseAnimator[eraseIndex].Play("Erase", -1, 0f);
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
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.EnemyBullet);
    }

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
    }
}
