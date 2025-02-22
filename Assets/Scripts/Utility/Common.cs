using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using UnityEngine;


[Serializable]
public static class Size
{
    public const float MAIN_CAMERA_DEPTH = -64f;
    public const float MAIN_CAMERA_WIDTH = 12f; // 정확히는 카메라 콜라이더
    public const float MAIN_CAMERA_HEIGHT = 16f;
    public const float GAME_WIDTH = 15.11f;
    public const float GAME_HEIGHT = 16f;

    public const float CAMERA_MOVE_LIMIT = 4.895f; // 카메라가 움직이는 플레이어의 최대/최소 x값

    public const float GAME_BOUNDARY_LEFT = - GAME_WIDTH / 2;
    public const float GAME_BOUNDARY_RIGHT = GAME_WIDTH / 2;
    public const float GAME_BOUNDARY_BOTTOM = -GAME_HEIGHT;
    public const float GAME_BOUNDARY_TOP = 0f;

    public const float BACKGROUND_CAMERA_ANGLE = 25f;
}

public static class Depth // Z Axis
{
    // Far
    public const int ENEMY = 32 + (int) Size.MAIN_CAMERA_DEPTH; // Only Air Enemy
    public const int EXPLOSION = 23 + (int) Size.MAIN_CAMERA_DEPTH; // Only Air Explosion
    public const int OVERVIEW = 22 + (int) Size.MAIN_CAMERA_DEPTH;
    public const int ITEMS = 21 + (int) Size.MAIN_CAMERA_DEPTH;
    public const int TRANSITION = 20 + (int) Size.MAIN_CAMERA_DEPTH;
    public const int PLAYER = 19 + (int) Size.MAIN_CAMERA_DEPTH; // Player, Laser
    public const int PLAYER_MISSILE = 18 + (int) Size.MAIN_CAMERA_DEPTH;
    public const int HIT_EFFECT = 17 + (int) Size.MAIN_CAMERA_DEPTH;
    public const int WHITE_EFFECT = 16 + (int) Size.MAIN_CAMERA_DEPTH;
    public const int SCORE_TEXT = 15 + (int) Size.MAIN_CAMERA_DEPTH;
    public const int ENEMY_BULLET = 14 + (int) Size.MAIN_CAMERA_DEPTH;
    public const int CAMERA = 0 + (int) Size.MAIN_CAMERA_DEPTH;
    // Close
}

public static class Layer // AirSmall(9), AirLarge(10), GroundSmall(11), GroundLarge(12)
{
    public const int SMALL = 2560; // (1 << 9 | 1 << 11) LayerMask.GetMask("AirSmallEnemy", "GroundSmallEnemy")
    public const int LARGE = 5120; // (1 << 10 | 1 << 12) LayerMask.GetMask("AirLargeEnemy", "GroundLargeEnemy")
    public const int AIR = 1536; // (1 << 9 | 1 << 10) LayerMask.GetMask("AirSmallEnemy", "AirLargeEnemy")
    public const int GROUND = 6144; // (1 << 11 | 1 << 12) LayerMask.GetMask("GroundSmallEnemy", "GroundLargeEnemy")
}

public struct TrainingInfo
{
    public int stage;
    public bool bossOnly;
}

public class WaitForFrames : CustomYieldInstruction
{
    private int _targetFrameCount;

    public WaitForFrames(int numberOfFrames)
    {
        _targetFrameCount = Time.frameCount + numberOfFrames;
    }

    public override bool keepWaiting
    {
        get
        {
            if (Time.timeScale == 0) {
                _targetFrameCount++;
            }
            return Time.frameCount < _targetFrameCount;
        }
    }
}

public class WaitForMillisecondFrames : CustomYieldInstruction
{
    private int _targetFrameCount;

    public WaitForMillisecondFrames(int millisecond)
    {
        int numberOfFrames = millisecond * Application.targetFrameRate / 1000;
        _targetFrameCount = Time.frameCount + numberOfFrames;
    }

    public override bool keepWaiting
    {
        get
        {
            if (Time.timeScale == 0) {
                _targetFrameCount++;
            }
            return Time.frameCount < _targetFrameCount;
        }
    }
}

