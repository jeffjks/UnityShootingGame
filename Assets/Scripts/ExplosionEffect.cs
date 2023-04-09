using UnityEngine;
using System.Collections;

public class ExplosionEffect : MonoBehaviour, UseObjectPool
{
    public string m_ObjectName;
    public int m_Lifetime;

    [HideInInspector] public MoveVector m_MoveVector;
    
    private IEnumerator m_ExplosionTimer;

    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;
    private PoolingManager m_PoolingManager = null;

    void Awake()
    {
        m_MoveVector = new MoveVector(0f, 0f);
        m_PoolingManager = PoolingManager.instance_op;
        m_PlayerManager = PlayerManager.instance_pm;
        m_SystemManager = SystemManager.instance_sm;
    }

    public void OnStart() {
        m_ExplosionTimer = ExplosionTimer();
        StartCoroutine(m_ExplosionTimer);
    }

    void Update()
    {
        MoveDirection(m_MoveVector);
    }

    private void MoveDirection(MoveVector movevector)
    {
        Vector2 vector2 = Quaternion.AngleAxis(movevector.direction, Vector3.forward) * Vector2.down;
        transform.Translate(vector2 * movevector.speed / Application.targetFrameRate * Time.timeScale, Space.World);
    }

    private IEnumerator ExplosionTimer() {
        yield return new WaitForMillisecondFrames(m_Lifetime);
        m_MoveVector.speed = 0f;
        ReturnToPool();
        yield break;
    }

    public void ReturnToPool() {
        if (m_ExplosionTimer != null) {
            StopCoroutine(m_ExplosionTimer);
        }
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.EXPLOSION);
    }
}