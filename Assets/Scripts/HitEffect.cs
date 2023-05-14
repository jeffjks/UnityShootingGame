using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour, IObjectPooling
{
    public string m_ObjectName;

    // [SerializeField] private float m_Lifetime;
    [SerializeField] private GameObject[] m_ActivatedObject = null;
    private PoolingManager m_PoolingManager = null;
    private Animator[] m_Animator = new Animator[3];

    [HideInInspector] public int m_HitEffectType;

    void Awake()
    {
        m_PoolingManager = PoolingManager.instance_op;
        for(int i = 0; i < m_ActivatedObject.Length; i++) {
            m_Animator[i] = m_ActivatedObject[i].GetComponent<Animator>();
        }
    }

    public void OnStart() {
        for(int i = 0; i < m_ActivatedObject.Length; i++) {
            m_ActivatedObject[i].SetActive(false);
        }
        if (m_ActivatedObject.Length > 0) {
            m_ActivatedObject[m_HitEffectType].SetActive(true);
        }
    }

    void Update()
    {
        if (m_Animator[m_HitEffectType].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
            ReturnToPool();
        }
    }

    public void ReturnToPool() {
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.EXPLOSION);
    }
}
