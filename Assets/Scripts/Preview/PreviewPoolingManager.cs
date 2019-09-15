using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPoolingManager : PoolingManager
{
    void Start()
    {
        Dictionary<string, PooledObject> pooledObject = new Dictionary<string, PooledObject>();

        m_ObjectPoolDictionary.Add("PlayerShot", new PooledObject(m_PooledPrefabs[0], 30, transform));
        m_ObjectPoolDictionary.Add("PlayerHomingMissile", new PooledObject(m_PooledPrefabs[1], 6, transform));
        m_ObjectPoolDictionary.Add("PlayerRocket", new PooledObject(m_PooledPrefabs[2], 6, transform));
        m_ObjectPoolDictionary.Add("PlayerAddShot", new PooledObject(m_PooledPrefabs[3], 8, transform));
    }
}