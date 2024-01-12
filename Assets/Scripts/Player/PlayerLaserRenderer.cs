using UnityEngine;

public class PlayerLaserRenderer : MonoBehaviour
{
    public GameObject m_FireEffect;
    public GameObject m_StormEffect;
    public GameObject m_RushEffect;
    public GameObject m_HitEffect;
    public LaserTransformDatas m_LaserTransformData;

    private PlayerLaserHandler _playerLaserHandler;
    private LineRenderer _lineRenderer;
    private ParticleSystem[] _fireParticles;
    private ParticleSystem[] _stormParticles;
    private ParticleSystem[] _rushParticles;
    private ParticleSystem[] _hitParticles;
    private BoxCollider2D _collider2D;
    private PlayerUnit _playerUnit;
    private Vector2 _laserHitBoxWidth;
    private bool _laserSaver;
    private float _currentLaserLength;
    private ParticleSystem.MainModule _particleMainModule;
    private float _stormSpeed;
    
    private const float LASER_SPEED = 30f;
    private const float MINIMUM_LASER_LENGTH = 0.1f;
    private const float ENDPOINT_ALPHA = 0.2f;

    private float CurrentLaserLength
    {
        get => _currentLaserLength;
        set
        {
            _currentLaserLength = value;
            OnChangedLaserLength();
        }
    }

    private void Awake ()
    {
        _collider2D = GetComponentInParent<BoxCollider2D>();
        _playerLaserHandler = GetComponentInParent<PlayerLaserHandler>();
        _playerUnit = GetComponentInParent<PlayerUnit>();

        _fireParticles = m_FireEffect.GetComponentsInChildren<ParticleSystem>();
        _stormParticles = m_StormEffect.GetComponentsInChildren<ParticleSystem>();
        _rushParticles = m_RushEffect.GetComponentsInChildren<ParticleSystem>();
        _hitParticles = m_HitEffect.GetComponentsInChildren<ParticleSystem>();

        _lineRenderer = GetComponent<LineRenderer>();

        if (_stormParticles.Length > 0) {
            _particleMainModule = _stormParticles[0].main;
            _stormSpeed = _particleMainModule.startSpeed.constant;
        }
        SetLaserScale();

        _playerLaserHandler.Action_OnStartLaser += OnStartLaser;
        _playerLaserHandler.Action_OnStopLaser += OnStopLaser;
        _playerUnit.Action_OnUpdatePlayerAttackLevel += SetLaserScale;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        var curPos = transform.position;
        
        _lineRenderer.SetPosition(0, curPos);
        //Debug.DrawRay(curPos - (Vector3)_laserHitBoxWidth, transform.forward, Color.red, 0.1f);
        //Debug.DrawRay(curPos + (Vector3)_laserHitBoxWidth, transform.forward, Color.red, 0.1f);

        RaycastHit2D hit1 = Physics2D.Raycast((Vector2) curPos - _laserHitBoxWidth, Vector2.up, CurrentLaserLength, Layer.LARGE);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2) curPos, Vector2.up, CurrentLaserLength, Layer.LARGE);
        RaycastHit2D hit3 = Physics2D.Raycast((Vector2) curPos + _laserHitBoxWidth, Vector2.up, CurrentLaserLength, Layer.LARGE);

        if (hit1.collider || hit2.collider || hit3.collider) // 하나라도 충돌하면
        {
            float min_y = Mathf.Min(hit1.point.y, hit2.point.y, hit3.point.y);
            //Vector3 endPoint = new Vector3(transform.position.x, min_y, Depth.PLAYER); // 가장 작은 y좌표를 endpoint로
            CurrentLaserLength = Mathf.Max(min_y - _playerLaserHandler.transform.position.y + ENDPOINT_ALPHA, MINIMUM_LASER_LENGTH);

            PlayParticles(_fireParticles);
            PlayParticles(_stormParticles);
            PlayParticles(_hitParticles);
            PlayParticles(_rushParticles);
        }
        else { // 아무 충돌이 없으면
            ShootLaserForward();

            PlayParticles(_fireParticles);
            PlayParticles(_stormParticles);
            if (m_RushEffect.transform.position.y < 0f) {
                PlayParticles(_rushParticles);
            }
            else {
                StopParticles(_rushParticles);
            }
            StopParticles(_hitParticles);
        }

        //Insurance against the appearance of a laser in the center of coordinates!
        if (!_lineRenderer.enabled && !_laserSaver)
        {
            _laserSaver = true;
            _lineRenderer.enabled = true;
        }
    }

    private void ShootLaserForward()
    {
        if (Time.timeScale == 0)
            return;
        
        if (_playerUnit.SlowMode) {
            CurrentLaserLength += LASER_SPEED / Application.targetFrameRate * Time.timeScale;
        }

        var maxClampLength = _playerUnit.m_IsPreviewObject ? _playerUnit.m_MaxLaserLength : -transform.position.y + 1f;
        CurrentLaserLength = Mathf.Clamp(CurrentLaserLength, 0f, maxClampLength);
    }

    private void OnChangedLaserLength()
    {
        var endPoint = transform.position + Vector3.up * CurrentLaserLength;
        _lineRenderer.SetPosition(1, endPoint);
        m_HitEffect.transform.position = endPoint;
        m_RushEffect.transform.position = endPoint;

        if (_stormParticles.Length > 0)
            _particleMainModule.startLifetime = - CurrentLaserLength / _stormSpeed * 0.6f;

        var colliderLength = Mathf.Min(CurrentLaserLength, -transform.position.y);
        _collider2D.offset = new Vector2(_collider2D.offset.x, colliderLength / 2f);
        _collider2D.size = new Vector2(_collider2D.size.x, colliderLength);
    }

    private void OnStartLaser() {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);
        
        StopParticles(_stormParticles);
        StopParticles(_hitParticles);
    }

    private void OnStopLaser()
    {
        DisablePrepare();
        CurrentLaserLength = 0f;
    }

    private void SetLaserScale()
    {
        var laserLevel = _playerUnit.PlayerAttackLevel;
        
        m_FireEffect.transform.localScale = m_LaserTransformData.fireLocalScale[laserLevel];
        m_RushEffect.transform.localScale = m_LaserTransformData.rushLocalScale[laserLevel];
        m_HitEffect.transform.localScale = m_LaserTransformData.hitLocalScale[laserLevel];
        
        var laserWidth = m_LaserTransformData.laserWidth[laserLevel];
        _lineRenderer.startWidth = laserWidth;
        _lineRenderer.endWidth = laserWidth;
        
        var hitBoxWidth = laserWidth*0.75f; // 레이저 히트박스 크기 (Raycast도 자동 조절)
        _laserHitBoxWidth = new Vector2(hitBoxWidth*0.5f, 0f);
        _collider2D.size = new Vector2(hitBoxWidth, 0f);
    }

    private void DisablePrepare() // Initiate Laser
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);
        
        //_fireParticles can = null in multiply shooting
        StopParticles(_fireParticles);
        StopParticles(_stormParticles);
        StopParticles(_rushParticles);
        StopParticles(_hitParticles);
    }

    private void PlayParticles(ParticleSystem[] particleSystems) {
        foreach (var particle in particleSystems) {
            if (!particle.isPlaying) particle.Play();
        }
    }

    private void StopParticles(ParticleSystem[] particleSystems) {
        if (particleSystems != null) {
            foreach (var particle in particleSystems) {
                if (particle.isPlaying) particle.Stop();
            }
        }
    }
}
