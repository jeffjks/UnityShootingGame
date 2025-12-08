using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;

public class DebugTriggerBody : MonoBehaviour
{
    public PlayerDebug m_PlayerDebug;
    public TextMeshProUGUI m_CountText;
    private bool _isCreating;

    private ObjectPool<PlayerDebug> _playerDebugPool;
    private const int PlayerDebugCount = 30;
    
    private int _periodCount;

    private void Awake()
    {
        _playerDebugPool = new ObjectPool<PlayerDebug>(
            CreateFunc,
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyPoolObject,
            collectionCheck: false,
            PlayerDebugCount
        );
    }

    private void Update()
    {
        CreateBulelt(3);
        CreatePlayerDebug(1);
        _periodCount++;

        if (_periodCount >= int.MaxValue)
        {
            _periodCount = 0;
        }
        m_CountText.SetText($"BulletCount: {BulletManager.GetBulletCount()}\nObjectCount: {PlayerDebug.Count}");
    }

    private PlayerDebug CreateFunc()
    {
        var obj = Instantiate(m_PlayerDebug, new Vector3(0f, 0f, Depth.PLAYER), Quaternion.identity);
        obj.SetPool(_playerDebugPool);
        obj.gameObject.SetActive(false);
        return obj;
    }

    private void OnGetFromPool(PlayerDebug obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(PlayerDebug obj)
    {
        obj.gameObject.SetActive(false);
    }
    
    private void OnDestroyPoolObject(PlayerDebug obj)
    {
        Destroy(obj.gameObject);
    }


    public void OnClickStart()
    {
        _isCreating = !_isCreating;
    }

    private void CreateBulelt(int count)
    {
        if (_isCreating == false)
            return;

        for (var i = 0; i < count; ++i)
        {
            BulletProperty property = new (new Vector3(0f, -2f, Depth.ENEMY_BULLET), BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, Random.Range(0f, 360f));
            GameObject bulletObject = PoolingManager.PopFromPool("EnemyBullet", PoolingParent.EnemyBullet);
            var enemyBullet = bulletObject.GetComponent<EnemyBullet>();
            enemyBullet.transform.position = property.startPos;
            bulletObject.SetActive(true);
            enemyBullet.OnStart(property);
        }
    }

    public void CreatePlayerDebug(int count)
    {
        if (_isCreating == false)
            return;
        
        for (var i = 0; i < count; ++i)
        {
            var obj = _playerDebugPool.Get();
            obj.transform.SetPositionAndRotation(new Vector3(Random.Range(-4f, 4f), -14f, Depth.PLAYER), Quaternion.identity);
        }
    }
}