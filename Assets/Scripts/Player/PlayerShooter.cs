using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : PlayerShooterManager
{
    public GameObject m_Bomb = null;

    private AudioSource m_AudioSource;
    private Transform m_MainCamera;
    private int m_DefaultBombNumber, m_BombNumber, m_MaxBombNumber = 5;
    private bool m_BombEnable = true;
    
    private PlayerManager m_PlayerManager = null;
    private PoolingManager m_PoolingManager = null;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        m_AudioSource = GetComponent<AudioSource>();
        m_MainCamera = m_PlayerManager.m_MainCamera.transform;

        for (int i = 0; i < PlayerMissile.Length; i++) {
            m_PlayerMissileName[i] = PlayerMissile[i].GetComponent<PlayerMissile>().m_ObjectName;
        }
        
        m_ShotDamage = m_PlayerManager.m_CurrentAttributes.m_ShotDamage; // 샷 데미지
        m_LaserDamage = m_PlayerManager.m_CurrentAttributes.m_LaserDamage; // 레이저 데미지
        m_Module = m_PlayerManager.m_CurrentAttributes.m_Module; // 모듈 종류
        m_PlayerShotZ = Depth.PLAYER_MISSILE;
        
        if (m_PlayerManager.m_CurrentAttributes.m_Bomb == 0) // 폭탄 개수
            m_DefaultBombNumber = 2;
        else
            m_DefaultBombNumber = 3;
        m_BombNumber = m_DefaultBombNumber;

        if (m_Module != 0) {
            if (m_Module == 1) {
                m_ModuleMinDelay = m_HomingMissileMinDelay;
                m_ModuleMaxDelay = m_HomingMissileMaxDelay;
            }
            else if (m_Module == 2) {
                m_ModuleMinDelay = m_RocketMinDelay;
                m_ModuleMaxDelay = m_RocketMaxDelay;
            }
            else if (m_Module == 3) {
                m_ModuleMinDelay = m_AddShotMinDelay;
                m_ModuleMaxDelay = m_AddShotMaxDelay;
            }
            m_ModuleDelay = (m_ModuleMaxDelay - m_ModuleMinDelay) / 4;

            UpdateShotNumber();
            StartCoroutine(ModuleShot());
        }
        else
            UpdateShotNumber();
    }

    void Update ()
    {
        if (Time.timeScale == 0)
            return;

        if (Input.GetButton("Fire1") && m_PlayerManager.PlayerControlable) {
            if (m_ShotKeyPressTime < 10f) {
                m_ShotKeyPressTime += Time.deltaTime; // 버튼 누를시 m_ShotKeyPressTime 증가
            }
        }
        else {
            m_ShotKeyPressTime = 0f;
            if (m_PlayerController.m_SlowMode == true) { // 버튼 떼면 샷 모드로
                m_PlayerController.m_SlowMode = false;
                m_PlayerLaserShooter.StopLaser();
                m_NowAttacking = false;
            }
        }

        if (m_PlayerManager.PlayerControlable) {
            if (Input.GetButtonDown("Fire1")) {
                if (m_PlayerController.m_SlowMode == false) { // 샷 모드일 경우 샷 발사
                    if (m_AutoShot <= 1)
                        m_AutoShot++;
                }
            }

            if (m_PlayerController.m_SlowMode == false) {
                if (m_ShotKeyPressTime > 0.5f) { // 0.5초간 누르면 레이저 모드
                    m_PlayerController.m_SlowMode = true;
                    m_PlayerLaserShooter.StartLaser();
                    m_NowAttacking = true;
                    m_AutoShot = 0;
                }
            }
        }
        else {
            m_AutoShot = 0;
        }

        if (m_AutoShot > 0) {
            if (!m_NowShooting) {
                m_NowShooting = true;
                StartCoroutine(Shot());
            }
            m_NowAttacking = true;
        }

        if (Input.GetButtonDown("Fire2")) { // 폭탄 사용
            if (m_PlayerManager.PlayerControlable) {
                if (m_BombNumber > 0) {
                    if (m_BombEnable) {
                        if (!m_Bomb.activeSelf) {
                            Vector3 bomb_pos = new Vector3(transform.position.x, transform.position.y, Depth.PLAYER_MISSILE);
                            ((PlayerController) m_PlayerController).EnableInvincible(4f);
                            m_Bomb.SetActive(true);
                            m_BombEnable = false;
                            m_BombNumber--;
                            Invoke("EnableBomb", 3f); // 폭탄 쿨타임
                        }
                    }
                }
            }
        }
    }

    private void EnableBomb() {
        m_BombEnable = true;
    }
    
    void OnEnable()
    {
        m_PlayerController.m_SlowMode = false;
        m_NowShooting = false;
        m_NowAttacking = false;
        m_AutoShot = 0;
        m_BombNumber = m_DefaultBombNumber;
        m_ShotKeyPressTime = 0f;
        m_PlayerLaserShooter.StopLaser();
        if (m_PlayerManager != null) {
            StartCoroutine(ModuleShot());
        }
    }
    
    protected new IEnumerator Shot() {
        m_AutoShot--;
        for (int i = 0; i < m_ShotNumber; i++) { // m_FireRate초 간격으로 ShotNumber회 실행. 실행 주기는 m_FireDelay
            if (m_AudioSource != null)
                m_AudioSource.Play();
            if (m_ShotDamage == 0)
                CreateShotNormal(m_ShotLevel);
            else if (m_ShotDamage == 1)
                CreateShotStrong(m_ShotLevel);
            else if (m_ShotDamage == 2)
                CreateShotVeryStrong(m_ShotLevel);
            else {
                m_ShotDamage = 0;
            }
            yield return new WaitForSeconds(m_FireRate);
        }
        yield return new WaitForSeconds(m_FireDelayWait); // m_FireDelay에서 m_FireRate가 차지하는 부분 빼기
        m_NowShooting = false;
        CheckNowShooting();
        yield break;
    }

    protected override void CheckNowShooting() { // Now Attacking : 현재 공격 중 (모듈 공격을 위한 변수)
        if (m_AutoShot == 0) { // Now Shooting : 현재 샷 중 (샷 딜레이를 위한 변수)
            if (!m_PlayerController.m_SlowMode && !m_NowShooting)
                m_NowAttacking = false;
        }
    }

    protected override void CreatePlayerAttacks(string name, Vector3 pos, Quaternion rot, byte type = 0) {
        if (!IsOutside(pos)) {
            GameObject obj = m_PoolingManager.PopFromPool(name, PoolingParent.PLAYER_MISSILE);
            PlayerMissile playerMissile = obj.GetComponent<PlayerMissile>();
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            playerMissile.m_DamageLevel = type;
            obj.SetActive(true);
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
        }
        else {
            // 점수 +
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

    private void ResetLaser() {
        if (m_PlayerController.m_SlowMode) {
            m_PlayerLaserShooter.StopLaser();
            m_PlayerLaserShooter.StartLaser();
        }
    }

    public void AddBomb() {
        if (m_BombNumber < m_MaxBombNumber) {
            m_BombNumber++;
        }
        else {
            // 점수 +
        }
    }

    private void UpdateShotNumber() {
        for (int i = 0; i < m_PlayerDrone.Length; i++)
            m_PlayerDrone[i].SetShotLevel(m_ShotLevel);

        if (m_ShotLevel <= -1) {
            m_PlayerDrone[2].gameObject.SetActive(false);
            m_PlayerDrone[3].gameObject.SetActive(false);
        }
        else {
            m_PlayerDrone[2].gameObject.SetActive(true);
            m_PlayerDrone[3].gameObject.SetActive(true);
            m_PlayerDrone[2].transform.localPosition = new Vector2(0f, -1f);
            m_PlayerDrone[3].transform.localPosition = new Vector2(0f, -1f);
        }
        // 0 / 1 2 / 3 4
        if (m_ShotLevel == 0) {
            m_ShotNumber = 3;
        }
        else if (m_ShotLevel <= 2) {
            m_ShotNumber = 4;
        }
        else {
            m_ShotNumber = 5;
        }

        m_CurrentModuleDelay = m_ModuleMaxDelay - m_ModuleDelay*m_ShotLevel;
        m_FireDelayWait = m_FireDelay - m_FireRate*m_ShotNumber;
    }
}
