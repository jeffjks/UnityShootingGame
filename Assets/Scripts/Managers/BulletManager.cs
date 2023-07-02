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
    
    public static bool InBulletFreeState { get; private set; }
    private static int _remainingFrame;
    private static BulletManager Instance { get; set; }
    
    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        Debug.Log(count);

        while (count > 0) {
            index = UnityEngine.Random.Range(0, count);
            if (num < 50) {
                Vector3 pos = bulletList[index].transform.position;
                if (Size.GAME_BOUNDARY_LEFT < pos.x && pos.x < Size.GAME_BOUNDARY_RIGHT) {
                    if (Size.GAME_BOUNDARY_BOTTOM < pos.y && pos.y < Size.GAME_BOUNDARY_TOP) {
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
