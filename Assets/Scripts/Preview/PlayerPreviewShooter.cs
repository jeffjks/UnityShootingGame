using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewShooter : PlayerShooterManager
{
    public PreviewPoolingManager m_PreviewPoolingManager;
    
    private GameManager m_GameManager = null;
    private const int m_DefaultShotLevel = 4;

    void Awake()
    {
        m_NowAttacking = true;
        m_GameManager = GameManager.instance_gm;
        InitShotLevel();

        for (int i = 0; i < PlayerMissile.Length; i++) {
            m_PlayerMissileName[i] = PlayerMissile[i].GetComponent<PlayerMissile>().m_ObjectName;
        }
        
        SetPreviewShooter();
        m_PlayerShotZ = 1;
    }

    void OnEnable() {   
        StartCoroutine(ModuleShot());
        StartCoroutine(PreviewSlowMode());
        UpdateShotNumber();
    }

    protected override IEnumerator Shot() {
        while (!m_PlayerController.m_SlowMode) {
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
                yield return new WaitForSeconds(m_FireRate);
            }
            yield return new WaitForSeconds(m_FireDelayWait); // m_FireDelay에서 m_FireRate가 차지하는 부분 빼기
        }
        yield break;
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
            DisableSlowMode();
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            EnableSlowMode();
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private void EnableSlowMode() {
        m_PlayerController.m_SlowMode = true;
        m_PlayerLaserShooter.StartLaser();
    }

    private void DisableSlowMode() {
        m_PlayerController.m_SlowMode = false;
        m_PlayerLaserShooter.StopLaser();
        StartCoroutine(Shot());
    }

    public void InitShotLevel() {
        m_ShotLevel = m_DefaultShotLevel;
    }


    public override void SetPreviewShooter() {
        m_ShotDamage = m_GameManager.m_CurrentAttributes.m_ShotDamage; // 샷 데미지
        m_Module = m_GameManager.m_CurrentAttributes.m_Module; // 모듈 종류
        SetModule();
    }
}
