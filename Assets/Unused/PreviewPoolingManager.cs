using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPoolingManager : PoolingManager
{
    void Start()
    {
        Debug.Log("PreviewPoolingManager");
        m_ObjectPoolDictionary.Add("PlayerShot", new PooledObject(m_PooledPrefabs[0], 20, transform));
        m_ObjectPoolDictionary.Add("PlayerHomingMissile", new PooledObject(m_PooledPrefabs[1], 4, transform));
        m_ObjectPoolDictionary.Add("PlayerRocket", new PooledObject(m_PooledPrefabs[2], 4, transform));
        m_ObjectPoolDictionary.Add("PlayerAddShot", new PooledObject(m_PooledPrefabs[3], 4, transform));
    }
}