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

    protected PlayerUnit m_PlayerMovement;
    protected Vector3 m_CurrentTargetLocalP; // 현재 위치 타겟
    protected Vector3 m_CurrentLocalP; // 현재 위치
    protected float m_CurrentTargetLocalR; // 현재 회전 타겟
    protected float m_CurrentLocalR; // 현재 회전
    protected int m_ShotForm;
    protected int m_ShotLevel;
    protected byte m_ShockWaveNumber;
    protected float m_DefaultDepth;

    private PlayerUnit _playerUnit;

    void Awake()
    {
        _playerUnit = GetComponentInParent<PlayerUnit>();
        m_PlayerMovement = GetComponentInParent<PlayerUnit>();
    }

    void Start()
    {
        m_ShotForm = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);
        m_DefaultDepth = transform.localPosition.y;

        var laser_damage = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);

        if (laser_damage == 0)
            m_ShockWaveNumber = 0;
        else
            m_ShockWaveNumber = 1;

        m_ParticleSystem[m_ShockWaveNumber].gameObject.SetActive(true);
    }

    public void PreviewStart()
    {
        //((PlayerPreviewShooter) m_PlayerController).InitShotLevel();
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
        if (!m_PlayerMovement.SlowMode) { // 샷 모드
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
        m_ShotForm = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.ShotIndex);

        laser_damage = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);
        
        SetShotLevel(_playerUnit.PlayerAttackLevel);

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
