using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissileHitEffect : MonoBehaviour, IObjectPooling
{
    public string m_ObjectName;
    public int m_LifeTime;
    
    private int _hitEffectIndex;

    public void OnStart() {
        StartCoroutine(RemoveTimer());
    }

    public void ReturnToPool() {
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.Explosion);
    }

    private IEnumerator RemoveTimer()
    {
        yield return new WaitForMillisecondFrames(m_LifeTime);
        ReturnToPool();
    }
}