public class AC_Ease
{
    public static readonly List<AnimationCurve> ac_ease = new();
}

[Serializable]
public class ShipAttributes
{
    private Dictionary<AttributeType, int> _attributes = new();

    public ShipAttributes() : this(0, 0, 0, 0, 0, 0, 0)
    {
    }
    
    public ShipAttributes(int color, int speed, int shotForm, int shotIndex, int laserIndex, int subWeaponIndex, int bomb)
    {
        _attributes[AttributeType.Color] = color;
        _attributes[AttributeType.Speed] = speed;
        _attributes[AttributeType.ShotIndex] = shotIndex;
        _attributes[AttributeType.LaserIndex] = laserIndex;
        _attributes[AttributeType.SubWeaponIndex] = subWeaponIndex;
        _attributes[AttributeType.Bomb] = bomb;
    }

    public override string ToString()
    {
        return
            $"{_attributes[AttributeType.Color]}, {_attributes[AttributeType.Speed]}, {_attributes[AttributeType.ShotIndex]}, {_attributes[AttributeType.LaserIndex]}, {_attributes[AttributeType.SubWeaponIndex]}, {_attributes[AttributeType.Bomb]}";
    }

    public ShipAttributes(string jsonCode)
    {
        _attributes = JsonConvert.DeserializeObject<Dictionary<AttributeType, int>>(jsonCode);
    }

    public void SetAttributes(AttributeType key, int value)
    {
        _attributes[key] = value;
    }

    public int GetAttributes(AttributeType key)
    {
        return _attributes[key];
    }

    public string GetAttributesCode() {
        return JsonConvert.SerializeObject(_attributes, Formatting.None);
    }
    
    public static bool operator ==(ShipAttributes op1, ShipAttributes op2)
    {
        if (op1 == null || op2 == null)
        {
            Debug.LogWarning("Compared null shipAttributes and returned false.");
            return false;
        }
        var dic1 = op1._attributes;
        var dic2 = op2._attributes;
        return dic1.Equals(dic2);
    }

    public static bool operator !=(ShipAttributes op1, ShipAttributes op2) {
        return !(op1 == op2);
    }

    public override bool Equals(object op)
    {
        return (this == ((ShipAttributes) op));
    }
    
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

/*
public static class BulletType
{
    public const byte NORMAL = 0; // 일반 총알
    public const byte CREATE = 1; // n초후 다른 총알 생성
    public const byte ERASE_AND_CREATE = 2; // n초후 파괴 후 다른 총알 생성
}*/

[Serializable]
public class Bullet
{
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
    public int eraseIndex;
    public float triggerBodyRadius;
}

public interface IObjectPooling {
    public void ReturnToPool();
}

public struct MoveVector
{
    public float speed, direction;

    public MoveVector(float speed, float direction) {
        this.speed = speed;
        this.direction = direction;
    }

    public MoveVector(Vector2 vector2) {
        speed = vector2.magnitude;
        direction = Vector2.SignedAngle(Vector2.down, vector2);
    }

    public Vector2 GetVector() {
        Vector2 vector2 = Quaternion.AngleAxis(direction, Vector3.forward) * Vector2.down;
        vector2 = vector2.normalized * speed;
        return vector2;
    }
}

public enum EaseType
{
    Linear,
    OutQuad,
    InQuad,
    InOutQuad
}

/*
public class TweenData<T, TResult> {
    public Func<T> getter;
    public Action<TResult> setter;

    TweenData(Func<T> getter, Action<TResult> setter) {
        this.getter = getter;
        this.setter = setter;
    }
}*/


[Serializable]
public abstract class UnitMovement {
    public int delay;
    public int duration;
}

[Serializable]
public class MovePattern : UnitMovement
{
    public bool keepSpeed;
    [DrawIf("keepSpeed", true, ComparisonType.NotEqual)]
    public float speed;
    public bool keepDirection;
    [DrawIf("keepDirection", true, ComparisonType.NotEqual)]
    public float direction;

    public MovePattern(int duration) {
        this.keepSpeed = true;
        this.keepDirection = true;
        this.duration = duration;
    }

    public MovePattern(int delay, int duration, float direction, float speed) {
        this.delay = delay;
        this.duration = duration;
        this.speed = speed;
        this.direction = direction;
    }

