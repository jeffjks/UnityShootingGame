using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{
}


public interface UseObjectPool { // TODO : 구현
    void ReturnToPool();
}

public class BulletDirection
{
    public const byte FIXED = 0;
    public const byte PLAYER = 1;
    public const byte CURRENT = 2;
}

public struct MoveVector
{
    public float speed, direction;

    public MoveVector(float speed, float direction) {
        this.speed = speed;
        this.direction = direction;
    }

    public MoveVector(Vector2 vector2) {
        this.speed = vector2.magnitude;
        this.direction = Vector2.SignedAngle(Vector2.down, vector2);
    }

    public Vector2 GetVector() {
        Vector2 vector2 = Quaternion.AngleAxis(this.direction, Vector3.forward) * Vector2.down;
        vector2 = vector2.normalized * speed;
        return vector2;
    }
}

public class EaseType
{
    public const int Linear = 0;
    public const int OutQuad = 1;
    public const int InQuad = 2;
    public const int InOutQuad = 3;
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

public class TweenData
{
    public int duration; // duration동안 대기(TweenData) 혹은 duration에 걸쳐서 변화(TweenDataMovement)

    public TweenData(int duration) {
        this.duration = duration;
    }
}

public class TweenDataMoveVector : TweenData
{
    public MoveVector moveVector;
    public int easeType = EaseType.Linear;

    public TweenDataMoveVector(MoveVector moveVector, int duration = 0, int easeType = EaseType.Linear) : base(duration) {
        this.moveVector = moveVector;
        this.duration = duration;
        this.easeType = easeType;
    }
}

public class TweenDataPosition : TweenData
{
    public Vector3 position;
    public int easeType = EaseType.Linear;

    public TweenDataPosition(Vector3 position, int duration = 0, int easeType = EaseType.Linear) : base(duration) {
        this.position = position;
        this.duration = duration;
        this.easeType = easeType;
    }
}

public enum EnemyType
{
    Zako,
    MiddleBoss,
    Boss,
}

public enum Debris
{
    None,
    Small,
    Medium,
    Large,
};

public enum PlayerDamageType
{
    Normal,
    Laser,
    LaserAura,
    Bomb
}

public enum Explosion
{
    None,
    ExplosionG_1,
    ExplosionG_2,
    ExplosionG_3,
    Explosion_1,
    Explosion_2,
    Explosion_3,
    ExplosionSimple_1,
    ExplosionSimple_2,
    ExplosionStar,
    ExplosionMine,
};

public enum HealthType {
    None,
    Independent,
    Share,
}