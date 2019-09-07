using UnityEngine;
using System.Collections;

public class ExplosionEffect : MonoBehaviour
{
    public string m_ObjectName;
    public float m_Lifetime;

    [HideInInspector] public MoveVector m_MoveVector;
    
    private bool m_OnEnable = false;

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

    void OnEnable()
    {
        if (m_OnEnable) {
            Invoke("OnDeath", m_Lifetime);
        }
        else
            m_OnEnable = true;
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Update()
    {
        MoveDirection(m_MoveVector);
    }

    private void MoveDirection(MoveVector movevector)
    {
        Vector2 vector2 = Quaternion.AngleAxis(movevector.direction, Vector3.forward) * Vector2.down;
        transform.Translate(vector2 * movevector.speed * Time.deltaTime, Space.World);
    }

    private void OnDeath() {
        m_MoveVector.speed = 0f;
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.EXPLOSION);
    }
}