    public MovePattern(int delay, int duration, bool keepDirection, float speed) {
        this.delay = delay;
        this.duration = duration;
        this.speed = speed;
        this.keepDirection = keepDirection;
    }

    public MovePattern(int delay, int duration, float direction, bool keepSpeed) {
        this.delay = delay;
        this.duration = duration;
        this.keepSpeed = keepSpeed;
        this.direction = direction;
    }
}

[Serializable]
public class MoveTarget : UnitMovement
{
    public Vector2 targetVector2;

    public MoveTarget(int duration, Vector2 targetVector2) {
        this.duration = duration;
        this.targetVector2 = targetVector2;
    }

    public MoveTarget(int delay, int duration, Vector2 targetVector2) {
        this.delay = delay;
        this.duration = duration;
        this.targetVector2 = targetVector2;
    }
}

public class TweenData
{
    public UnitMovement unitMovement;
    public readonly EaseType easeType = EaseType.Linear;

    public TweenData(UnitMovement unitMovement, EaseType easeType = EaseType.Linear) {
        this.unitMovement = unitMovement;
        this.easeType = easeType;
    }
}

[Serializable]
public struct DetailsWindowElement
{
    public Sprite sprite;
    public int cost;
    public string name;
    public string nativeName;
    [TextArea(4, 6)]
    public string description;
    [TextArea(4, 6)]
    public string nativeDescription;
}

/*
public class TweenDataPosition
{
    public Vector3 position;
    public int easeType = EaseType.Linear;

    public TweenDataPosition(Vector3 position, int duration = 0, int easeType = EaseType.Linear) : base(duration) {
        this.position = position;
        this.duration = duration;
        this.easeType = easeType;
    }
}*/

public struct JVector3
{
    public float x;
    public float y;
    public float z;

    public JVector3 (Vector3 v) {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public JVector3(float f) {
        x = y = z = f;
    }
    
    public static implicit operator Vector3(JVector3 jVector)
    {
        return new Vector3(jVector.x, jVector.y, jVector.z);
    }
}

public class Effect {
    public ExplType explType;
    public ExplAudioType explAudioType;
    public JVector3 position;
    public float radius;
    public PairFloat speed;
    public PairFloat direction;

    public Effect() {
        this.position = new JVector3(Vector3.zero);
        this.radius = 0f;
        this.speed = new PairFloat(0f);
        this.direction = new PairFloat(0f);
    }

    public Effect(ExplType explType, ExplAudioType explAudioType) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(Vector3.zero);
        this.radius = 0f;
        this.speed = new PairFloat(0f);
        this.direction = new PairFloat(0f);
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = 0f;
        this.speed = new PairFloat(0f);
        this.direction = new PairFloat(0f);
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, MoveVector moveVector) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = 0f;
        this.speed = new PairFloat(moveVector.speed);
        this.direction = new PairFloat(moveVector.direction);
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, PairFloat speed, PairFloat direction) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = 0f;
        this.speed = speed;
        this.direction = direction;
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, float radius) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = radius;
        this.speed = new PairFloat(0f);
        this.direction = new PairFloat(0f);
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, float radius, MoveVector moveVector) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = radius;
        this.speed = new PairFloat(moveVector.speed);
        this.direction = new PairFloat(moveVector.direction);
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, float radius, PairFloat speed, PairFloat direction) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = radius;
        this.speed = speed;
        this.direction = direction;
    }
}

public class ExplosionCoroutine {
    public int duration;
    public PairInt timer_add;
    public int number;

    public ExplosionCoroutine() {
        this.duration = 0;
        this.timer_add = new PairInt(0);
        this.number = 0;
    }

    public ExplosionCoroutine(int duration, int timer_add, int number) {
        this.duration = duration;
        this.timer_add = new PairInt(timer_add);
        this.number = number;
    }

    public ExplosionCoroutine(int duration, PairInt timer_add, int number) {
        this.duration = duration;
        this.timer_add = timer_add;
        this.number = number;
    }
}

[Serializable]
public struct ExplosionData {
    public Effect effect;
    public ExplosionCoroutine coroutine;
    public int waitAfter;

