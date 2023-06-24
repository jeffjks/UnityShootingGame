using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyBullet : EnemyObject, IObjectPooling
{
    public string m_ObjectName;

    [HideInInspector] public byte m_ImageType;
    [HideInInspector] public int m_Timer;
    [HideInInspector] public byte m_Type, m_NewImageType, m_NewDirectionType;
    [HideInInspector] public float m_NewDirectionAdder;
    [HideInInspector] public Vector2Int m_SecondTimer;
    [HideInInspector] public int m_NewNumber;
    [HideInInspector] public float m_NewInterval;
    public MoveVector m_NewMoveVector;
    public EnemyBulletAccel m_EnemyBulletAccel;
    public EnemyBulletAccel m_NewEnemyBulletAccel;
    public int ImageDepth { get; set; }

    private bool m_RotateBullet = false; // 자동 회전
    private GameObject m_BulletExplosion;
    private SpriteRenderer[] m_SpriteRenderers;
    //private Tween m_Tween = null;

    [SerializeField] private GameObject[] m_BulletTypeObject = null;
    [SerializeField] private GameObject[] m_BulletEraseObject = null;
    [SerializeField] private Animator[] m_EraseAnimator = null;

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
        m_SpriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void OnEnable()
    {
        BulletManager.BulletNumber++;
    }

    private void OnDisable()
    {
        BulletManager.BulletNumber--;
    }

    public void OnStart() {
        //m_Position = Vector2Int.RoundToInt(transform.position*256);
        m_RotateBullet = false;
        
        for (int i = 0; i < m_BulletTypeObject.Length; i++) {
            m_BulletTypeObject[i].SetActive(false);
        }
        for (int i = 0; i < m_BulletEraseObject.Length; i++) {
            m_BulletEraseObject[i].SetActive(false);
        }

        if (m_BulletTypeObject.Length > 0) {
            m_BulletTypeObject[m_ImageType].SetActive(true);
        }

        if (m_ImageType == 1 || m_ImageType == 4) { // Needle Form
            m_BulletTypeObject[m_ImageType].transform.eulerAngles = new Vector3(0f, 0f, m_MoveVector.direction);
        }
        else if (m_ImageType == 3 || m_ImageType == 5) { // Blue Normal Form
            m_RotateBullet = true;
        }
        else {
            m_BulletTypeObject[m_ImageType].transform.rotation = Quaternion.identity;
        }

        SetSortingLayer();

        switch(m_Type) {
            case BulletType.CREATE: // n초후 다른 총알 생성
                if (m_SecondTimer == Vector2Int.zero)
                    StartCoroutine(CreateSubBullet(m_Timer));
                else
                    StartCoroutine(CreateSubBullet(m_Timer, Random.Range(m_SecondTimer.x, m_SecondTimer.y)));
                break;
            case BulletType.ERASE_AND_CREATE: // n초후 다른 총알 생성 후 파괴
                StartCoroutine(CreateSubBullet(m_Timer));
                StartCoroutine(StopAndDeath(m_Timer));
                break;
            default:
                break;
        }

        if (BulletManager.InBulletFreeState) {
            PlayEraseAnimation();
        }

        float targetSpeed = m_EnemyBulletAccel.targetSpeed;
        int duration = m_EnemyBulletAccel.duration;

        if (targetSpeed > 0) {
            StartCoroutine(ChangeBulletSpeed(targetSpeed, duration));
        }
    }

    void Update()
    {
        CheckDeath();
        CheckOutside();
        MoveDirection(m_MoveVector.speed, m_MoveVector.direction);
        //PlayerManager.GetPlayerPosition() = PlayerManager.GetPlayerPosition();
    }
    
    void LateUpdate()
    {
        if (m_RotateBullet) {
            m_BulletTypeObject[m_ImageType].transform.Rotate(Vector3.back, 200 / Application.targetFrameRate * Time.timeScale, Space.Self);
        }
    }

    private void SetSortingLayer()
    {
        ImageDepth = BulletManager.BulletsSortingLayer++;
        m_SpriteRenderers[m_ImageType].sortingOrder = ImageDepth;
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
        m_BulletTypeObject[m_ImageType].SetActive(false);

        byte erase_type;
        if (m_ImageType <= 2) {
            erase_type = 0;
        }
        else {
            erase_type = 1;
        }
        m_BulletEraseObject[erase_type].SetActive(true);
        m_EraseAnimator[erase_type].Play("Erase", -1, 0f);
        StartCoroutine(ReturnToPoolDelayed(1000));
    }

    private void CheckDeath() { // 탄 소거시 Bullet 파괴
        if (BulletManager.InBulletFreeState) {
            if (m_BulletTypeObject[m_ImageType].activeSelf) {
                PlayEraseAnimation();
            }
        }
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
        yield break;
    }

    public void ReturnToPool() {
        StopAllCoroutines();
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.EnemyBullet);
    }

    protected override bool BulletCondition(Vector3 pos) {
        Vector2 camera_pos = MainCamera.Camera.transform.position;

        if (!PlayerManager.IsPlayerAlive) {
            return false;
        }
        else if (!SystemManager.IsOnGamePlayState()) {
            return false;
        }
        else if (2 * Mathf.Abs(pos.x - camera_pos.x) > Size.MAIN_CAMERA_WIDTH) {
            return false;
        }
        else if (pos.y > 0 || pos.y < - Size.MAIN_CAMERA_HEIGHT) {
            return false;
        }
        else
            return true;
    }
}
