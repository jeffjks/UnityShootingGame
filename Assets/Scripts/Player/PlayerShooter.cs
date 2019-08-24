using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public PlayerController m_PlayerController;
    public GameObject[] PlayerMissile = new GameObject[4];
    public Transform[] m_PlayerShotPosition = new Transform[6];
    public PlayerDrone[] m_PlayerDrone = new PlayerDrone[4];
    public PlayerLaserShooter m_PlayerLaserShooter;
    public GameObject m_Bomb;

    [Header("ShotNumber회 m_FireRate초 간격으로 실행. 실행 주기는 m_FireDelay")]
    [SerializeField] private float m_FireRate = 0.05f;
    [SerializeField] private float m_FireDelay = 0.32f;
    [SerializeField] private float m_HomingMissileMaxDelay = 0.4f, m_RocketMaxDelay = 1.2f, m_AddShotMaxDelay = 0.2f;
    [SerializeField] private float m_HomingMissileMinDelay = 0.3f, m_RocketMinDelay = 0.6f, m_AddShotMinDelay = 0.2f;
    
    [HideInInspector] public int m_ShotLevel = 0;
    private string[] m_PlayerMissileName = new string[4];
    private AudioSource m_AudioSource;
    private Transform m_MainCamera;
    private float m_FireDelayWait;
    private int m_ShotNumber;
    private bool m_NowShooting, m_NowAttacking = false;
    private int m_AutoShot = 0;
    private float m_ShotKeyPressTime = 0f;
    private int m_ShotDamage, m_LaserDamage, m_Module;
    private int m_DefaultBombNumber, m_BombNumber, m_MaxBombNumber = 5;
    private bool m_BombEnable = true;
    private float m_CurrentModuleDelay, m_ModuleDelay, m_ModuleMinDelay, m_ModuleMaxDelay;
    private const sbyte m_PlayerShotZ = Depth.PLAYER_MISSILE;
    
    private PlayerManager m_PlayerManager = null;
    private PoolingManager m_PoolingManager = null;

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        m_AudioSource = GetComponent<AudioSource>();
        m_MainCamera = m_PlayerManager.m_MainCamera.transform;

        for (int i=0; i<PlayerMissile.Length; i++) {
            m_PlayerMissileName[i] = PlayerMissile[i].GetComponent<PlayerMissile>().m_ObjectName;
        }
        
        m_ShotDamage = m_PlayerManager.m_CurrentAttributes[3]; // 샷 데미지
        m_LaserDamage = m_PlayerManager.m_CurrentAttributes[4]; // 레이저 데미지
        m_Module = m_PlayerManager.m_CurrentAttributes[5]; // 모듈 종류
        
        if (m_PlayerManager.m_CurrentAttributes[6] == 0) // 폭탄 개수
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
                            m_PlayerController.EnableInvincible(4f);
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
        UpdateShotNumber();
        if (m_PlayerManager != null) {
            StartCoroutine(ModuleShot());
        }
    }
    

    private IEnumerator Shot() {
        m_AutoShot--;
        for (int i=0; i<m_ShotNumber; i++) { // m_FireRate초 간격으로 ShotNumber회 실행. 실행 주기는 m_FireDelay
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

    private void CheckNowShooting() { // Now Attacking : 현재 공격 중 (모듈 공격을 위한 변수)
        if (m_AutoShot == 0) { // Now Shooting : 현재 샷 중 (샷 딜레이를 위한 변수)
            if (!m_PlayerController.m_SlowMode && !m_NowShooting)
                m_NowAttacking = false;
        }
    }

    private IEnumerator ModuleShot() {
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

    private void CreateShotNormal(int level) {
        Vector3[] shotPosition = new Vector3[5];
        Quaternion[] shotDirection = new Quaternion[5];
        m_AudioSource.Play();

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
    
    private void CreateShotStrong(int level) {
        Vector3[] shotPosition = new Vector3[5];
        Quaternion[] shotDirection = new Quaternion[5];
        m_AudioSource.Play();

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

    private void CreateShotVeryStrong(int level) {
        Vector3[] shotPosition = new Vector3[5];
        Quaternion[] shotDirection = new Quaternion[5];
        m_AudioSource.Play();

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

    private void CreateHomingMissile() {
        Vector3[] shotPosition = new Vector3[2];
        shotPosition[0] = m_PlayerShotPosition[5].position;
        shotPosition[1] = m_PlayerShotPosition[6].position;
        CreatePlayerAttacks(m_PlayerMissileName[1], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 0f));
        CreatePlayerAttacks(m_PlayerMissileName[1], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 0f));
    }

    private void CreateRocket() {
        Vector3[] shotPosition = new Vector3[2];
        shotPosition[0] = m_PlayerShotPosition[5].position;
        shotPosition[1] = m_PlayerShotPosition[6].position;
        CreatePlayerAttacks(m_PlayerMissileName[2], new Vector3(shotPosition[0][0], shotPosition[0][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 0f));
        CreatePlayerAttacks(m_PlayerMissileName[2], new Vector3(shotPosition[1][0], shotPosition[1][1], m_PlayerShotZ), Quaternion.Euler(0f, 0f, 0f));
    }

    private void CreateAddShot(int level) {
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

    private void CreatePlayerAttacks(string name, Vector3 pos, Quaternion rot, byte type = 0) {
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


    public void PowerUp() {
        if (m_ShotLevel < 4) {
            m_ShotLevel++;
            ResetLaser();
        }
        else {
            // 점수 +
        }
        for (int i=0; i<4; i++)
            m_PlayerDrone[i].SetShotLevel(m_ShotLevel);
        UpdateShotNumber();
    }
    
    public void PowerDown() {
        if (m_ShotLevel > 0) {
            m_ShotLevel--;
            ResetLaser();
        }
        for (int i=0; i<4; i++)
            m_PlayerDrone[i].SetShotLevel(m_ShotLevel);
        UpdateShotNumber();
    }

    private void ResetLaser() {
        if (m_PlayerController.m_SlowMode) {
            m_PlayerLaserShooter.StopLaser();
            m_PlayerLaserShooter.StartLaser();
        }
    }

    private void UpdateShotNumber() {

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

    public void AddBomb() {
        if (m_BombNumber < m_MaxBombNumber) {
            m_BombNumber++;
        }
        else {
            // 점수 +
        }
    }
}
