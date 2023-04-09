using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ModuleDelay {
    public int max_delay;
    public int min_delay;

    public ModuleDelay(int max_delay, int min_delay) {
        this.max_delay = max_delay;
        this.min_delay = min_delay;
    }
}

public abstract class PlayerShooterManager : MonoBehaviour
{
    public GameObject[] PlayerMissile = new GameObject[4];
    public Transform[] m_PlayerShotPosition = new Transform[7];
    public PlayerDrone[] m_PlayerDrone = new PlayerDrone[4];
    public PlayerLaserShooterManager m_PlayerLaserShooter;
    public PlayerControllerManager m_PlayerController;

    [Header("ShotNumber회 m_FireRate초 간격으로 실행. 실행 주기는 m_FireDelay (ms)")]
    [SerializeField] protected int m_FireRate; // 50
    [SerializeField] protected int m_FireDelay; // 320
    [SerializeField] protected ModuleDelay m_HomingMissileDelay; // 400, 300
    [SerializeField] protected ModuleDelay m_RocketDelay; // 1000, 1000
    [SerializeField] protected ModuleDelay m_AddShotDelay; // 200, 100
    
    [HideInInspector] public int m_ShotLevel; // 0 ~ 4
    protected string[] m_PlayerMissileName = new string[4];
    protected int m_FireDelayWait; // m_FireDelay에서 m_FireRate가 차지하는 부분 뺀 딜레이
    protected int m_ShotNumber, m_ShotDamage, m_Module;
    protected bool m_NowAttacking;
    protected int m_CurrentModuleDelay, m_ModuleDelayGap;
    protected ModuleDelay m_ModuleDelay;
    protected sbyte m_PlayerShotZ;
    protected abstract IEnumerator Shot();
    protected abstract void CreatePlayerAttacks(string name, Vector3 pos, Quaternion rot, byte type = 0);
    public abstract void SetPreviewShooter();

    private int[] m_ShotLevelToType = { 0, 0, 1, 1, 2 };

    protected IEnumerator ModuleShot() {
        while(true) {
            if (m_NowAttacking) {
                if (m_Module == 1) {
                    CreateHomingMissile();
                }
                else if (m_Module == 2) {
                    CreateRocket();
                }
                else if (m_Module == 3) {
                    CreateAddShot();
                }
                yield return new WaitForMillisecondFrames(m_CurrentModuleDelay);
            }
            yield return new WaitForFrames(0);
        }
    }

