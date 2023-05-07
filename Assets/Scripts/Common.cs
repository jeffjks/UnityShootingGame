using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{
}


public static class Depth // Z Axis
{
    // Far
    public const int ENEMY = 32 + (int) Size.MAIN_CAMERA_POS; // Only Air Enemy
    public const int EXPLOSION = 23 + (int) Size.MAIN_CAMERA_POS; // Only Air Explosion
    public const int OVERVIEW = 22 + (int) Size.MAIN_CAMERA_POS;
    public const int ITEMS = 21 + (int) Size.MAIN_CAMERA_POS;
    public const int TRANSITION = 20 + (int) Size.MAIN_CAMERA_POS;
    public const int PLAYER = 19 + (int) Size.MAIN_CAMERA_POS; // Player, Laser
    public const int PLAYER_MISSILE = 18 + (int) Size.MAIN_CAMERA_POS;
    public const int HIT_EFFECT = 17 + (int) Size.MAIN_CAMERA_POS;
    public const int WHITE_EFFECT = 16 + (int) Size.MAIN_CAMERA_POS;
    public const int SCORE_TEXT = 15 + (int) Size.MAIN_CAMERA_POS;
    public const int ENEMY_BULLET = 14 + (int) Size.MAIN_CAMERA_POS;
    public const int CAMERA = 0 + (int) Size.MAIN_CAMERA_POS;
    // Close
}

public static class Layer // AirSmall(9), AirLarge(10), GroundSmall(11), GroundLarge(12)
{
    public const int SMALL = 2560; // (1 << 9 | 1 << 11) LayerMask.GetMask("AirSmallEnemy", "GroundSmallEnemy")
    public const int LARGE = 5120; // (1 << 10 | 1 << 12) LayerMask.GetMask("AirLargeEnemy", "GroundLargeEnemy")
    public const int AIR = 1536; // (1 << 9 | 1 << 10) LayerMask.GetMask("AirSmallEnemy", "AirLargeEnemy")
    public const int GROUND = 6144; // (1 << 11 | 1 << 12) LayerMask.GetMask("GroundSmallEnemy", "GroundLargeEnemy")
}

public static class ItemScore // Z Axis
{
    // Far
    public const ushort GEM_GROUND = 200;
    public const ushort GEM_AIR = 100;
    public const ushort POWERUP = 2000;
    public const ushort BOMB = 5000;
    // Close
}

public static class BonusScale // Overview
{
    public const float BONUS_0 = 0.5f;
    public const float BONUS_1 = 0.3f;
    public const float BONUS_2 = 0.1f;
}

public static class Difficulty
{
    public const int DIFFICULTY_SIZE = 3;
    public const string DIFFICULTY1 = "Normal";
    public const string DIFFICULTY2 = "Expert";
    public const string DIFFICULTY3 = "Hell";
    public const int NORMAL = 0;
    public const int EXPERT = 1;
    public const int HELL = 2;
}

public enum GameType
{
    GAMETYPE_NORMAL = 0,
    GAMETYPE_TRAINING = 1,
    GAMETYPE_REPLAY = 2
}

public enum DebugDifficulty
{
    Normal,
    Expert,
    Hell,
}

public interface UseObjectPool { // TODO : 구현
    public void ReturnToPool();
}

public interface IExplosionCreater {
    public void StartExplosion();
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
    Boss
}

public enum Debris
{
    Small,
    Medium,
    Large
};

public enum PlayerDamageType
{
    Normal,
    Laser,
    LaserAura,
    Bomb
}

public enum ExplType
{
    None = -1,
    Ground_1,
    Ground_2,
    Ground_3,
    Normal_1,
    Normal_2,
    Normal_3,
    Simple_1,
    Simple_2,
    StarShape,
    MineShape,
};

public enum ExplAudioType
{
    None = -1,

    AirMedium_1,
    AirMedium_2,
    AirSmall,
    GroundMedium,
    GroundSmall,
    Huge_1,
    Huge_2,
    Large,
    Player = 11
};

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
    ExplosionMine
};

public enum HealthType {
    None,
    Independent,
    Share
}

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
    public float[] speed;
    public float[] direction;

    public Effect() {
        this.position = new JVector3(Vector3.zero);
        this.radius = 0f;
        this.speed = new float[2] { 0f, 0f };
        this.direction = new float[2] { 0f, 0f };
    }

    public Effect(ExplType explType, ExplAudioType explAudioType) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(Vector3.zero);
        this.radius = 0f;
        this.speed = new float[2] { 0f, 0f };
        this.direction = new float[2] { 0f, 0f };
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = 0f;
        this.speed = new float[2] { 0f, 0f };
        this.direction = new float[2] { 0f, 0f };
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, MoveVector moveVector) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = 0f;
        this.speed = new float[2] { moveVector.speed, moveVector.speed };
        this.direction = new float[2] { moveVector.direction, moveVector.direction };
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, float[] speed, float[] direction) {
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
        this.speed = new float[2] { 0f, 0f };
        this.direction = new float[2] { 0f, 0f };
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, float radius, MoveVector moveVector) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = radius;
        this.speed = new float[2] { moveVector.speed, moveVector.speed };
        this.direction = new float[2] { moveVector.direction, moveVector.direction };
    }

    public Effect(ExplType explType, ExplAudioType explAudioType, Vector3 position, float radius, float[] speed, float[] direction) {
        this.explType = explType;
        this.explAudioType = explAudioType;
        this.position = new JVector3(position);
        this.radius = radius;
        this.speed = speed;
        this.direction = direction;
    }
}

public class Coroutine {
    public int duration;
    public int[] timer_add;
    public int number;

    public Coroutine() {
        this.duration = 0;
        this.timer_add = new int[] { 0, 0 };
        this.number = 0;
    }

    public Coroutine(int duration, int timer_add, int number) {
        this.duration = duration;
        this.timer_add = new int[] { timer_add, timer_add };
        this.number = number;
    }

    public Coroutine(int duration, int[] timer_add, int number) {
        this.duration = duration;
        this.timer_add = timer_add;
        this.number = number;
    }
}

[System.Serializable]
public struct ExplosionData {
    public Effect effect;
    public Coroutine coroutine;
    public int waitAfter;

    public ExplosionData(Effect effect, Coroutine coroutine, int waitAfter) {
        this.effect = effect;
        this.coroutine = coroutine;
        this.waitAfter = waitAfter;
    }
}