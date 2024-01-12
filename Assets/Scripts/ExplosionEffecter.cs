using UnityEngine;
using System.Collections;

public class ExplosionEffecter : MonoBehaviour, IObjectPooling
{
    public string m_ObjectName;
    public int m_Lifetime;

    [HideInInspector] public MoveVector m_MoveVector;
    
    private IEnumerator m_ExplosionTimer;

    void Awake()
    {
        m_MoveVector = new MoveVector(0f, 0f);
    }

    void OnEnable() {
        m_ExplosionTimer = ExplosionTimer();
        StartCoroutine(m_ExplosionTimer);
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        
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
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.Explosion);
    }
}