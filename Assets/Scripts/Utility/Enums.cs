using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum AttributeType
{
    Color,
    Speed,
    ShotLevel,
    LaserLevel,
    Module,
    Bomb
}

public enum PlayState
{
    OutGame = -1,
    OnField,
    OnMiddleBoss,
    OnBoss,
    OnBossCleared,
    OnStageResult,
    OnStageTransition
}

public enum GameDifficulty
{
    Normal,
    Expert,
    Hell
}

public enum GameMode
{
    Normal = 0,
    Training = 1,
    Replay = 2
}

public enum UnitMovementType {
    MoveVector,
    MoveTarget
}

public enum ScreenModeSetting
{
    Windowed,
    FullScreen
}

public enum QualitySetting
{
    Ultra,
    VeryHigh,
    High,
    Medium,
    Low,
    VeryLow
}

public enum AntiAliasingSetting
{
    Deactivated,
    Activated
}

public enum Language {
    English,
    Korean
}

public enum EnemyType
{
    Zako,
    MiddleBoss,
    Boss
}

public enum DebrisType
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

public enum ItemType
{
    PowerUp,
    Bomb,
    Life,
    GemGround,
    GemAir
}

public enum PoolingParent
{
    None = -1,
    PlayerMissile,
    EnemyBullet,
    Explosion,
    GemAir,
    GemGround,
    Debris,
    ScoreText
}