    public ExplosionData(Effect effect, ExplosionCoroutine coroutine, int waitAfter) {
        this.effect = effect;
        this.coroutine = coroutine;
        this.waitAfter = waitAfter;
    }
}

[Serializable]
public struct PairInt {
    public int value1, value2;

    public PairInt(int value) {
        this.value1 = value;
        this.value2 = value;
    }

    public PairInt(int value1, int value2) {
        this.value1 = value1;
        this.value2 = value2;
    }

    public int this[int index] {
        get {
            switch (index) {
                case 0:
                    return value1;
                case 1:
                    return value2;
                default:
                    throw new System.IndexOutOfRangeException();
            }
        }
    }
}

[Serializable]
public struct PairFloat {
    public float value1, value2;

    public PairFloat(float value) {
        this.value1 = value;
        this.value2 = value;
    }

    public PairFloat(float value1, float value2) {
        this.value1 = value1;
        this.value2 = value2;
    }

    public float this[int index] {
        get {
            switch (index) {
                case 0:
                    return value1;
                case 1:
                    return value2;
                default:
                    throw new System.IndexOutOfRangeException();
            }
        }
    }
}

public class LocalRankingData : IComparable<LocalRankingData> {
    public string id;
    public long score;
    public ShipAttributes shipAttributes;
    public int miss;
    public long date;

    public LocalRankingData(string id, long score, ShipAttributes shipAttributes, int miss, long date) {
        this.id = id;
        this.score = score;
        this.shipAttributes = shipAttributes;
        this.miss = miss;
        this.date = date;
    }

    public int CompareTo(LocalRankingData other) {
        if (score == other.score) {
            if (date < other.date) {
                return 1;
            }
            return -1;
        }
        if (score > other.score) {
            return 1;
        }
        return -1;
    }

    // 자신 기록과 비교
    public bool isBetter(LocalRankingData localRankingData) {
        if (id != localRankingData.id) {
            return false;
        }
        if (score > localRankingData.score) {
            return false;
        }
        if (shipAttributes != localRankingData.shipAttributes) {
            return false;
        }
        if (miss < localRankingData.miss) {
            return false;
        }
        if (date < localRankingData.date) {
            return false;
        }
        return true;
    }

    public void Print() {
        string attributesCode = shipAttributes.GetAttributesCode();
        Debug.Log($"{id}, {score}, {attributesCode}, {miss}, {date}");
    }
}

[Serializable]
public struct MusicInfo
{
    public string musicName;
    public AudioClip musicAudio;
    public float loopStartPoint;
    public float loopEndPoint;
}

[Serializable]
public struct SoundInfo
{
    public string soundName;
    public AudioClip soundAudio;
}

[Serializable]
public struct SoundExplosionInfo
{
    public ExplAudioType explosionAudioType;
    public AudioClip soundAudio;
}

public class EnemyBuilder {
    private GameObject _prefab;
    private Vector3 _spawnPosition;
    private MoveVector _moveVector;
    private Queue<TweenData> _tweenDataQueue = new Queue<TweenData>();

    public EnemyBuilder(GameObject prefab) {
        _prefab = prefab;
    }

    public EnemyBuilder SetPosition(Vector3 pos) {
        if (_prefab.CheckLayer(Layer.AIR)) {
            _spawnPosition = new Vector3(pos.x, pos.y, Depth.ENEMY);
        }
        else
        {
            _spawnPosition = pos;
        }
        return this;
    }

    public EnemyBuilder AddTarget(int duration, Vector2 targetVector2) {
        _tweenDataQueue.Enqueue(new TweenData(new MoveTarget(duration, targetVector2)));
        return this;
    }

    public EnemyBuilder SetMoveVector(float speed, float direction) {
        _moveVector = new MoveVector(speed, direction);
        return this;
    }

    public GameObject Build() {
        var instance = UnityEngine.Object.Instantiate(_prefab, _spawnPosition, Quaternion.identity);
        EnemyUnit enemyUnit = instance.GetComponent<EnemyUnit>();
        enemyUnit.m_TweenDataQueue = _tweenDataQueue;
        enemyUnit.m_MoveVector = _moveVector;
        return instance;
    }
}