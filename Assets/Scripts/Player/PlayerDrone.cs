using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    public PlayerDroneTransformDatas m_PlayerDroneTransformData;
    public GameObject m_ParticleObject;

    private PlayerUnit _playerUnit;
    private PlayerLaserHandler _playerLaserHandler;
    private PlayerShootHandler _playerShootHandler;
    private ParticleSystem[] _particleSystems;
    private Vector3 _currentTargetLocalP; // 현재 위치 타겟
    private Vector3 _currentLocalP; // 현재 위치
    private Vector3 _currentTargetLocalR; // 현재 회전 타겟
    private Vector3 _currentLocalR; // 현재 회전
    private int _shotIndex;
    private byte _shockWaveNumber;
    private float _defaultDepth;
    private float _defaultParticlePositionZ;

    void Awake()
    {
        _playerUnit = GetComponentInParent<PlayerUnit>();
        _playerLaserHandler = _playerUnit.GetComponentInChildren<PlayerLaserHandler>();
        _playerShootHandler = _playerUnit.GetComponentInChildren<PlayerShootHandler>();
        _particleSystems = m_ParticleObject.GetComponentsInChildren<ParticleSystem>(true);
        _playerUnit.Action_OnUpdatePlayerAttackLevel += SetPreviewDrones;
        _playerLaserHandler.Action_OnLaserIndexChanged += SetPreviewDrones;
        _defaultParticlePositionZ = m_ParticleObject.transform.position.z;
    }

    void Start()
    {
        _defaultDepth = transform.localPosition.y;
        
        SetPreviewDrones();
    }

    private void Update()
    {
        DisplayParticles();
        _currentLocalP = Vector3.MoveTowards(_currentLocalP, _currentTargetLocalP, 12f / Application.targetFrameRate * Time.timeScale);
        _currentLocalR = Vector3.MoveTowards(_currentLocalR, _currentTargetLocalR, 12f / Application.targetFrameRate * Time.timeScale);
        transform.localPosition = _currentLocalP;
        transform.localRotation = Quaternion.Euler(_currentLocalR);
    }

    private void DisplayParticles() {
        if (!_playerUnit.SlowMode) { // 샷 모드
            _currentTargetLocalP = m_PlayerDroneTransformData.shotTransformData[_shotIndex].positionData;
            _currentTargetLocalR = m_PlayerDroneTransformData.shotTransformData[_shotIndex].rotationData;
            m_ParticleObject.SetActive(false);
            // if (_particleSystems[_shockWaveNumber].isPlaying)
            //     _particleSystems[_shockWaveNumber].Stop();
        }
        else { // 레이저 모드
            _currentTargetLocalP = m_PlayerDroneTransformData.laserTransformData[_shotIndex].positionData;
            _currentTargetLocalR = m_PlayerDroneTransformData.laserTransformData[_shotIndex].rotationData;
            m_ParticleObject.SetActive(true);
            // if (!_particleSystems[_shockWaveNumber].isPlaying)
            //     _particleSystems[_shockWaveNumber].Play();
        }
    }

    private void SetParticleScale(int level) {
        const float coefficient = 0.2f;
        _particleSystems[0].transform.localScale = new Vector3(1f + level*coefficient, 1f + level*coefficient, 1f + level*coefficient);
        _particleSystems[0].transform.localPosition = new Vector3(0f, _defaultDepth, _defaultParticlePositionZ + level*coefficient / 2f);
        _particleSystems[1].transform.localScale = new Vector3(1f + level*coefficient, 1f + level*coefficient, 1f + level*coefficient);
        _particleSystems[1].transform.localPosition = new Vector3(0f, _defaultDepth, _defaultParticlePositionZ + level*coefficient / 2f);
    }

    private void SetPreviewDrones()
    {
        _shotIndex = _playerShootHandler.ShotIndex;
        var laserIndex = _playerLaserHandler.LaserIndex;
        
        foreach (var particle in _particleSystems)
        {
            particle.gameObject.SetActive(false);
        }
        _particleSystems[laserIndex].gameObject.SetActive(true);
        
        //SetParticleScale(_playerUnit.PlayerAttackLevel);
    }

    public float GetCurrentLocalRotation() {
        return _currentLocalR.y;
    }
}
