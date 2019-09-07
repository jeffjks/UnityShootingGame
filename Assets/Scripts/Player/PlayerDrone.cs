using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    public GameObject[] m_Particle = new GameObject[2];
    [Header("샷 모드 위치, 회전")]
    public Vector3[] m_InitialLocalP = new Vector3[3]; // 샷 모드 위치
    public float[] m_InitialLocalR = new float[3]; // 샷 모드 회전
    [Header("레이저 모드 위치, 회전")]
    public Vector3 m_TargetLocalP; // 레이저 모드 위치
    public float m_TargetLocalR; // 레이저 모드 회전

    private PlayerController m_PlayerController;
    private Vector3 m_CurrentTargetLocalP; // 현재 위치 타겟
    private Vector3 m_CurrentLocalP; // 현재 위치
    private float m_CurrentTargetLocalR; // 현재 회전 타겟
    private float m_CurrentLocalR; // 현재 회전
    private int m_ShotForm;
    private int m_ShotLevel;
    private float m_ParticleLocalScale;
    private float m_DefaultDepth;
    
    private PlayerManager m_PlayerManager = null;
    private ParticleSystem m_ParticleSystem;
    
    void Start()
    {
        m_PlayerController = GetComponentInParent<PlayerController>();
        m_PlayerManager = PlayerManager.instance_pm;
        m_ShotForm = m_PlayerManager.m_CurrentAttributes[2];
        m_DefaultDepth = transform.localPosition.y;

        int laser_damage = m_PlayerManager.m_CurrentAttributes[4];
        if (laser_damage == 0) {
            m_Particle[0].SetActive(true);
            m_ParticleSystem = m_Particle[0].GetComponentInChildren<ParticleSystem>();
            m_ParticleLocalScale = m_Particle[0].transform.localScale[0];
        }
        else {
            m_Particle[1].SetActive(true);
            m_ParticleSystem = m_Particle[1].GetComponentInChildren<ParticleSystem>();
            m_ParticleLocalScale = m_Particle[1].transform.localScale[0];
        }
    }

    void Update()
    {
        if (!m_PlayerController.m_SlowMode) { // 샷 모드
            m_CurrentTargetLocalP = m_InitialLocalP[m_ShotForm];
            m_CurrentTargetLocalR = m_InitialLocalR[m_ShotForm];
            if (m_ParticleSystem.isPlaying)
                m_ParticleSystem.Stop();
        }
        else { // 레이저 모드
            m_CurrentTargetLocalP = m_TargetLocalP;
            m_CurrentTargetLocalR = m_TargetLocalR;
            if (!m_ParticleSystem.isPlaying)
                m_ParticleSystem.Play();
        }
        
        m_CurrentLocalP = Vector3.MoveTowards(m_CurrentLocalP, m_CurrentTargetLocalP, 0.2f);
        m_CurrentLocalR = Mathf.MoveTowards(m_CurrentLocalR, m_CurrentTargetLocalR, 0.2f);
        transform.localPosition = m_CurrentLocalP;
        transform.localRotation = Quaternion.Euler(0f, m_CurrentLocalR, 0f);
    }

    public void SetShotLevel(int level) {
        m_ShotLevel = level;
        m_Particle[0].transform.localScale = new Vector3(m_ParticleLocalScale + level*4, m_ParticleLocalScale + level*4, m_ParticleLocalScale + level*4);
        m_Particle[0].transform.localPosition = new Vector3(0f, m_DefaultDepth, 2.5f + level*0.12f);
        m_Particle[1].transform.localScale = new Vector3(m_ParticleLocalScale + level*4, m_ParticleLocalScale + level*4, m_ParticleLocalScale + level*4);
        m_Particle[1].transform.localPosition = new Vector3(0f, m_DefaultDepth, 2.5f + level*0.12f);
    }
}
