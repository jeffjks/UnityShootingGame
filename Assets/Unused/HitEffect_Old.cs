using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect_Old : MonoBehaviour, IObjectPooling
{
    public string m_ObjectName;
    public int m_LifeTime;

    // [SerializeField] private float m_Lifetime;
    [SerializeField] private GameObject[] m_ActivatedObject;
    
    private int _hitEffectIndex;

    public void OnStart() {
        m_ActivatedObject[_hitEffectIndex].SetActive(true);
        StartCoroutine(RemoveTimer());
    }

    // public void SetHitEffectType(HitEffectType hitEffectType)
    // {
    //     _hitEffectIndex = (int) hitEffectType;
    // }

    public void ReturnToPool() {
        m_ActivatedObject[_hitEffectIndex].SetActive(false);
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.Explosion);
    }

    private IEnumerator RemoveTimer()
    {
        yield return new WaitForMillisecondFrames(m_LifeTime);
        ReturnToPool();
    }
}
