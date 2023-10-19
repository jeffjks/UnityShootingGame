using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerShotHandler : MonoBehaviour
{
    public Transform[] m_PlayerShotPosition = new Transform[7];
    public PlayerUnit m_PlayerUnit;
    
    private const int FIRE_RATE = 50; // 50

    public PlayerDamageDatas m_PlayerShotData;
    public PlayerDamageDatas[] m_PlayerSubWeaponDamageData;
    public int AutoShot { get; set; }
    public float PiercingShotDirection { get; private set; }
    private float PiercingShotTargetDirection { get; set; }
    private const float MAX_PIERCING_SHOT_DIRECTION = 30f;

    private const string PLAYER_SHOT = "PlayerShot";
    
    private List<int> _currentSubWeaponDelay = new();
    private PlayerSubWeaponManager _playerSubWeaponManager;
    private PlayerDrone[] _playerDrones;

    private readonly List<ISubWeapon> _subWeapons = new()
    {
        new PlayerSubWeaponNone(),
        new PlayerSubWeaponHomingMissile(),
        new PlayerSubWeaponRocket(),
        new PlayerSubWeaponPiercingShot()
    };
    
    private ISubWeapon _currentSubWeapon;

    private int _shotIndex;
    private int _subWeaponIndex;

    public int ShotIndex
    {
        get => _shotIndex;
        set
        {
            _shotIndex = value;
            Action_OnShotIndexChanged?.Invoke();
        }
    }

    public int SubWeaponIndex
    {
        get => _subWeaponIndex;
        set
        {
            _subWeaponIndex = value;
            SetSubWeaponIndex();
        }
    }
    
    private readonly int[] _shotNumbers = { 3, 4, 4, 5, 5 };
    
    public event Action Action_OnShotIndexChanged;

    private void Awake()
    {
        _playerSubWeaponManager = new PlayerSubWeaponManager();
        _playerDrones = GetComponentsInChildren<PlayerDrone>();
        SubWeaponIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.SubWeaponIndex);
        ShotIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.ShotIndex);
        m_PlayerUnit.Action_OnControllableChanged += (controllable) =>
        {
            if (controllable)
                return;
            AutoShot = 0;
        };
    }
    
    private void OnEnable()
    {
        PiercingShotDirection = 0f;
        StartCoroutine(SubWeaponShot());
    }

    private void OnDisable()
    {
        AutoShot = 0;
        m_PlayerUnit.SlowMode = false;
        m_PlayerUnit.IsAttacking = false;
        m_PlayerUnit.IsShooting = false;
        StopAllCoroutines();
    }

    private void Update()
    {
        PiercingShotDirection = Mathf.MoveTowardsAngle(PiercingShotDirection, PiercingShotTargetDirection, 3f);
        PiercingShotDirection = Mathf.Clamp(PiercingShotDirection, -MAX_PIERCING_SHOT_DIRECTION, MAX_PIERCING_SHOT_DIRECTION);
    }

    private IEnumerator SubWeaponShot() {
        while(true) {
            if (SubWeaponIndex > 0 && m_PlayerUnit.IsAttacking) {
                _currentSubWeapon.Shoot(this, m_PlayerSubWeaponDamageData[SubWeaponIndex - 1], m_PlayerUnit.PlayerAttackLevel);
                yield return new WaitForMillisecondFrames(_currentSubWeaponDelay[m_PlayerUnit.PlayerAttackLevel]);
            }
            yield return new WaitForFrames(0);
        }
    }

    public void ReceiveHorizontalMovement(float movement)
    {
        if (!PlayerUnit.IsControllable)
            return;
        PiercingShotTargetDirection = - MAX_PIERCING_SHOT_DIRECTION * Math.Sign(movement);
    }

    public void StartShotCoroutine()
    {
        StartCoroutine(Shot());
    }
    
    private IEnumerator Shot() {
        if (AutoShot > 0)
            AutoShot--;
        var shotNumber = _shotNumbers[m_PlayerUnit.PlayerAttackLevel];
        
        for (int i = 0; i < shotNumber; i++) { // FIRE_RATE초 간격으로 ShotNumber회 실행. 실행 주기는 m_FireDelay
            if (ShotIndex == 0)
                CreateShotNormal(m_PlayerUnit.PlayerAttackLevel);
            else if (ShotIndex == 1)
                CreateShotStrong(m_PlayerUnit.PlayerAttackLevel);
            else if (ShotIndex == 2)
                CreateShotVeryStrong(m_PlayerUnit.PlayerAttackLevel);
            
            if (!m_PlayerUnit.m_IsPreviewObject)
                AudioService.PlaySound("PlayerShot1");
            yield return new WaitForMillisecondFrames(FIRE_RATE);
        }
        yield return new WaitForMillisecondFrames(m_PlayerShotData.cooldownByLevel[0] - FIRE_RATE*shotNumber);

        m_PlayerUnit.IsShooting = false;
        CheckNowShooting();
    }

    private void CheckNowShooting() { // m_PlayerUnit.IsAttacking : 현재 공격 중 (모듈 공격을 위한 변수)
        if (m_PlayerUnit.m_IsPreviewObject)
        {
            return;
        }
        if (AutoShot == 0) { // m_PlayerUnit.IsShooting : 현재 샷 중 (샷 딜레이를 위한 변수)
            if (!m_PlayerUnit.SlowMode && !m_PlayerUnit.IsShooting)
                m_PlayerUnit.IsAttacking = false;
        }
    }

    public void CreatePlayerAttack(string objectName, PlayerDamageDatas playerDamage, Vector3 pos, float dir, int damageLevel)
    {
        GameObject obj = PoolingManager.PopFromPool(objectName, PoolingParent.PlayerMissile);
        PlayerWeapon playerWeapon = obj.GetComponent<PlayerWeapon>();
        obj.transform.position = pos;
        playerWeapon.m_MoveVector.direction = dir;
        obj.SetActive(true);
        playerWeapon.OnStart();
        playerWeapon.DamageLevel = damageLevel;
    }

    private void CreateShotNormal(int level) {
        Vector3[] shotPosition = new Vector3[5];
        float[] shotDirection = new float[5];
        //Quaternion[] shotDirection = new Quaternion[5];

        for (int i = 0; i < 5; i++) {
            //shotDirection[i] = m_PlayerShotPosition[i].rotation * Quaternion.Euler(90f, 0f, 0f);
            if (i == 0) {
                shotDirection[i] = 180f;
            }
            else {
                shotDirection[i] = 180f - _playerDrones[i - 1].GetCurrentLocalRotation();
            }
            shotPosition[i] = new Vector3(m_PlayerShotPosition[i].position[0], m_PlayerShotPosition[i].position[1], Depth.PLAYER_MISSILE);
        }

        if (level <= 1) { // ----------------------------------[ 0 0 0 0 0 ]
            for (int i = 0; i < 5; i++) {
                CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[i], shotDirection[i], 0);
            }
        }
        else if (level <= 3) { // ---------------------------------- [ 0 1 0 1 0 ]
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[0], shotDirection[0], 0);
            for (int i = 1; i < 3; i++) {
                CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[i], shotDirection[i], 1);
                CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[i+2], shotDirection[i+2], 0);
            }
        }
        else { // ---------------------------------- [ 0 1 2 1 0 ]
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[0], shotDirection[0], 2);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[1], shotDirection[1], 1);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[2], shotDirection[2], 1);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[3], shotDirection[3], 0);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[4], shotDirection[4], 0);
        }
    }
    
    private void CreateShotStrong(int level) {
        Vector3[] shotPosition = new Vector3[5];
        float[] shotDirection = new float[5];
        //Quaternion[] shotDirection = new Quaternion[5];

        for (int i = 0; i < 5; i++) {
            //shotDirection[i] = m_PlayerShotPosition[i].rotation * Quaternion.Euler(90f, 0f, 0f);
            if (i == 0) {
                shotDirection[i] = 180f;
            }
            else {
                shotDirection[i] = 180f - _playerDrones[i - 1].GetCurrentLocalRotation();
            }
            shotPosition[i] = new Vector3(m_PlayerShotPosition[i].position[0], m_PlayerShotPosition[i].position[1], Depth.PLAYER_MISSILE);
        }
        if (level <= 1) { // ---------------------------------- [ 0 1 0 1 0 ]
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[0], shotDirection[0], 0);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[1], shotDirection[1], 1);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[2], shotDirection[2], 1);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[3], shotDirection[3], 0);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[4], shotDirection[4], 0);
        }
        else if (level <= 3) { // ---------------------------------- [ 0 1 2 1 0 ]
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[0], shotDirection[0], 2);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[1], shotDirection[1], 1);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[2], shotDirection[2], 1);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[3], shotDirection[3], 0);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[4], shotDirection[4], 0);
        }
        else { // ---------------------------------- [ 1 1 2 1 1 ]
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[0], shotDirection[0], 2);
            for (int i = 1; i < 5; i++) {
                CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[i], shotDirection[i], 1);
            }
        }
    }

    private void CreateShotVeryStrong(int level) {
        Vector3[] shotPosition = new Vector3[5];
        float[] shotDirection = new float[5];
        //Quaternion[] shotDirection = new Quaternion[5];

        for (int i = 0; i < 5; i++) {
            //shotDirection[i] = m_PlayerShotPosition[i].rotation * Quaternion.Euler(90f, 0f, 0f);
            if (i == 0) {
                shotDirection[i] = 180f;
            }
            else {
                shotDirection[i] = 180f - _playerDrones[i - 1].GetCurrentLocalRotation();
            }
            shotPosition[i] = new Vector3(m_PlayerShotPosition[i].position[0], m_PlayerShotPosition[i].position[1], Depth.PLAYER_MISSILE);
        }

        if (level <= 1) { // ---------------------------------- [ 0 1 1 1 0 ]
            for (int i = 0; i < 3; i++) {
                CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[i], shotDirection[i], 1);
            }
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[3], shotDirection[3], 0);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[4], shotDirection[4], 0);
        }
        else if (level <= 3) { // ---------------------------------- [ 1 1 2 1 1 ]
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[0], shotDirection[0], 2);
            for (int i = 1; i < 5; i++) {
                CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[i], shotDirection[i], 1);
            }
        }
        else { // ---------------------------------- [ 1 2 2 2 1 ]
            for (int i = 0; i < 3; i++) {
                CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[i], shotDirection[i], 2);
            }
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[3], shotDirection[3], 1);
            CreatePlayerAttack(PLAYER_SHOT, m_PlayerShotData, shotPosition[4], shotDirection[4], 1);
        }
    }

    private void SetSubWeaponIndex()
    {
        _currentSubWeapon = _subWeapons[SubWeaponIndex];

        if (SubWeaponIndex == 0)
        {
            return;
        }
        
        _currentSubWeaponDelay = m_PlayerSubWeaponDamageData[SubWeaponIndex - 1].cooldownByLevel;
        _playerSubWeaponManager.ChangeSubWeapon(_currentSubWeapon);
    }
}
