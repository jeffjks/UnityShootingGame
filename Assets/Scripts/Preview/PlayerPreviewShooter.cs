using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewShooter : PlayerShooterManager
{
    public PreviewPoolingManager m_PreviewPoolingManager;
    
    private GameManager m_GameManager = null;

    void Start()
    {
        m_GameManager = GameManager.instance_gm;

        for (int i = 0; i < PlayerMissile.Length; i++) {
            m_PlayerMissileName[i] = PlayerMissile[i].GetComponent<PlayerMissile>().m_ObjectName;
        }
        
        m_ShotDamage = m_GameManager.m_CurrentAttributes.m_ShotDamage; // 샷 데미지
        m_LaserDamage = m_GameManager.m_CurrentAttributes.m_LaserDamage; // 레이저 데미지
        m_Module = m_GameManager.m_CurrentAttributes.m_Module; // 모듈 종류
        m_PlayerShotZ = 1;

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
            
            StartCoroutine(ModuleShot());
        }
    }

    void Update ()
    {
        if (Time.timeScale == 0)
            return;

        if (Input.GetButton("Fire1")) {
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

        if (m_AutoShot > 0) {
            if (!m_NowShooting) {
                m_NowShooting = true;
                StartCoroutine(Shot());
            }
            m_NowAttacking = true;
        }
    }
    
    void OnEnable()
    {
        m_PlayerController.m_SlowMode = false;
        m_NowShooting = false;
        m_NowAttacking = false;
        m_AutoShot = 0;
        m_ShotKeyPressTime = 0f;
        //m_PlayerLaserShooter.StopLaser();
        StartCoroutine(ModuleShot());
    }

    protected override void CheckNowShooting() { // Now Attacking : 현재 공격 중 (모듈 공격을 위한 변수)
        if (m_AutoShot == 0) { // Now Shooting : 현재 샷 중 (샷 딜레이를 위한 변수)
            if (!m_NowShooting)
                m_NowAttacking = false;
        }
    }

    protected override void CreatePlayerAttacks(string name, Vector3 pos, Quaternion rot, byte type = 0) {
        GameObject obj = m_PreviewPoolingManager.PopFromPool(name);
        PlayerMissile playerMissile = obj.GetComponent<PlayerMissile>();
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        playerMissile.m_DamageLevel = type;
        obj.SetActive(true);
    }

    private IEnumerator PreviewSlowMode() {
        while(true) {
            m_PlayerController.m_SlowMode ^= true;
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
}