    protected void CreateShotNormal(int level) {
        Vector3[] shotPosition = new Vector3[5];
        Quaternion[] shotDirection = new Quaternion[5];

        for (int i = 0; i < 5; i++) {
            shotDirection[i] = m_PlayerShotPosition[i].rotation * Quaternion.Euler(90f, 0f, 0f);
            shotPosition[i] = new Vector3(m_PlayerShotPosition[i].position[0], m_PlayerShotPosition[i].position[1], m_PlayerShotZ);
        }

        if (level <= 1) { // ----------------------------------[ 0 0 0 0 0 ]
            for (int i = 0; i < 5; i++) {
                CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[i], shotDirection[i], 0);
            }
        }
        else if (level <= 3) { // ---------------------------------- [ 0 1 0 1 0 ]
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[0], shotDirection[0], 0);
            for (int i = 1; i < 3; i++) {
                CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[i], shotDirection[i], 1);
                CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[i+2], shotDirection[i+2], 0);
            }
        }
        else { // ---------------------------------- [ 0 1 2 1 0 ]
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[0], shotDirection[0], 2);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[1], shotDirection[1], 1);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[2], shotDirection[2], 1);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[3], shotDirection[3], 0);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[4], shotDirection[4], 0);
        }
    }
    
    protected void CreateShotStrong(int level) {
        Vector3[] shotPosition = new Vector3[5];
        Quaternion[] shotDirection = new Quaternion[5];

        for (int i = 0; i < 5; i++) {
            shotDirection[i] = m_PlayerShotPosition[i].rotation * Quaternion.Euler(90f, 0f, 0f);
            shotPosition[i] = new Vector3(m_PlayerShotPosition[i].position[0], m_PlayerShotPosition[i].position[1], m_PlayerShotZ);
        }
        if (level <= 1) { // ---------------------------------- [ 0 1 0 1 0 ]
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[0], shotDirection[0], 0);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[1], shotDirection[1], 1);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[2], shotDirection[2], 1);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[3], shotDirection[3], 0);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[4], shotDirection[4], 0);
        }
        else if (level <= 3) { // ---------------------------------- [ 0 1 2 1 0 ]
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[0], shotDirection[0], 2);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[1], shotDirection[1], 1);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[2], shotDirection[2], 1);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[3], shotDirection[3], 0);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[4], shotDirection[4], 0);
        }
        else { // ---------------------------------- [ 1 1 2 1 1 ]
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[0], shotDirection[0], 2);
            for (int i = 1; i < 5; i++) {
                CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[i], shotDirection[i], 1);
            }
        }
    }

    protected void CreateShotVeryStrong(int level) {
        Vector3[] shotPosition = new Vector3[5];
        Quaternion[] shotDirection = new Quaternion[5];

        for (int i = 0; i < 5; i++) {
            shotDirection[i] = m_PlayerShotPosition[i].rotation * Quaternion.Euler(90f, 0f, 0f);
            shotPosition[i] = new Vector3(m_PlayerShotPosition[i].position[0], m_PlayerShotPosition[i].position[1], m_PlayerShotZ);
        }

        if (level <= 1) { // ---------------------------------- [ 0 1 1 1 0 ]
            for (int i = 0; i < 3; i++) {
                CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[i], shotDirection[i], 1);
            }
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[3], shotDirection[3], 0);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[4], shotDirection[4], 0);
        }
        else if (level <= 3) { // ---------------------------------- [ 1 1 2 1 1 ]
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[0], shotDirection[0], 2);
            for (int i = 1; i < 5; i++) {
                CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[i], shotDirection[i], 1);
            }
        }
        else { // ---------------------------------- [ 1 2 2 2 1 ]
            for (int i = 0; i < 3; i++) {
                CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[i], shotDirection[i], 2);
            }
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[3], shotDirection[3], 1);
            CreatePlayerAttacks(m_PlayerMissileName[0], shotPosition[4], shotDirection[4], 1);
        }
    }

    protected void CreateHomingMissile() {
        Vector3[] shotPosition = new Vector3[2];
        shotPosition[0] = m_PlayerShotPosition[5].position;
        shotPosition[1] = m_PlayerShotPosition[6].position;
        CreatePlayerAttacks(m_PlayerMissileName[1], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, -15f));
        CreatePlayerAttacks(m_PlayerMissileName[1], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 15f));
    }

    protected void CreateRocket() {
        Vector3[] shotPosition = new Vector3[2];
        byte damage_level = (byte) m_ShotLevelToType[m_ShotLevel];
        shotPosition[0] = m_PlayerShotPosition[5].position;
        shotPosition[1] = m_PlayerShotPosition[6].position;
        CreatePlayerAttacks(m_PlayerMissileName[2], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 0f), damage_level);
        CreatePlayerAttacks(m_PlayerMissileName[2], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 0f), damage_level);
    }

    protected void CreateAddShot() {
        Vector3[] shotPosition = new Vector3[2];
        byte damage_level = (byte) m_ShotLevelToType[m_ShotLevel];
        float rot = m_PlayerController.m_PlayerBody.eulerAngles.y;
        shotPosition[0] = m_PlayerShotPosition[5].position;
        shotPosition[1] = m_PlayerShotPosition[6].position;
        CreatePlayerAttacks(m_PlayerMissileName[3], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, rot), damage_level);
        CreatePlayerAttacks(m_PlayerMissileName[3], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, rot), damage_level);
    }

    protected void UpdateShotNumber() {
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

        SetModuleDelay();
        m_FireDelayWait = m_FireDelay - m_FireRate*m_ShotNumber;
    }

    protected void SetModule() {
        if (m_Module == 1) {
            m_ModuleDelay = m_HomingMissileDelay;
        }
        else if (m_Module == 2) {
            m_ModuleDelay = m_RocketDelay;
        }
        else if (m_Module == 3) {
            m_ModuleDelay = m_AddShotDelay;
        }
        m_ModuleDelayGap = (m_ModuleDelay.max_delay - m_ModuleDelay.min_delay) / 4;
        SetModuleDelay();
    }

    private void SetModuleDelay() {
        m_CurrentModuleDelay = m_ModuleDelay.max_delay - m_ModuleDelayGap*m_ShotLevel;
    }

    protected void ResetLaser() {
        if (m_PlayerController.m_SlowMode) {
            m_PlayerLaserShooter.StopLaser();
            m_PlayerLaserShooter.StartLaser();
        }
    }
}
