using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
m_ObjectPoolDictionary : 특정 종류의 오브젝트들의 List를 담은 PooledObject 들을 가진 Dictionary
m_PoolQueue : 오브젝트들을 담은 Queue. PooledObject 클래스 안에 존재. 아직 생성되지 않은 오브젝트들이 Queue 안에 있음.
*/
public static class PoolingParent
{
    public const sbyte NONE = -1;
    public const sbyte PLAYER_MISSILE = 0;
    public const sbyte ENEMY_BULLET = 1;
    public const sbyte EXPLOSION = 2;
    public const sbyte ITEM_GEM = 3;
    public const sbyte DEBRIS = 4;
}

public class PoolingManager : MonoBehaviour
{
    protected Dictionary<string, PooledObject> m_ObjectPoolDictionary = new Dictionary<string, PooledObject>();
    [SerializeField] protected GameObject[] m_PooledPrefabs;

    private PlayerManager m_PlayerManager = null;
    private int m_Module;

    public static PoolingManager instance_op = null;

    void Awake()
    {
        if (instance_op != null) {
            Destroy(this.gameObject);
            return;
        }
        instance_op = this;
        
        // DontDestroyOnLoad(gameObject);
    }
    

    void Start()
    {
        m_PlayerManager = PlayerManager.instance_pm;

        m_Module = m_PlayerManager.m_CurrentAttributes.m_Module; // 모듈 종류

        Dictionary<string, PooledObject> pooledObject = new Dictionary<string, PooledObject>();

        m_ObjectPoolDictionary.Add("PlayerShot", new PooledObject(m_PooledPrefabs[0], 30, transform.GetChild(PoolingParent.PLAYER_MISSILE)));

        switch(m_Module) {
            case 1: // Homing
                m_ObjectPoolDictionary.Add("PlayerHomingMissile", new PooledObject(m_PooledPrefabs[1], 6, transform.GetChild(PoolingParent.PLAYER_MISSILE)));
                break;
            case 2: // Rocket
                m_ObjectPoolDictionary.Add("PlayerRocket", new PooledObject(m_PooledPrefabs[2], 6, transform.GetChild(PoolingParent.PLAYER_MISSILE)));
                break;
            case 3: // AddShot
                m_ObjectPoolDictionary.Add("PlayerAddShot", new PooledObject(m_PooledPrefabs[3], 8, transform.GetChild(PoolingParent.PLAYER_MISSILE)));
                break;
            default:
                break;
        }

        m_ObjectPoolDictionary.Add("EnemyBullet", new PooledObject(m_PooledPrefabs[4], 1024, transform.GetChild(PoolingParent.ENEMY_BULLET)));

        m_ObjectPoolDictionary.Add("ExplosionG_1", new PooledObject(m_PooledPrefabs[5], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("ExplosionG_2", new PooledObject(m_PooledPrefabs[6], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("ExplosionG_3", new PooledObject(m_PooledPrefabs[7], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("Explosion_1", new PooledObject(m_PooledPrefabs[8], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("Explosion_2", new PooledObject(m_PooledPrefabs[9], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("Explosion_3", new PooledObject(m_PooledPrefabs[10], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("ExplosionSimple_1", new PooledObject(m_PooledPrefabs[11], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("ExplosionSimple_2", new PooledObject(m_PooledPrefabs[12], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("ExplosionStar", new PooledObject(m_PooledPrefabs[13], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("ExplosionMine", new PooledObject(m_PooledPrefabs[14], 15, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("PlayerExplosion", new PooledObject(m_PooledPrefabs[15], 1, transform.GetChild(PoolingParent.EXPLOSION)));
        m_ObjectPoolDictionary.Add("PlayerHitEffect", new PooledObject(m_PooledPrefabs[16], 20, transform.GetChild(PoolingParent.EXPLOSION)));
        
        m_ObjectPoolDictionary.Add("ItemGemAir", new PooledObject(m_PooledPrefabs[17], 60, transform.GetChild(PoolingParent.ITEM_GEM)));
        m_ObjectPoolDictionary.Add("ItemGemGround", new PooledObject(m_PooledPrefabs[18], 20, transform.GetChild(PoolingParent.ITEM_GEM)));

        m_ObjectPoolDictionary.Add("Debris", new PooledObject(m_PooledPrefabs[19], 20, transform.GetChild(PoolingParent.DEBRIS)));
    }

    public bool PushToPool(string itemName, GameObject item, sbyte parent = -1) // = -1
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
            return false;
        
        if (parent == -1)
            pool.PushToPool(item, transform);
        else
            pool.PushToPool(item, transform.GetChild(parent));

        //pool.PushToPool(item, parent == null ? transform : parent);
        return true;
    }

    public GameObject PopFromPool(string itemName, sbyte child_number = -1)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
            return null;

        if (child_number == -1)
            return pool.PopFromPool(transform);
        else
            return pool.PopFromPool(transform.GetChild(child_number));
    }

    private PooledObject GetPoolItem(string itemName)
    {
        try {
            PooledObject pool = m_ObjectPoolDictionary[itemName];
            if (pool != null)
                return pool;
        }
        catch (KeyNotFoundException) {
            Debug.LogAssertion(itemName);
        }

        Debug.LogWarning("There's no matched pool list.");
        return null;
    }
}


[System.Serializable]
public class PooledObject
{
    // public string m_PoolItemName = string.Empty;
    public GameObject m_PooledPrefabs = null;
    public int m_DefaultPoolCount;
    
    [SerializeField]
    private Queue<GameObject> m_PoolQueue = new Queue<GameObject>();

    // Constructor
    public PooledObject(GameObject prefab, int count, Transform parent = null) {
        m_PooledPrefabs = prefab;
        m_DefaultPoolCount = count;

        Vector3 pos = new Vector3(0f, 0f, 0f);
        Quaternion rot = Quaternion.identity;
        for (int i = 0; i < m_DefaultPoolCount; i++) {
            m_PoolQueue.Enqueue(CreateItem(parent));
        }
    }

    public void Initialize(Transform parent = null)
    {
        Vector3 pos = new Vector3(0f, 0f, 0f);
        Quaternion rot = Quaternion.identity;
        for (int i = 0; i < m_DefaultPoolCount; i++) {
            m_PoolQueue.Enqueue(CreateItem(parent));
        }
    }

    public void PushToPool(GameObject item, Transform parent = null)
    {
        item.transform.SetParent(parent);
        item.SetActive(false);
        m_PoolQueue.Enqueue(item);
    }

    public GameObject PopFromPool(Transform parent = null)
    {
        if (m_PoolQueue.Count == 0) {
            m_PoolQueue.Enqueue(CreateItem(parent));
        }
        
        GameObject item = m_PoolQueue.Dequeue();

        return item;
    }

    private GameObject CreateItem(Transform parent = null)
    {
        GameObject item = Object.Instantiate(m_PooledPrefabs);
        item.transform.SetParent(parent);
        item.SetActive(false);

        return item;
    }
}