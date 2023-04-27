using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    public ParticleSystem[] m_ParticleSystem = new ParticleSystem[2];
    [Header("샷 모드 위치, 회전")]
    public Vector3[] m_InitialLocalP = new Vector3[3]; // 샷 모드 위치
    public float[] m_InitialLocalR = new float[3]; // 샷 모드 회전
    [Header("레이저 모드 위치, 회전")]
    public Vector3 m_TargetLocalP; // 레이저 모드 위치
    public float m_TargetLocalR; // 레이저 모드 회전

    protected PlayerUnit m_PlayerController;
    protected Vector3 m_CurrentTargetLocalP; // 현재 위치 타겟
    protected Vector3 m_CurrentLocalP; // 현재 위치
    protected float m_CurrentTargetLocalR; // 현재 회전 타겟
    protected float m_CurrentLocalR; // 현재 회전
    protected int m_ShotForm;
    protected int m_ShotLevel;
    protected byte m_ShockWaveNumber;
    protected float m_DefaultDepth;

    private PlayerShooterManager m_PlayerShooter;
    
    private PlayerManager m_PlayerManager = null;

    void Awake()
    {
        m_PlayerManager = PlayerManager.instance_pm;

        m_PlayerShooter = GetComponentInParent<PlayerShooterManager>();
        m_PlayerController = GetComponentInParent<PlayerUnit>();
    }

    void Start()
    {
        int laser_damage;
        if (m_PlayerManager != null) {
            m_ShotForm = m_PlayerManager.m_CurrentAttributes.m_ShotDamage;
        }
        m_DefaultDepth = transform.localPosition.y;

        if (m_PlayerManager == null)
            laser_damage = 0;
        else
            laser_damage = m_PlayerManager.m_CurrentAttributes.m_LaserDamage;

        if (laser_damage == 0)
            m_ShockWaveNumber = 0;
        else
            m_ShockWaveNumber = 1;

        m_ParticleSystem[m_ShockWaveNumber].gameObject.SetActive(true);
    }

    public void PreviewStart()
    {
        ((PlayerPreviewShooter) m_PlayerShooter).InitShotLevel();
        m_DefaultDepth = transform.localPosition.y;
        SetPreviewDrones();
    }

    void Update()
    {
        DisplayParticles();
        m_CurrentLocalP = Vector3.MoveTowards(m_CurrentLocalP, m_CurrentTargetLocalP, 12f / Application.targetFrameRate * Time.timeScale);
        m_CurrentLocalR = Mathf.MoveTowards(m_CurrentLocalR, m_CurrentTargetLocalR, 12f / Application.targetFrameRate * Time.timeScale);
        transform.localPosition = m_CurrentLocalP;
        transform.localRotation = Quaternion.Euler(0f, m_CurrentLocalR, 0f);
    }

    private void DisplayParticles() {
        if (!m_PlayerController.m_SlowMode) { // 샷 모드
            m_CurrentTargetLocalP = m_InitialLocalP[m_ShotForm];
            m_CurrentTargetLocalR = m_InitialLocalR[m_ShotForm];
            if (m_ParticleSystem[m_ShockWaveNumber].isPlaying)
                m_ParticleSystem[m_ShockWaveNumber].Stop();
        }
        else { // 레이저 모드
            m_CurrentTargetLocalP = m_TargetLocalP;
            m_CurrentTargetLocalR = m_TargetLocalR;
            if (!m_ParticleSystem[m_ShockWaveNumber].isPlaying)
                m_ParticleSystem[m_ShockWaveNumber].Play();
        }
    }

    public void SetShotLevel(int level) {
        float coefficient = 0.3f;
        m_ShotLevel = level;
        m_ParticleSystem[0].transform.localScale = new Vector3(1f + level*coefficient, 1f + level*coefficient, 1f + level*coefficient);
        m_ParticleSystem[0].transform.localPosition = new Vector3(0f, m_DefaultDepth, 2.5f + level*0.12f);
        m_ParticleSystem[1].transform.localScale = new Vector3(1f + level*coefficient, 1f + level*coefficient, 1f + level*coefficient);
        m_ParticleSystem[1].transform.localPosition = new Vector3(0f, m_DefaultDepth, 2.5f + level*0.12f);
    }

    public void SetPreviewDrones() {
        int laser_damage;
        m_ShotForm = m_PlayerManager.m_CurrentAttributes.m_ShotDamage;

        laser_damage = m_PlayerManager.m_CurrentAttributes.m_LaserDamage;
        
        SetShotLevel(m_PlayerShooter.m_ShotLevel);

        if (laser_damage == 0)
            m_ShockWaveNumber = 0;
        else
            m_ShockWaveNumber = 1;

        m_ParticleSystem[m_ShockWaveNumber].gameObject.SetActive(true);
        m_ParticleSystem[1 - m_ShockWaveNumber].gameObject.SetActive(false);
    }

    public float GetCurrentLocalRotation() {
        return m_CurrentLocalR;
    }
}
