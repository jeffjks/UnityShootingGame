using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : PlayerShooterManager
{
    public PlayerBomb m_PlayerBomb;

    private Transform m_MainCamera;
    private int m_ShotKeyPressFrame;
    private bool m_ShotKeyPrevious = false, m_NowShooting;
    private int m_AutoShot, m_ShotKeyPress = 0, m_BombKeyPress = 0;
    
    private PlayerManager m_PlayerManager = null;
    private SystemManager m_SystemManager = null;
    private PoolingManager m_PoolingManager = null;

    void Awake()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_SystemManager = SystemManager.instance_sm;
        m_PoolingManager = PoolingManager.instance_op;
    }

    void Start()
    {
        m_MainCamera = m_SystemManager.m_MainCamera.transform;

        for (int i = 0; i < PlayerWeapon.Length; i++) {
            m_PlayerWeaponName[i] = PlayerWeapon[i].GetComponent<PlayerWeapon>().m_ObjectName;
        }
        
        SetPreviewShooter();
        m_PlayerShotZ = Depth.PLAYER_MISSILE;

        
        
        if (PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Bomb) == 0) // 폭탄 개수
            m_SystemManager.SetMaxBombNumber(2);
        else
            m_SystemManager.SetMaxBombNumber(3);
        m_SystemManager.InitBombNumber();

        if (m_Module != 0) {
            SetModule();
            UpdateShotNumber();
            StartCoroutine(ModuleShot());
        }
        else
            UpdateShotNumber();
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
            
        if (SystemManager.GameMode != GameMode.GAMEMODE_REPLAY) {
            if (Input.GetButton("Fire1")) {
                m_ShotKeyPress = 1; // 버튼 누를시 m_ShotKeyPressFrame 증가
            }
            else {
                m_ShotKeyPress = 0;
            }

            if (Input.GetButtonDown("Fire2")) { // 폭탄 사용
                m_BombKeyPress = 1;
            }
        }
    }

    public void ResetKeyPress() {
        m_BombKeyPress = 0;
        m_ShotKeyPress = 0;
    }

    public void PlayerShooterBehaviour() {
        if (!m_PlayerManager.m_PlayerControlable) {
            m_PlayerUnit.m_SlowMode = false;
            m_NowAttacking = false;
            m_ShotKeyPress = 0;
            m_AutoShot = 0;
            m_PlayerLaserShooter.StopLaser();
            return;
        }
        
        if (m_ShotKeyPress == 1) {
            m_ShotKeyPressFrame++;
            if (!m_ShotKeyPrevious) {
                m_ShotKeyPrevious = true;
                if (!m_PlayerUnit.m_SlowMode) { // 샷 모드일 경우 AutoShot 증가
                    if (m_AutoShot <= 1) {
                        m_AutoShot++;
                    }
                }
            }
        }
        else {
            m_ShotKeyPrevious = false;
            m_ShotKeyPressFrame = 0;
            m_PlayerUnit.m_SlowMode = false;
            m_PlayerLaserShooter.StopLaser();
            m_NowAttacking = false;
        }
        
        if (!m_PlayerUnit.m_SlowMode) {
            if (m_ShotKeyPressFrame > Application.targetFrameRate / 2) { // 0.5초간 누르면 레이저 모드
                m_PlayerUnit.m_SlowMode = true;
                m_PlayerLaserShooter.StartLaser();
                m_NowAttacking = true;
                m_AutoShot = 0;
            }
        }

        if (m_AutoShot > 0) {
            if (!m_NowShooting) {
                m_NowShooting = true;
                StartCoroutine(Shot());
            }
            m_NowAttacking = true;
        }

        if (m_BombKeyPress == 1) {
            BombKeyPressed();
        }
    }

    private void BombKeyPressed() {
        if (m_SystemManager.GetBombNumber() <= 0) {
            return;
        }
        if (!m_PlayerBomb.GetEnableState()) {
            return;
        }
        if (m_SystemManager.m_PlayState >= 2) {
            return;
        }
        Vector3 bomb_pos = new Vector3(transform.position.x, transform.position.y, Depth.PLAYER_MISSILE);
        ((PlayerController) m_PlayerUnit).DisableInvincibility(4000);
        m_PlayerBomb.UseBomb();
        m_SystemManager.SetBombNumber(-1);
    }
    
    void OnEnable()
    {
        ResetKeyPress();
        m_PlayerUnit.m_SlowMode = false;
        m_NowShooting = false;
        m_NowAttacking = false;
        m_AutoShot = 0;
        m_SystemManager.InitBombNumber();
        m_ShotKeyPressFrame = 0;
        m_PlayerLaserShooter.StopLaser();
        if (m_PlayerManager != null) {
            StartCoroutine(ModuleShot());
        }
    }
    
    protected override IEnumerator Shot() {
        m_AutoShot--;
        for (int i = 0; i < m_ShotNumber; i++) { // m_FireRate초 간격으로 ShotNumber회 실행. 실행 주기는 m_FireDelay
            if (m_ShotDamage == 0)
                CreateShotNormal(m_ShotLevel);
            else if (m_ShotDamage == 1)
                CreateShotStrong(m_ShotLevel);
            else if (m_ShotDamage == 2)
                CreateShotVeryStrong(m_ShotLevel);
            else {
                m_ShotDamage = 0;
            }
            AudioService.PlaySound("PlayerShot1");
            yield return new WaitForMillisecondFrames(m_FireRate);
        }
        yield return new WaitForMillisecondFrames(m_FireDelayWait);

        m_NowShooting = false;
        CheckNowShooting();
        yield break;
    }

    private void CheckNowShooting() { // Now Attacking : 현재 공격 중 (모듈 공격을 위한 변수)
        if (m_AutoShot == 0) { // Now Shooting : 현재 샷 중 (샷 딜레이를 위한 변수)
            if (!m_PlayerUnit.m_SlowMode && !m_NowShooting)
                m_NowAttacking = false;
        }
    }

    protected override void CreatePlayerAttacks(string name, Vector3 pos, float dir, byte type = 0) {
        if (!IsOutside(pos)) {
            GameObject obj = m_PoolingManager.PopFromPool(name, PoolingParent.PLAYER_MISSILE);
            PlayerWeapon playerWeapon = obj.GetComponent<PlayerWeapon>();
            obj.transform.position = pos;
            playerWeapon.m_MoveVector.direction = dir;
            playerWeapon.m_DamageLevel = type;
            obj.SetActive(true);
            playerWeapon.OnStart();
        }
    }

    private bool IsOutside(Vector2 pos) {
        if (pos[0] <= m_MainCamera.position[0] - 6f) {
            return true;
        }
        else if (pos[0] >= m_MainCamera.position[0] + 6f) {
            return true;
        }
        else if (pos[1] <= m_MainCamera.position[1] - 8f) {
            return true;
        }
        else if (pos[1] >= m_MainCamera.position[1] + 8f) {
            return true;
        }
        return false;
    }

    public void PowerSet(int power) {
        m_ShotLevel = Mathf.Clamp(power, 0, 4);
        ResetLaser();
        UpdateShotNumber();
    }

    public void PowerUp() {
        if (m_ShotLevel < 4) {
            m_ShotLevel++;
            ResetLaser();
            m_SystemManager.DisplayScoreText("POWER UP");
        }
        else {
            m_SystemManager.AddScoreEffect(ItemScore.POWERUP);
        }
        UpdateShotNumber();
    }
    
    public void PowerDown() {
        if (m_ShotLevel > 0) {
            m_ShotLevel--;
            ResetLaser();
        }
        UpdateShotNumber();
    }

    public void AddBomb() {
        if (m_SystemManager.GetBombNumber() < m_SystemManager.GetMaxBombNumber()) {
            m_SystemManager.SetBombNumber(1);
            m_SystemManager.DisplayScoreText("BOMB");
        }
        else {
            m_SystemManager.AddScoreEffect(ItemScore.BOMB);
        }
    }
    
    public override void SetPreviewShooter() {
        m_ShotDamage = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.ShotLevel); // 샷 데미지
        m_Module = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.Module); // 모듈 종류
    }

    public int ShotKeyPress {
        get { return m_ShotKeyPress; }
        set { m_ShotKeyPress = value; }
    }

    public int BombKeyPress {
        get { return m_BombKeyPress; }
        set { m_BombKeyPress = value; }
    }
}
