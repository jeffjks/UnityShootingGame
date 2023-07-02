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

    private PlayerUnit _playerUnit;
    private Vector3 _currentTargetLocalP; // 현재 위치 타겟
    private Vector3 _currentLocalP; // 현재 위치
    private float _currentTargetLocalR; // 현재 회전 타겟
    private float _currentLocalR; // 현재 회전
    private int _shotIndex;
    private byte _shockWaveNumber;
    private float _defaultDepth;

    void Awake()
    {
        _playerUnit = GetComponentInParent<PlayerUnit>();
    }

    void Start()
    {
        _shotIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.ShotIndex);
        _defaultDepth = transform.localPosition.y;

        var laser_damage = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);

        if (laser_damage == 0)
            _shockWaveNumber = 0;
        else
            _shockWaveNumber = 1;

        m_ParticleSystem[_shockWaveNumber].gameObject.SetActive(true);
    }

    private void Update()
    {
        DisplayParticles();
        _currentLocalP = Vector3.MoveTowards(_currentLocalP, _currentTargetLocalP, 12f / Application.targetFrameRate * Time.timeScale);
        _currentLocalR = Mathf.MoveTowards(_currentLocalR, _currentTargetLocalR, 12f / Application.targetFrameRate * Time.timeScale);
        transform.localPosition = _currentLocalP;
        transform.localRotation = Quaternion.Euler(0f, _currentLocalR, 0f);
    }

    private void DisplayParticles() {
        if (!_playerUnit.SlowMode) { // 샷 모드
            _currentTargetLocalP = m_InitialLocalP[_shotIndex];
            _currentTargetLocalR = m_InitialLocalR[_shotIndex];
            if (m_ParticleSystem[_shockWaveNumber].isPlaying)
                m_ParticleSystem[_shockWaveNumber].Stop();
        }
        else { // 레이저 모드
            _currentTargetLocalP = m_TargetLocalP;
            _currentTargetLocalR = m_TargetLocalR;
            if (!m_ParticleSystem[_shockWaveNumber].isPlaying)
                m_ParticleSystem[_shockWaveNumber].Play();
        }
    }

    private void SetShotLevel(int level) {
        float coefficient = 0.3f;
        m_ParticleSystem[0].transform.localScale = new Vector3(1f + level*coefficient, 1f + level*coefficient, 1f + level*coefficient);
        m_ParticleSystem[0].transform.localPosition = new Vector3(0f, _defaultDepth, 2.5f + level*0.12f);
        m_ParticleSystem[1].transform.localScale = new Vector3(1f + level*coefficient, 1f + level*coefficient, 1f + level*coefficient);
        m_ParticleSystem[1].transform.localPosition = new Vector3(0f, _defaultDepth, 2.5f + level*0.12f);
    }

    public void SetPreviewDrones() {
        _shotIndex = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.ShotIndex);

        var laser_damage = PlayerManager.CurrentAttributes.GetAttributes(AttributeType.LaserIndex);
        
        SetShotLevel(_playerUnit.PlayerAttackLevel);

        if (laser_damage == 0)
            _shockWaveNumber = 0;
        else
            _shockWaveNumber = 1;

        m_ParticleSystem[_shockWaveNumber].gameObject.SetActive(true);
        m_ParticleSystem[1 - _shockWaveNumber].gameObject.SetActive(false);
    }

    public float GetCurrentLocalRotation() {
        return _currentLocalR;
    }
}
