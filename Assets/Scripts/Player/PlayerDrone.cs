using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    public PlayerDroneTransformDatas m_PlayerDroneTransformData;
    public GameObject m_ParticleObject;
    
    private PlayerUnit _playerUnit;
    private PlayerLaserHandler _playerLaserHandler;
    private PlayerShotHandler _playerShotHandler;
    private ParticleSystem[] _particleSystems;
    private Vector3 _currentTargetLocalP; // 현재 위치 타겟
    private Vector3 _currentLocalP; // 현재 위치
    private Quaternion _currentTargetLocalR; // 현재 회전 타겟
    private Quaternion _currentLocalR; // 현재 회전
    private int _shotIndex;
    private int _laserIndex;
    private byte _shockWaveNumber;
    private float _defaultDepth;
    private float _defaultParticlePositionZ;

    void Awake()
    {
        _playerUnit = GetComponentInParent<PlayerUnit>();
        _playerLaserHandler = _playerUnit.GetComponentInChildren<PlayerLaserHandler>();
        _playerShotHandler = _playerUnit.GetComponentInChildren<PlayerShotHandler>();
        _particleSystems = m_ParticleObject.GetComponentsInChildren<ParticleSystem>(true);
        _playerUnit.Action_OnUpdatePlayerAttackLevel += SetPreviewDrones;
        _playerLaserHandler.Action_OnLaserIndexChanged += SetPreviewDrones;
        _playerShotHandler.Action_OnShotIndexChanged += SetPreviewDrones;
        _defaultParticlePositionZ = m_ParticleObject.transform.position.z;
    }

    void Start()
    {
        _defaultDepth = transform.localPosition.y;
        _currentLocalP = transform.localPosition;
        _currentLocalR = transform.localRotation;
        
        SetPreviewDrones();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        SetDroneMode();
        var maxDistanceDelta = 12f / Application.targetFrameRate * Time.timeScale;
        _currentLocalP = Vector3.MoveTowards(_currentLocalP, _currentTargetLocalP, maxDistanceDelta);
        _currentLocalR = Quaternion.RotateTowards(_currentLocalR, _currentTargetLocalR, maxDistanceDelta);
        transform.localPosition = _currentLocalP;
        transform.localRotation = _currentLocalR;
    }

    private void SetDroneMode() {
        if (!_playerUnit.SlowMode) { // 샷 모드
            _currentTargetLocalP = m_PlayerDroneTransformData.shotTransformData[_shotIndex].positionData;
            _currentTargetLocalR = Quaternion.Euler(m_PlayerDroneTransformData.shotTransformData[_shotIndex].rotationData);
            m_ParticleObject.SetActive(false);
        }
        else { // 레이저 모드
            _currentTargetLocalP = m_PlayerDroneTransformData.laserTransformData[_laserIndex].positionData;
            _currentTargetLocalR = Quaternion.Euler(m_PlayerDroneTransformData.laserTransformData[_laserIndex].rotationData);
            m_ParticleObject.SetActive(true);
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
        _shotIndex = _playerShotHandler.ShotIndex;
        _laserIndex = _playerLaserHandler.LaserIndex;
        
        foreach (var particle in _particleSystems)
        {
            particle.gameObject.SetActive(false);
        }
        _particleSystems[_laserIndex].gameObject.SetActive(true);
        
        //SetParticleScale(_playerUnit.PlayerAttackLevel);
    }

    public float GetCurrentLocalRotation() {
        return _currentLocalR.eulerAngles.y;
    }
}
