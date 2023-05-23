using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewShooter : PlayerShooterManager
{
    private PlayerManager m_PlayerManager = null;
    private PoolingManager m_PoolingManager = null;

    private const int DEFAULT_SHOT_LEVEL = 4;

    void Awake()
    {
        m_NowAttacking = true;
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        InitShotLevel();

        for (int i = 0; i < PlayerWeapon.Length; i++) {
            m_PlayerWeaponName[i] = PlayerWeapon[i].GetComponent<PlayerWeapon>().m_ObjectName;
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
        while (!m_PlayerUnit.m_SlowMode) {
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
                yield return new WaitForMillisecondFrames(m_FireRate);
            }
            yield return new WaitForMillisecondFrames(m_FireDelayWait);
        }
        yield break;
    }

    protected override void CreatePlayerAttacks(string name, Vector3 pos, float dir, byte type = 0) {
        GameObject obj = m_PoolingManager.PopFromPool(name, PoolingParent.PLAYER_MISSILE);
        PlayerWeapon playerWeapon = obj.GetComponent<PlayerWeapon>();
        obj.transform.position = pos;
        playerWeapon.m_MoveVector.direction = dir;
        playerWeapon.m_DamageLevel = type;
        obj.SetActive(true);
        playerWeapon.OnStart();
    }

    private IEnumerator PreviewSlowMode() {
        while(true) {
            DisableSlowMode();
            yield return new WaitForMillisecondFrames(Random.Range(1000, 3000));
            EnableSlowMode();
            yield return new WaitForMillisecondFrames(Random.Range(1000, 3000));
        }
    }

    private void EnableSlowMode() {
        m_PlayerUnit.m_SlowMode = true;
        m_PlayerLaserShooter.StartLaser();
    }

    private void DisableSlowMode() {
        m_PlayerUnit.m_SlowMode = false;
        m_PlayerLaserShooter.StopLaser();
        StartCoroutine(Shot());
    }

    public void InitShotLevel() {
        m_ShotLevel = DEFAULT_SHOT_LEVEL;
    }


    public override void SetPreviewShooter() {
        m_ShotDamage = m_PlayerManager.m_CurrentAttributes.m_ShotLevel; // 샷 데미지
        m_Module = m_PlayerManager.m_CurrentAttributes.m_Module; // 모듈 종류
        SetModule();
    }
}
