using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct BulletProperty
{
    public Vector3 startPos;
    public BulletImage image;
    public float speed;
    public BulletPivot pivot;
    public float direction;
    public BulletAccel accel;
    public int number;
    public float interval;

    public BulletProperty(Vector3 startPos, BulletImage image, float speed, BulletPivot pivot, float direction, BulletAccel accel, int number = 1, float interval = 0f)
    {
        this.startPos = startPos;
        this.image = image;
        this.speed = speed;
        this.pivot = pivot;
        this.direction = direction;
        this.accel = accel;
        this.number = number;
        this.interval = interval;
    }

    public BulletProperty(Vector3 startPos, BulletImage image, float speed, BulletPivot pivot, float direction, int number = 1, float interval = 0f)
    {
        this.startPos = startPos;
        this.image = image;
        this.speed = speed;
        this.pivot = pivot;
        this.direction = direction;
        accel = default;
        this.number = number;
        this.interval = interval;
    }
}

public struct BulletAccel // Target Value는 0이면 적용 안됨
{
    public float targetSpeed;
    public int duration;

    public BulletAccel(float targetSpeed, int duration) {
        this.targetSpeed = targetSpeed;
        this.duration = duration;
    }
}

public struct BulletSpawnTiming
{
    public BulletSpawnType spawnType;
    public int delay;
    public Vector2Int period;

    public BulletSpawnTiming(BulletSpawnType spawnType, int delay, Vector2Int period = new())
    {
        this.spawnType = spawnType;
        this.delay = delay;
        this.period = period;
    }
}

public interface IBulletPattern
{
    public IEnumerator ExecutePattern(UnityAction onCompleted = null);
}

public class BulletFactory
{
    protected readonly EnemyObject _enemyObject;
    
    protected Vector3 CurrentPos => _enemyObject.transform.position;
    
    private const float SAFE_LINE = -11f;

    protected BulletFactory(EnemyObject enemyObject)
    {
        _enemyObject = enemyObject;
    }

    protected Vector3 GetFirePos(int index)
    {
        return _enemyObject.m_FirePosition[index].position;
    }

    protected Vector3 GetFirePos(int index, float gap)
    {
        return _enemyObject.m_FirePosition[index].TransformPoint(new Vector3(gap, 0f, 0f));
    }
    
    /*
    protected EnemyBullet[] CreateMultipleBullets(Vector3 pos, BulletProperty property)
    {
        var num = property.number;
        EnemyBullet[] enemyBullets = new EnemyBullet[num];
        var mainDirection = property.direction;
        
        for (var i = 0; i < num; ++i)
        {
            property.direction = mainDirection - property.interval * (num - i * 2 - 1) / 2;
            enemyBullets[i] = CreateBullet(pos, property);
        }

        return enemyBullets;
    }

    protected EnemyBullet[] CreateMultipleBullets(Vector3 pos, BulletProperty property, BulletSpawnTiming spawnTiming, BulletProperty subProperty)
    {
        var num = property.number;
        EnemyBullet[] enemyBullets = new EnemyBullet[num];
        var mainDirection = property.direction;
        
        for (var i = 0; i < num; ++i)
        {
            property.direction = mainDirection - property.interval * (num - i * 2 - 1) / 2;
            enemyBullets[i] = CreateBullet(pos, property, spawnTiming, subProperty);
        }

        return enemyBullets;
    }*/

    protected List<EnemyBullet> CreateBullet(BulletProperty property)
    {
        if (!_enemyObject.m_IsAir)
            property.startPos = BackgroundCamera.GetScreenPosition(property.startPos);
        property.startPos.z = Depth.ENEMY_BULLET;
        
        if (!CanCreateBullet(property.startPos))
            return new List<EnemyBullet>();

        var num = property.number;
        var mainDirection = property.direction;
        List<EnemyBullet> enemyBullets = new(num);
        
        if (num <= 0)
        {
            Debug.LogWarning("유효하지 않은 num 값입니다. 1로 재조정합니다.");
            num = 1;
        }
        
        for (var i = 0; i < num; ++i)
        {
            property.direction = mainDirection - property.interval * (num - i * 2 - 1) / 2;
            
            GameObject bulletObject = PoolingManager.PopFromPool("EnemyBullet", PoolingParent.EnemyBullet);
            enemyBullets.Add(bulletObject.GetComponent<EnemyBullet>());
            enemyBullets[i].transform.position = property.startPos;
            enemyBullets[i].OwnerEnemyObject = _enemyObject;
            
            bulletObject.SetActive(true);
            enemyBullets[i].OnStart(property);
        }
        
        return enemyBullets;
    }

    protected List<EnemyBullet> CreateBullet(BulletProperty property, BulletSpawnTiming spawnTiming, BulletProperty subProperty)
    {
        if (!_enemyObject.m_IsAir)
            property.startPos = BackgroundCamera.GetScreenPosition(property.startPos);
        property.startPos.z = Depth.ENEMY_BULLET;
        
        if (!CanCreateBullet(property.startPos))
            return new List<EnemyBullet>();

        var num = property.number;
        var mainDirection = property.direction;
        List<EnemyBullet> enemyBullets = new(num);
        
        if (num <= 0)
        {
            Debug.LogWarning("유효하지 않은 num 값입니다. 1로 재조정합니다.");
            num = 1;
        }
        
        for (var i = 0; i < num; ++i)
        {
            property.direction = mainDirection - property.interval * (num - i * 2 - 1) / 2;
            
            GameObject bulletObject = PoolingManager.PopFromPool("EnemyBullet", PoolingParent.EnemyBullet);
            enemyBullets.Add(bulletObject.GetComponent<EnemyBullet>());
            enemyBullets[i].transform.position = property.startPos;
            enemyBullets[i].OwnerEnemyObject = _enemyObject;
            
            bulletObject.SetActive(true);
            enemyBullets[i].OnStart(property, spawnTiming, subProperty);
        }
        
        return enemyBullets;
    }

    private bool CanCreateBullet(Vector3 pos) {
        var mainCameraPosX = MainCamera.Instance.GetCameraScreenPosition().x;
        var limitLine = (_enemyObject is EnemyBullet) ? -Size.GAME_HEIGHT : SAFE_LINE;

        if (2 * Mathf.Abs(pos.x - mainCameraPosX) > Size.MAIN_CAMERA_WIDTH)
            return false;
        if (pos.y > 0)
            return false;
        if (pos.y < limitLine)
            return false;
        if (!_enemyObject.IsInteractable())
            return false;

        if (GameManager.IsDebugScene)
            return true;
        
        if (!PlayerManager.IsPlayerAlive)
            return false;
        if (!SystemManager.IsOnGamePlayState())
            return false;
        
        return true;
    }
}
