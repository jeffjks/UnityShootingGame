using UnityEngine;
using UnityEngine.Pool;

public class PlayerDebug : MonoBehaviour
{
    private IObjectPool<PlayerDebug> _pool;
    public TriggerBody m_TriggerBody;

    public static int Count;

    private const int REMOVE_TIMER = 600;

    private Vector2 _direction;
    private float _speed;
    private bool _hasCollided;
    private int _removeTimer = REMOVE_TIMER;

    private void Awake()
    {
        m_TriggerBody.m_OnTriggerBodyEnter += OnTriggerBodyEnter;
    }

    private void OnDestroy()
    {
        m_TriggerBody.m_OnTriggerBodyEnter -= OnTriggerBodyEnter;
    }

    private void OnEnable()
    {
        _hasCollided = false;
        _removeTimer = REMOVE_TIMER;
        _direction = Vector2.up; //Random.insideUnitCircle.normalized;
        _speed = Random.Range(2f, 3f);
        Count++;
    }

    private void OnDisable()
    {
        Count--;
    }

    private void Update()
    {
        transform.Translate(_direction * _speed / Application.targetFrameRate * Time.timeScale, Space.World);
        _removeTimer--;

        if (_removeTimer <= 0)
        {
            ReleaseToPool();
        }
    }

    public void SetPool(IObjectPool<PlayerDebug> pool)
    {
        _pool = pool;
    }

    public void ReleaseToPool()
    {
        if (_pool != null)
        {
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerBodyEnter(TriggerBody other) // 충돌 감지
    {
        if (other.m_TriggerBodyType == TriggerBodyType.Bullet) // 대상이 총알이면 대상과 자신 파괴
        {
            var enemyBullet = other.gameObject.GetComponentInParent<EnemyBullet>();
            TriggerEnter(enemyBullet);
        }
    }

    private void TriggerEnter(EnemyBullet enemyBullet)
    {
        if (_hasCollided)
            return;
        _hasCollided = true;
        enemyBullet.PlayEraseAnimation();
        ReleaseToPool();
    }
}
