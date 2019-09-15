using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerShooterManager : MonoBehaviour
{
    public GameObject[] PlayerMissile = new GameObject[4];
    public Transform[] m_PlayerShotPosition = new Transform[7];
    public PlayerDrone[] m_PlayerDrone = new PlayerDrone[4];
    public PlayerLaserShooterManager m_PlayerLaserShooter;
    public PlayerControllerManager m_PlayerController;

    [Header("ShotNumber회 m_FireRate초 간격으로 실행. 실행 주기는 m_FireDelay")]
    [SerializeField] protected float m_FireRate = 0.05f;
    [SerializeField] protected float m_FireDelay = 0.32f;
    [SerializeField] protected float m_HomingMissileMaxDelay = 0.4f, m_RocketMaxDelay = 1.2f, m_AddShotMaxDelay = 0.2f;
    [SerializeField] protected float m_HomingMissileMinDelay = 0.3f, m_RocketMinDelay = 0.6f, m_AddShotMinDelay = 0.2f;
    
    [HideInInspector] public int m_ShotLevel = 0;
    protected string[] m_PlayerMissileName = new string[4];
    protected float m_FireDelayWait;
    protected int m_ShotNumber;
    protected bool m_NowShooting, m_NowAttacking = false;
    protected int m_AutoShot;
    protected float m_ShotKeyPressTime;
    protected int m_ShotDamage, m_LaserDamage, m_Module;
    protected float m_CurrentModuleDelay, m_ModuleDelay, m_ModuleMinDelay, m_ModuleMaxDelay;
    protected sbyte m_PlayerShotZ;
    protected abstract void CreatePlayerAttacks(string name, Vector3 pos, Quaternion rot, byte type = 0);
    protected abstract void CheckNowShooting();

    protected IEnumerator Shot() {
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
            yield return new WaitForSeconds(m_FireRate);
        }
        yield return new WaitForSeconds(m_FireDelayWait); // m_FireDelay에서 m_FireRate가 차지하는 부분 빼기
        m_NowShooting = false;
        CheckNowShooting();
        yield break;
    }

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
                    CreateAddShot(m_ShotLevel);
                }
                yield return new WaitForSeconds(m_CurrentModuleDelay);
            }
            yield return null;
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
        shotPosition[0] = m_PlayerShotPosition[5].position;
        shotPosition[1] = m_PlayerShotPosition[6].position;
        CreatePlayerAttacks(m_PlayerMissileName[2], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 0f));
        CreatePlayerAttacks(m_PlayerMissileName[2], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 0f));
    }

    protected void CreateAddShot(int level) {
        Vector3[] shotPosition = new Vector3[2];
        float rot = transform.rotation.eulerAngles[1];
        shotPosition[0] = m_PlayerShotPosition[5].position;
        shotPosition[1] = m_PlayerShotPosition[6].position;
        if (level <= 1) {
            CreatePlayerAttacks(m_PlayerMissileName[3], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, rot), 0);
            CreatePlayerAttacks(m_PlayerMissileName[3], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, rot), 0);
        }
        else if (level <= 3) {
            CreatePlayerAttacks(m_PlayerMissileName[3], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, rot), 1);
            CreatePlayerAttacks(m_PlayerMissileName[3], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, rot), 1);
        }
        else {
            CreatePlayerAttacks(m_PlayerMissileName[3], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, rot), 2);
            CreatePlayerAttacks(m_PlayerMissileName[3], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, rot), 2);
        }
    }
}
