using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour, IObjectPooling
{
    public string m_ObjectName;

    // [SerializeField] private float m_Lifetime;
    [SerializeField] private GameObject[] m_ActivatedObject;
    
    private readonly Animator[] _animator = new Animator[3];
    private int _hitEffectIndex;

    void Awake()
    {
        for(int i = 0; i < m_ActivatedObject.Length; i++) {
            _animator[i] = m_ActivatedObject[i].GetComponent<Animator>();
        }
    }

    public void OnStart() {
        for(int i = 0; i < m_ActivatedObject.Length; i++) {
            m_ActivatedObject[i].SetActive(false);
        }
        if (m_ActivatedObject.Length > 0) {
            m_ActivatedObject[_hitEffectIndex].SetActive(true);
        }
    }

    void Update()
    {
        if (_animator[_hitEffectIndex].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
            ReturnToPool();
        }
    }

    public void SetHitEffectType(HitEffectType hitEffectType)
    {
        _hitEffectIndex = (int) hitEffectType;
    }

    public void ReturnToPool() {
        PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.Explosion);
    }
}
