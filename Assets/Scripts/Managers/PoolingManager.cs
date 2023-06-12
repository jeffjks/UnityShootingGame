using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
m_ObjectPoolDictionary : 특정 종류의 오브젝트들의 List를 담은 PooledObject 들을 가진 Dictionary
m_PoolQueue : 오브젝트들을 담은 Queue. PooledObject 클래스 안에 존재. 아직 생성되지 않은 오브젝트들이 Queue 안에 있음.
*/

public class PoolingManager : MonoBehaviour
{
    protected Dictionary<string, PooledObject> m_ObjectPoolDictionary = new Dictionary<string, PooledObject>(); // 오브젝트 풀 큐를 종류별로 담은 딕셔너리
    [SerializeField] protected GameObject[] m_PooledPrefabs;

    private static PoolingManager Instance = null;

    void Awake()
    {
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    

    void Start()
    {
        //m_Module = m_GameManager.m_CurrentAttributes.GetAttributes(AttributeType.Module); // 모듈 종류

        m_ObjectPoolDictionary.Add("PlayerShot", new PooledObject(m_PooledPrefabs[0], 30, GetChildByPoolingParent(PoolingParent.PlayerMissile)));

        m_ObjectPoolDictionary.Add("PlayerHomingMissile", new PooledObject(m_PooledPrefabs[1], 8, GetChildByPoolingParent(PoolingParent.PlayerMissile)));
        m_ObjectPoolDictionary.Add("PlayerRocket", new PooledObject(m_PooledPrefabs[2], 6, GetChildByPoolingParent(PoolingParent.PlayerMissile)));
        m_ObjectPoolDictionary.Add("PlayerAddShot", new PooledObject(m_PooledPrefabs[3], 8, GetChildByPoolingParent(PoolingParent.PlayerMissile)));

        /*
        switch(m_Module) {
            case 1: // Homing
                m_ObjectPoolDictionary.Add("PlayerHomingMissile", new PooledObject(m_PooledPrefabs[1], 6, GetChildByPoolingParent(PoolingParent.PlayerMissile)));
                break;
            case 2: // Rocket
                m_ObjectPoolDictionary.Add("PlayerRocket", new PooledObject(m_PooledPrefabs[2], 6, GetChildByPoolingParent(PoolingParent.PlayerMissile)));
                break;
            case 3: // AddShot
                m_ObjectPoolDictionary.Add("PlayerAddShot", new PooledObject(m_PooledPrefabs[3], 8, GetChildByPoolingParent(PoolingParent.PlayerMissile)));
                break;
            default:
                break;
        }*/

        m_ObjectPoolDictionary.Add("EnemyBullet", new PooledObject(m_PooledPrefabs[4], 1024, GetChildByPoolingParent(PoolingParent.EnemyBullet)));

        m_ObjectPoolDictionary.Add("ExplosionGround_1", new PooledObject(m_PooledPrefabs[5], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionGround_2", new PooledObject(m_PooledPrefabs[6], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionGround_3", new PooledObject(m_PooledPrefabs[7], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionNormal_1", new PooledObject(m_PooledPrefabs[8], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionNormal_2", new PooledObject(m_PooledPrefabs[9], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionNormal_3", new PooledObject(m_PooledPrefabs[10], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionSimple_1", new PooledObject(m_PooledPrefabs[11], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionSimple_2", new PooledObject(m_PooledPrefabs[12], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionStarShape", new PooledObject(m_PooledPrefabs[13], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("ExplosionMineShape", new PooledObject(m_PooledPrefabs[14], 15, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("PlayerExplosion", new PooledObject(m_PooledPrefabs[15], 1, GetChildByPoolingParent(PoolingParent.Explosion)));
        m_ObjectPoolDictionary.Add("PlayerHitEffect", new PooledObject(m_PooledPrefabs[16], 40, GetChildByPoolingParent(PoolingParent.Explosion)));
        
        m_ObjectPoolDictionary.Add("ItemGemAir", new PooledObject(m_PooledPrefabs[17], 60, GetChildByPoolingParent(PoolingParent.GemAir)));
        
        m_ObjectPoolDictionary.Add("ItemGemGround", new PooledObject(m_PooledPrefabs[18], 20, GetChildByPoolingParent(PoolingParent.GemGround)));

        m_ObjectPoolDictionary.Add("Debris", new PooledObject(m_PooledPrefabs[19], 30, GetChildByPoolingParent(PoolingParent.Debris)));

        m_ObjectPoolDictionary.Add("ScoreText", new PooledObject(m_PooledPrefabs[20], 10, GetChildByPoolingParent(PoolingParent.ScoreText)));
    }

    public static bool PushToPool(string itemName, GameObject item, PoolingParent parent = PoolingParent.None) // = -1
    {
        PooledObject pool = Instance.GetPoolItem(itemName);
        if (pool == null)
            return false;
        
        if (parent == PoolingParent.None)
            pool.PushToPool(item, Instance.transform);
        else
            pool.PushToPool(item, Instance.GetChildByPoolingParent(parent));

        //pool.PushToPool(item, parent == null ? transform : parent);
        return true;
    }

    public static GameObject PopFromPool(string itemName, PoolingParent child_number = PoolingParent.None)
    {
        PooledObject pool = Instance.GetPoolItem(itemName);
        if (pool == null)
            return null;

        if (child_number == PoolingParent.None)
            return pool.PopFromPool(Instance.transform);
        else
            return pool.PopFromPool(Instance.GetChildByPoolingParent(child_number));
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

    public static void ResetPool()
    {
        Instance.GetChildByPoolingParent(PoolingParent.Debris).position = Vector3.zero;
        Instance.GetChildByPoolingParent(PoolingParent.GemGround).position = Vector3.zero;;
        PushToPoolAll();
    }

    private static void PushToPoolAll() // 풀에 활성화 상태로 남아있는 모든 오브젝트 비활성화 후 풀로 되돌리기 (이전 판에 남아있던 영향 제거)
    {
        foreach (Transform childObject in Instance.transform) { // 모든 직계 자식 오브젝트 순회
            foreach (Transform item in childObject) { // 모든 직계 자식(손자) 오브젝트 순회
                if (item.gameObject.activeSelf) {
                    IObjectPooling script = item.GetComponentInChildren<IObjectPooling>();
                    script.ReturnToPool();
                }
            }
        }
    }

    public Transform GetChildByPoolingParent(PoolingParent parent)
    {
        return transform.GetChild((int) parent);
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