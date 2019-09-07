using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
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
        for(int i=0; i<m_ActivatedObject.Length; i++) {
            m_Animator[i] = m_ActivatedObject[i].GetComponent<Animator>();
        }
    }

    void OnEnable()
    {
        int randomRotation = (int) Random.Range(0f, 360f);
        transform.eulerAngles = new Vector3(0f, 0f, randomRotation);
        for(int i=0; i<m_ActivatedObject.Length; i++) {
            m_ActivatedObject[i].SetActive(false);
        }
        if (m_ActivatedObject.Length > 0) {
            m_ActivatedObject[m_HitEffectType].SetActive(true);
        }

        //Invoke("OnDeath", m_Lifetime);
    }

    void Update()
    {
        if (m_Animator[m_HitEffectType].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) {
            OnDeath();
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    private void OnDeath() {
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.EXPLOSION);
    }
}
