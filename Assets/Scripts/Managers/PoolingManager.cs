using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/*
m_ObjectPoolDictionary : 특정 종류의 오브젝트들의 List를 담은 PooledObject 들을 가진 Dictionary
_poolQueue : 오브젝트들을 담은 Queue. PooledObject 클래스 안에 존재. 아직 생성되지 않은 오브젝트들이 Queue 안에 있음.
*/

public class PoolingManager : MonoBehaviour
{
    public bool m_TestMode;
    public PoolingDatas m_PoolingData;
    private readonly Dictionary<string, PooledObject> m_ObjectPoolDictionary = new(); // 오브젝트 풀 큐를 종류별로 담은 딕셔너리

    private static PoolingManager Instance;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    

    void Start()
    {
        InitOutGame();
        InitInGame();
        //PlayerManager.Action_OnStartStartNewGame += InitInGame;
        //SystemManager.Action_OnQuitInGame += ClearAllObjectPool;
        
        //if (m_TestMode)
        //    InitInGame();
    }

    private void InitOutGame()
    {
        foreach (var poolingInfo in m_PoolingData.poolingOutGameInfos)
        {
            var parentTransform = GetChildByPoolingParent(PoolingParent.PlayerMissile);
            var pooledObject = new PooledObject(poolingInfo.poolingObject, poolingInfo.defaultNumber, parentTransform);
            m_ObjectPoolDictionary.Add(poolingInfo.objectName, pooledObject);
        }
        Debug.Log($"[Object Pooling] Init outGame object pool");
    }

    private void InitInGame()
    {
        foreach (var poolingInfo in m_PoolingData.poolingInfos)
        {
            var parentTransform = GetChildByPoolingParent(PoolingParent.PlayerMissile);
            var pooledObject = new PooledObject(poolingInfo.poolingObject, poolingInfo.defaultNumber, parentTransform);
            m_ObjectPoolDictionary.Add(poolingInfo.objectName, pooledObject);
        }
        Debug.Log($"[Object Pooling] Init inGame object pool");
    }

    private void ClearAllObjectPool()
    {
        m_ObjectPoolDictionary.Clear();
        Debug.Log($"[Object Pooling] Cleared all object pool");
        InitOutGame();
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
    private Queue<GameObject> _poolQueue = new Queue<GameObject>();

    // Constructor
    public PooledObject(GameObject prefab, int count, Transform parent = null) {
        m_PooledPrefabs = prefab;
        m_DefaultPoolCount = count;

        Vector3 pos = new Vector3(0f, 0f, 0f);
        Quaternion rot = Quaternion.identity;
        for (int i = 0; i < m_DefaultPoolCount; i++) {
            _poolQueue.Enqueue(CreateItem(parent));
        }
    }

    public void ClearQueue()
    {
        _poolQueue.Clear();
    }

    public void Initialize(Transform parent = null)
    {
        Vector3 pos = new Vector3(0f, 0f, 0f);
        Quaternion rot = Quaternion.identity;
        for (int i = 0; i < m_DefaultPoolCount; i++) {
            _poolQueue.Enqueue(CreateItem(parent));
        }
    }

    public void PushToPool(GameObject item, Transform parent = null)
    {
        item.transform.SetParent(parent);
        item.SetActive(false);
        _poolQueue.Enqueue(item);
    }

    public GameObject PopFromPool(Transform parent = null)
    {
        if (_poolQueue.Count == 0) {
            _poolQueue.Enqueue(CreateItem(parent));
        }
        
        GameObject item = _poolQueue.Dequeue();

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