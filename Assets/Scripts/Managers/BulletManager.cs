using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private bool _destroySingleton;
    private const int GridRegionWidth = 30;
    private const int GridRegionHeight = 32;
    private static readonly bool[][] _gridRegion = new bool[GridRegionWidth][];
    
    public static LinkedList<EnemyBullet> EnemyBulletList = new();
    private static int _bulletsSortingLayer;
    public static int BulletsSortingLayer
    {
        get => _bulletsSortingLayer;
        set
        {
            _bulletsSortingLayer = value;
            ResetSortingLayer();
        }
    }

    public static event Action Action_OnBulletFreeStateStart;

    private static bool _inBulletFreeState;
    public static bool InBulletFreeState
    {
        get => _inBulletFreeState;
        private set
        {
            _inBulletFreeState = value;
            if (_inBulletFreeState)
                Action_OnBulletFreeStateStart?.Invoke();
        }
    }
    private static int _remainingFrame;
    private static BulletManager Instance { get; set; }

    static BulletManager()
    {
        for (var i = 0; i < GridRegionWidth; ++i)
        {
            _gridRegion[i] = new bool[GridRegionHeight];
        }
    }
    
    void Awake()
    {
        if (Instance != null)
        {
            _destroySingleton = true;
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SystemManager.Action_OnNextStage += InitSortingLayer;
    }

    private void OnDestroy()
    {
        if (_destroySingleton)
            return;
        SystemManager.Action_OnNextStage -= InitSortingLayer;
    }

    private void InitSortingLayer(bool hasNextStage)
    {
        _bulletsSortingLayer = 0;
    }

    public static void SetBulletFreeState(int millisecond)
    {
        int frame = millisecond * Application.targetFrameRate / 1000;
        if (frame < _remainingFrame || frame <= 0)
        {
            return;
        }

        InBulletFreeState = true;
        _remainingFrame = frame;
    }

    public static void BulletsToGems(int millisecond)
    {
        var node = EnemyBulletList.First;
        while (node != null)
        {
            var currentNode = node;
            node = node.Next;
            
            var bulletPosition = currentNode.Value.transform.position;
            if (!IsGridRegionAssignable(bulletPosition))
                continue;
            
            var gem = PoolingManager.PopFromPool("ItemGemAir", PoolingParent.GemAir); // Gem 생성
            bulletPosition.z = Depth.ITEMS;
            gem.transform.position = bulletPosition;
            gem.SetActive(true);

            currentNode.Value.RemoveFromBulletList();
        }
        
        SetBulletFreeState(millisecond);
        return;
        
        List<GameObject> bulletList = GameObject.FindGameObjectsWithTag("EnemyBulletParent").ToList();
        int num = 0, count = bulletList.Count;

        while (count > 0)
        {
            var index = UnityEngine.Random.Range(0, count);
            var enemyBullet = bulletList[index].GetComponent<EnemyBullet>();
            
            if (num < 50) {
                var pos = bulletList[index].transform.position;

                do
                {
                    if (pos.x is <= Size.GAME_BOUNDARY_LEFT or >= Size.GAME_BOUNDARY_RIGHT)
                        break;
                    if (pos.y is <= Size.GAME_BOUNDARY_BOTTOM or >= Size.GAME_BOUNDARY_TOP)
                        break;
                    if (enemyBullet.IsPlayingEraseAnimation)
                        break;

                    var gem = PoolingManager.PopFromPool("ItemGemAir", PoolingParent.GemAir); // Gem 생성
                    pos.z = Depth.ITEMS;
                    gem.transform.position = pos;
                    gem.SetActive(true);
                    num++;
                } while (false);
            }
            enemyBullet.ReturnToPool();
            bulletList.RemoveAt(index);

            count--;
        }
        SetBulletFreeState(millisecond);
    }

    private static bool IsGridRegionAssignable(Vector2 position)
    {
        var wIndex = (int) ((position.x - Size.GAME_BOUNDARY_LEFT) / Size.GAME_WIDTH * GridRegionWidth);
        var hIndex = (int) ((position.y - Size.GAME_BOUNDARY_BOTTOM) / Size.GAME_HEIGHT * GridRegionHeight);

        if (wIndex is < 0 or >= GridRegionWidth)
            return false;
        if (hIndex is < 0 or >= GridRegionHeight)
            return false;
        
        if (!_gridRegion[wIndex][hIndex])
        {
            _gridRegion[wIndex][hIndex] = true;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (_remainingFrame > 0)
        {
            _remainingFrame--;
        }
        else if (InBulletFreeState)
        {
            InBulletFreeState = false;
        }
    }

    private static void ResetSortingLayer()
    {
        if (_bulletsSortingLayer >= int.MaxValue)
        {
            _bulletsSortingLayer = int.MinValue;
        }
    }

    public static int GetBulletCount()
    {
        return EnemyBulletList.Count;
    }
}
