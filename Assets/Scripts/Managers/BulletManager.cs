using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static int BulletNumber;
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
    
    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SystemManager.Action_OnNextStage += (hasNextStage) => _bulletsSortingLayer = 0;
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
        List<GameObject> bulletList = GameObject.FindGameObjectsWithTag("EnemyBulletParent").ToList();
        int index, num = 0, count = bulletList.Count;

        while (count > 0) {
            index = UnityEngine.Random.Range(0, count);
            if (num < 50) {
                Vector3 pos = bulletList[index].transform.position;
                if (pos.x is > Size.GAME_BOUNDARY_LEFT and < Size.GAME_BOUNDARY_RIGHT) {
                    if (pos.y is > Size.GAME_BOUNDARY_BOTTOM and < Size.GAME_BOUNDARY_TOP) {
                        GameObject gem = PoolingManager.PopFromPool("ItemGemAir", PoolingParent.GemAir); // Gem 생성
                        pos.z = Depth.ITEMS;
                        gem.transform.position = pos;
                        gem.SetActive(true);
                        num++;
                    }
                }
            }
            EnemyBullet enemy_bullet = bulletList[index].GetComponent<EnemyBullet>();
            enemy_bullet.ReturnToPool();
            bulletList.RemoveAt(index);

            count--;
        }
        SetBulletFreeState(millisecond);
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
}
